using System.Collections;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;
public class DiceSystem 
{
    private RollTheDice _plugin;
    private Dictionary<ulong, int>? _plyRollCounter;
    private Dictionary<ulong, Effect>? _plysActiveEffect = new();

    public DiceSystem()
    {
        _plugin = RollTheDice.Instance!;
        
        _plyRollCounter = new Dictionary<ulong, int>();

        _plugin.RegisterEventHandler<EventPlayerDisconnect>(HandlePlayerDisconnect);
        _plugin.RegisterEventHandler<EventPlayerDeath>(HandlePlayerDeath);
        _plugin.RegisterEventHandler<EventRoundStart>(HandleRoundStart);
        _plugin.RegisterEventHandler<EventPlayerHurt>(HandlePlayerHurt);
    }

    public void ResetState()
    {
        if(_plyRollCounter != null)
            _plyRollCounter.Clear();
        else
            _plyRollCounter = new Dictionary<ulong, int>();

        if(_plysActiveEffect != null)
            _plysActiveEffect.Clear();
        else
            _plysActiveEffect = new Dictionary<ulong, Effect>();
    }

    private int GetConfigValueInt(string key)
    {
        try {
            if(Config.ConfigData.General.TryGetValue(key, out var output))
            {
                return (int)output;
            }
        }
        catch(Exception e)
        {
            PluginFeedback.WriteConsole($"Error while getting config value for {key} (Contact Server Owner)", FeedbackType.Error);
            PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
            PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Error);
        }
        
        return 0;
    }

    private bool GetConfigValueBool(string key)
    {
        try {
            if(Config.ConfigData.General.TryGetValue(key, out var output))
            {
                JsonElement value = (JsonElement)output;
                return value.GetBoolean();
            }
        }
        catch(Exception e)
        {
            PluginFeedback.WriteConsole($"Error while getting config value for {key} (Contact Server Owner)", FeedbackType.Error);
            PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
            PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Error);
        }
        
        return false;
    }

    private void RemoveOrResetPlyDiceCounter(CCSPlayerController plyController, bool isRemove)
    {
        if(!plyController.IsValidPly())
            return;

        ulong plyId = plyController.GetPlyId();

        if(!_plyRollCounter!.ContainsKey(plyId))
            return;

        if(isRemove)
            _plyRollCounter.Remove(plyId);
        else 
            _plyRollCounter[plyId] = GetConfigValueInt("DiceRollPerRound");
    }

    public bool CanRoll(CCSPlayerController plyController)
    {
        ulong plyId = plyController.GetPlyId();

        if(!_plyRollCounter!.ContainsKey(plyId))
            _plyRollCounter.Add(plyId, GetConfigValueInt("DiceRollPerRound"));

        int plyRollAmountLeft = --_plyRollCounter[plyId];

        if(plyRollAmountLeft < 0)
        {
            plyController.CustomPrint("You can not roll the dice anymore for this round!"
                    .__("dice_already_rolled"), FeedbackType.Warning);

            return false;
        }

        if(plyRollAmountLeft > 0)
            plyController.CustomPrint($"You have $(mark){plyRollAmountLeft}$(default) rolls left for this round!"
                .__("dice_rolls_left", plyRollAmountLeft+""));

        return true;
    }

    private bool CheckTeamAndLifeState(CCSPlayerController plyController)
    {

        if(plyController.PlayerPawn.Value.LifeState != 0)
        {
            plyController.CustomPrint("You can not roll the dice while dead!"
                    .__("dice_cant_roll_dead"), FeedbackType.Warning);

            return false;
        }

        bool canCTRoll = GetConfigValueBool("CTsCanRoll");
        bool canTRoll = GetConfigValueBool("TsCanRoll");

        var teamName = "";
        switch(plyController.TeamNum)
        {
            case 2: 
                    teamName = "Terrorist";
                break;
            case 3: 
                    teamName = "Counter Terrorist";
                break;
            default:
                    teamName = "Unknown";
                break;
        }

        if(!canTRoll && plyController.TeamNum == 2 || !canCTRoll && plyController.TeamNum == 3)
        {
            plyController.CustomPrint($"You can not roll as a $(mark){teamName}"
                .__("dice_wrong_team", teamName), FeedbackType.Warning);

            return false;
        }

        return true;
    }

    public void RollDice(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly() || !CheckTeamAndLifeState(plyController) || !CanRoll(plyController))
            return;

        RollAndApplyEffect(plyController);
    }

    private Effect? GetEffectByRoll()
    {
        var effectsList = Effect.Effects;

        if(effectsList == null)
        {
            PluginFeedback.PrintBroadcast("Dice effects are null", FeedbackType.Error);
            return null;
        }

        double roll = Random.Shared.NextDouble() * Effect.TotalCumulativeProbability;

        return effectsList
                .Where(e => e.Enabled)
                .OrderBy(e => e.CumulativeProbability)
                .FirstOrDefault(e => roll <= e.CumulativeProbability);
    }

    public void RollAndApplyEffect(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        Effect? effect = GetEffectByRoll()!;

        if(effect == null)
            return;

        ulong plyId = plyController.GetPlyId();

        if(_plysActiveEffect!.ContainsKey(plyId))
            _plysActiveEffect[plyId] = effect;
        else
            _plysActiveEffect.Add(plyId, effect);


        bool localMessage = GetConfigValueBool("DiceRollMessageLocal");
        bool broadcastMessage = GetConfigValueBool("DiceRollMessageBroadcast");

        if(localMessage)
        {
            plyController.CustomPrint($"You rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_local", effect.RollNumber+"", effect.PrettyName));
        }

        if(broadcastMessage)
        {
            PluginFeedback.PrintBroadcast($"$(mark){plyController.PlayerName}$(default) rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_broadcast", plyController.PlayerName, effect.RollNumber+"", effect.PrettyName));
        }

        var effectAction = effect.EffectAction;
        if(effectAction != null)
            effectAction(effect, plyController);
        else 
            plyController.CustomPrint(effect.Description, FeedbackType.Chat);
    }

    private void RemoveOrResetPlyActiveEffects(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        ulong plyId = plyController.GetPlyId();

        if(!_plysActiveEffect!.ContainsKey(plyId))
            return;

        _plysActiveEffect.Remove(plyId);
    }

    #region Hooks

    public HookResult HandlePlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceCounter(plyController, true);
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(!GetConfigValueBool("ResetPlayerRollOnDeath"))
            return HookResult.Continue;

        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceCounter(plyController, false);
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(GetConfigValueBool("NotifyAtRoundStart"))
            PluginFeedback.PrintBroadcast("Enter $(mark)!rtd$(default) in chat to roll the dice!".__("dice_notify_round_start"), FeedbackType.Chat);

        if(!GetConfigValueBool("ResetRollsOnRoundStart"))
            return HookResult.Continue;

        _plysActiveEffect?.Clear();

        Utilities.GetPlayers().ForEach(plyController => 
        {
            RemoveOrResetPlyDiceCounter(plyController, false);
        });


        return HookResult.Continue;
    }

    public HookResult HandlePlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(!attackerController.IsValidPly() || !victimController.IsValidPly())
            return HookResult.Continue;

        ulong attackerId = attackerController.GetPlyId();

        if(_plysActiveEffect == null || !_plysActiveEffect.TryGetValue(@event.Attacker.SteamID, out Effect? effect))
            return HookResult.Continue;

        if(effect == null || effect.EffectHookAction == null || effect.EffectHookName != "PlayerHurt")
            return HookResult.Continue;

        effect.EffectHookAction(effect, @event, info);

        return HookResult.Continue;
    }

    #endregion
}
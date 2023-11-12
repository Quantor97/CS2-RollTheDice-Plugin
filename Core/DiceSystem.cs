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
            _plyRollCounter[plyId] = RollTheDice.Config!.GetConfigValue<int>("DiceRollsPerRound");
    }

    public bool CanRoll(CCSPlayerController plyController)
    {
        ulong plyId = plyController.GetPlyId();

        if(!_plyRollCounter!.ContainsKey(plyId))
            _plyRollCounter.Add(plyId, RollTheDice.Config!.GetConfigValue<int>("DiceRollsPerRound"));

        int plyRollAmountLeft = --_plyRollCounter[plyId];

        if(plyRollAmountLeft < 0)
        {
            plyController.CustomPrint("You can not roll the dice anymore for this round!"
                    .__("dice_already_rolled"), FeedbackType.Warning);

            return false;
        }

        if(plyRollAmountLeft > 0 && RollTheDice.Config!.GetConfigValue<bool>("UnicastRollAmount"))
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

        bool canCTRoll = RollTheDice.Config!.GetConfigValue<bool>("CTsCanRoll");
        bool canTRoll = RollTheDice.Config!.GetConfigValue<bool>("TsCanRoll");

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

    public void PreRollDice(CCSPlayerController plyController)
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
            PluginFeedback.WriteConsole("No effects found", FeedbackType.Error);
            return null;
        }

        double roll = Random.Shared.NextDouble() * Effect.TotalCumulativeProbability;

        return effectsList
                .Where(e => e.Enabled)
                .OrderBy(e => e.CumulativeProbability)
                .FirstOrDefault(e => roll <= e.CumulativeProbability);
    }

    private void BroadOrUnicastRollMessages(CCSPlayerController target, Effect effect)
    {
        bool localMessage = RollTheDice.Config!.GetConfigValue<bool>("UnicastDiceRoll");
        bool broadcastMessage = RollTheDice.Config!.GetConfigValue<bool>("BroadcastDiceRoll");
        bool broadcastMessageTerrorists = RollTheDice.Config!.GetConfigValue<bool>("BroadcastDiceRollTerroristOnly");
        bool broadcastMessageCounterTerrorists = RollTheDice.Config!.GetConfigValue<bool>("BroadcastDiceRolllCounterTerroristOnly");

        if(localMessage)
        {
            target.CustomPrint($"You rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_local", effect.RollNumber+"", effect.PrettyName));
        }

        if(broadcastMessage || (broadcastMessageTerrorists && broadcastMessageCounterTerrorists))
        {
            PluginFeedback.PrintBroadcast($"$(mark){target.PlayerName}$(default) rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_broadcast", target.PlayerName, effect.RollNumber+"", effect.PrettyName), null, true);
        }
        else if(broadcastMessageTerrorists || broadcastMessageCounterTerrorists)
        {
            Utilities.GetPlayers().ForEach(_plyController => 
            {
                if(!_plyController.IsValidPly())
                    return;

                bool broadcastTerroristPly = _plyController.TeamNum == 2 && broadcastMessageTerrorists;
                bool broadcastCounterTerroristPly = _plyController.TeamNum == 3 && broadcastMessageCounterTerrorists;
                
                if(broadcastTerroristPly || broadcastCounterTerroristPly)
                {
                    _plyController.CustomPrint($"$(mark){target.PlayerName}$(default) rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                            .__("dice_rolled_broadcast", target.PlayerName, effect.RollNumber+"", effect.PrettyName));
                }
            });
        }
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

        BroadOrUnicastRollMessages(plyController, effect);

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
        if(!RollTheDice.Config!.GetConfigValue<bool>("ResetRollsOnDeath"))
            return HookResult.Continue;

        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceCounter(plyController, false);
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(RollTheDice.Config!.GetConfigValue<bool>("BroadcastPluginCommand"))
            PluginFeedback.PrintBroadcast("Enter $(mark)!rtd$(default) in chat to roll the dice!".__("dice_notify_round_start"), FeedbackType.Chat);

        if(!RollTheDice.Config!.GetConfigValue<bool>("ResetRollsOnRoundStart"))
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
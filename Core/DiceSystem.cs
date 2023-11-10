using System.Collections;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;
public class DiceSystem 
{
    private RollTheDice _plugin;
    private Dictionary<ulong, int>? _plyDiceUseCounter;
    private Dictionary<ulong, Effect>? _plysActiveEffect = new();

    public DiceSystem()
    {
        _plugin = RollTheDice.Instance!;
        
        _plyDiceUseCounter = new Dictionary<ulong, int>();

        _plugin.RegisterEventHandler<EventPlayerDisconnect>(HandlePlayerDisconnect);
        _plugin.RegisterEventHandler<EventPlayerDeath>(HandlePlayerDeath);
        _plugin.RegisterEventHandler<EventRoundStart>(HandleRoundStart);
        _plugin.RegisterEventHandler<EventPlayerHurt>(HandlePlayerHurt);
    }

    public void ResetState()
    {
        if(_plyDiceUseCounter != null)
            _plyDiceUseCounter.Clear();
        else
            _plyDiceUseCounter = new Dictionary<ulong, int>();

        if(_plysActiveEffect != null)
            _plysActiveEffect.Clear();
        else
            _plysActiveEffect = new Dictionary<ulong, Effect>();
    }

    private int GetConfigRollPerRound()
    {
        return (int) Config.ConfigData.General["DiceRollPerRound"];
    }

    private bool GetConfigDiceRollMessageBroadcast()
    {
        return (bool) Config.ConfigData.General["DiceRollMessageBroadcast"];
    }

    private bool GetConfigDiceRollMessageLocal()
    {
        return (bool) Config.ConfigData.General["DiceRollMessageLocal"];
    }

    private void RemoveOrResetPlyDiceTimer(CCSPlayerController plyController, bool isRemove)
    {
        if(!plyController.IsValidPly())
            return;

        ulong plyId = plyController.GetPlyId();

        if(!_plyDiceUseCounter!.ContainsKey(plyId))
            return;

        if(isRemove)
            _plyDiceUseCounter.Remove(plyId);
        else 
            _plyDiceUseCounter[plyId] = GetConfigRollPerRound();
    }

    public bool CanRoll(CCSPlayerController plyController)
    {
        ulong plyId = plyController.GetPlyId();

        if(!_plyDiceUseCounter!.ContainsKey(plyId))
        {
            _plyDiceUseCounter.Add(plyId, GetConfigRollPerRound());
        }

        int plyRollAmountLeft = --_plyDiceUseCounter[plyId];

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

    public void RollDice(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        if(!CanRoll(plyController))
            return;

        RollAndApplyEffect(plyController);
    }

    private Effect? GetEffectByRoll(double roll)
    {
        var effectsList = Effect.Effects;
        if(effectsList == null)
        {
            PluginFeedback.PrintBroadcast("Dice effects are null (Contact Server Owner)", FeedbackType.Error);
            return null;
        }

        return effectsList
                .Where(e => e.Enabled)
                .OrderBy(e => e.CumulativeProbability)
                .FirstOrDefault(e => roll <= e.CumulativeProbability);
    }

    public void RollAndApplyEffect(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        double roll = Random.Shared.NextDouble() * Effect.TotalCumulativeProbability;; 
        Effect? effect = GetEffectByRoll(roll)!;

        if(effect == null)
            return;

        ulong plyId = plyController.GetPlyId();

        if(_plysActiveEffect!.ContainsKey(plyId))
            _plysActiveEffect[plyId] = effect;
        else
            _plysActiveEffect.Add(plyId, effect);


        if(GetConfigDiceRollMessageLocal())
        {
            plyController.CustomPrint($"You rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_local", effect.RollNumber+"", effect.Name));
        }

        if(GetConfigDiceRollMessageBroadcast())
        {
            PluginFeedback.PrintBroadcast($"$(mark){plyController.PlayerName}$(default) rolled a $(mark){effect.RollNumber}$(default) and got $(mark){effect.PrettyName}"
                    .__("dice_rolled_broadcast", plyController.PlayerName, effect.RollNumber+"", effect.Name));
        }

        var effectAction = effect.EffectAction;
        if(effectAction != null)
            effectAction(effect, plyController);
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
        RemoveOrResetPlyDiceTimer(plyController, true);
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceTimer(plyController, false);
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Utilities.GetPlayers().ForEach(plyController => 
        {
            RemoveOrResetPlyActiveEffects(plyController);
            RemoveOrResetPlyDiceTimer(plyController, false);
        });

        return HookResult.Continue;
    }

    public HookResult HandlePlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(!attackerController.IsValidPly() && !victimController.IsValidPly())
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
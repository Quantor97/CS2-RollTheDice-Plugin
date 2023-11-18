
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core;
public class DiceSystem 
{
    private RollTheDice _plugin;
    public Dictionary<ulong, int>? PlyRollCounter;

    public DiceSystem(RollTheDice plugin)
    {
        _plugin = plugin;
        
        PlyRollCounter = new Dictionary<ulong, int>();
    }

    public void ResetState()
    {
        PlyRollCounter?.Clear();
    }

    private void RemoveOrResetPlyDiceCounter(CCSPlayerController plyController, bool isRemove)
    {
        if(!plyController.IsValidPly())
            return;

        var handle = plyController.SteamID;

        if(!PlyRollCounter!.ContainsKey(handle))
            return;

        if(isRemove)
            PlyRollCounter.Remove(handle);
        else 
            PlyRollCounter[handle] = _plugin.Config.RollsPerRound;
    }

    public bool CanRoll(CCSPlayerController plyController)
    {
        var plyID = plyController.SteamID;

        if(!PlyRollCounter!.ContainsKey(plyID))
            PlyRollCounter.Add(plyID, _plugin.Config!.RollsPerRound);

        int plyRollAmountLeft = --PlyRollCounter[plyID];

        if(plyRollAmountLeft < 0)
        {
            plyController.LogChat("You can not roll the dice anymore for this round!"
                    .__("dice_already_rolled"), LogType.INFO);

            return false;
        }

        if(plyRollAmountLeft > 0 && _plugin.Config!.UnicastRollAmount)
            plyController.LogChat($"You have {{mark}}{plyRollAmountLeft}$(default) rolls left for this round!"
                .__("dice_rolls_left", plyRollAmountLeft+""));

        return true;
    }

    private bool CheckTeamAndLifeState(CCSPlayerController plyController)
    {

        if(!plyController.PawnIsAlive)
        {
            plyController.LogChat("You can not roll the dice while dead!"
                    .__("dice_cant_roll_dead"), LogType.INFO);

            return false;
        }

        bool canCTRoll = _plugin.Config!.CTsCanRoll;
        bool canTRoll = _plugin.Config!.TsCanRoll;

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
            plyController.LogChat($"You can not roll as a {{mark}}{teamName}"
                .__("dice_wrong_team", teamName), LogType.INFO);

            return false;
        }

        return true;
    }

    public void CheckPlayerStatus(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly() || !CheckTeamAndLifeState(plyController) || !CanRoll(plyController))
            return;

        RollAndApplyEffect(plyController);
    }

    private EffectBase? GetEffectByRoll()
    {
        var effectsList = EffectBase.Effects;

        if(effectsList == null)
        {
            Log.PrintServerConsole("No effects found", LogType.ERROR);
            return null;
        }

        double roll = Random.Shared.NextDouble() * EffectBase.TotalCumulativeProbability;

        return effectsList
                .Where(e => e.Enabled)
                .OrderBy(e => e.CumulativeProbability)
                .FirstOrDefault(e => roll <= e.CumulativeProbability);
    }

    private void BroadOrUnicastRollMessages(CCSPlayerController target, EffectBase effect)
    {
        bool localMessage = _plugin.Config!.UnicastOnRollMessage;
        bool broadcastMessage = _plugin.Config!.BroadcastOnRollMessage;
        bool broadcastMessageTerrorists = _plugin.Config!.BroadcastOnRollMessageTerrorists;
        bool broadcastMessageCounterTerrorists = _plugin.Config!.BroadcastOnRollMessageCounterTerrorists;

        var broadcastRollMessage = $"{{mark}}{target.PlayerName}{{default}} rolled a {{mark}}{effect.RollNumber}{{mark}} and got {{mark}}{effect.PrettyName}"
                .__("dice_rolled_broadcast", target.PlayerName, effect.RollNumber+"", effect.PrettyName);

        Log.PrintServerConsole(broadcastRollMessage, LogType.INFO);

        if(localMessage)
        {
            target.LogChat($"You rolled a {{mark}}{effect.RollNumber}{{default}} and got {{mark}}{effect.PrettyName}"
                    .__("dice_rolled_local", effect.RollNumber+"", effect.PrettyName));
        }

        if(broadcastMessage || (broadcastMessageTerrorists && broadcastMessageCounterTerrorists))
        {
            Log.PrintChatAll(broadcastRollMessage, LogType.DEFAULT);
        }
        else if(broadcastMessageTerrorists || broadcastMessageCounterTerrorists)
        {
            CounterStrikeSharp.API.Utilities.GetPlayers().Where(_plyRollCounter => _plyRollCounter.IsValidPly()).ToList().ForEach(_plyController => 
            {
                bool broadcastTerroristPly = _plyController.TeamNum == 2 && broadcastMessageTerrorists;
                bool broadcastCounterTerroristPly = _plyController.TeamNum == 3 && broadcastMessageCounterTerrorists;
                
                if(broadcastTerroristPly || broadcastCounterTerroristPly)
                    _plyController.LogChat(broadcastRollMessage, LogType.DEFAULT);
            });
        }
    }

    public void RollAndApplyEffect(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        EffectBase? effect = GetEffectByRoll()!;

        if(effect == null)
            return;

        var plyID = plyController.SteamID;

        var plyActiveEffect = RollTheDice.Instance!.EffectManager!.PlyActiveEffect;
        if(plyActiveEffect!.ContainsKey(plyID))
        {
            if(_plugin.Config.RemoveLastEffect)
                plyActiveEffect[plyID]!.OnRemove(plyController);

            plyActiveEffect[plyID] = effect;
        }
        else
            plyActiveEffect.Add(plyID, effect);

        BroadOrUnicastRollMessages(plyController, effect);

        if(effect is EffectBaseRegular regularEffect)
            regularEffect.OnApply(plyController);

        if(effect.ShowDescriptionOnRoll)
            plyController.LogChat(effect.GetEffectPrefix() + effect.Description, LogType.DEFAULT);
    }

    #region Hooks
    public HookResult HandlePlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController? plyController = @event.Userid;
        RemoveOrResetPlyDiceCounter(plyController, true);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(!_plugin!.Config!.ResetOnDeath)
            return HookResult.Continue;

        CCSPlayerController? plyController = @event.Userid;
        RemoveOrResetPlyDiceCounter(plyController, false);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(_plugin.Config!.BroadcastPluginCommandInformation)
            Log.PrintChatAll("Enter {blue}!dice{default} in chat to roll the dice!".__("dice_notify_round_start"), LogType.DEFAULT);

        if(!_plugin.Config!.ResetOnRoundStart)
            return HookResult.Continue;

        CounterStrikeSharp.API.Utilities.GetPlayers().ForEach(plyController => 
        {
            RemoveOrResetPlyDiceCounter(plyController, false);
        });


        return HookResult.Continue;
    }

    #endregion

}
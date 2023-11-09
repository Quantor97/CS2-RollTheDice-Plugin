
using System.Collections;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;
internal class DiceSystem 
{
    private RollTheDice _plugin;
    private Dictionary<ulong, bool>? _plyDiceTimer;

    public DiceSystem(RollTheDice plugin)
    {
        _plugin = plugin;
        
        _plyDiceTimer = new Dictionary<ulong, bool>();
    }

    private void RemoveOrResetPlyDiceTimer(CCSPlayerController plyController, bool isRemove)
    {
        if(plyController == null || !plyController.IsValid)
            return;

        ulong plyId = _plugin.GetPlyId(plyController);

        if(!_plyDiceTimer!.ContainsKey(plyId))
            return;

        if(isRemove)
            _plyDiceTimer.Remove(plyId);
        else 
            _plyDiceTimer[plyId] = false;
    }

    public bool CanRoll(CCSPlayerController plyController)
    {
        ulong plySteamId = _plugin.GetPlyId(plyController);

        if(!_plyDiceTimer!.ContainsKey(plySteamId))
        {
            _plyDiceTimer.Add(plySteamId, true);
            return true;
        }

        bool plyHasRolled = _plyDiceTimer[plySteamId];

        if(plyHasRolled)
        {
            PluginFeedback.Chat("You already rolled the dice for this round!",  PluginFeedback.FeedbackType.Warning, plyController);
            return false;
        }

        _plyDiceTimer[plySteamId] = true;
        return true;
    }

    public void RollDice(CCSPlayerController plyController)
    {
        if(plyController == null || !plyController.IsValid)
            return;

/*
        if(!CanRoll(plyController))
            return;
*/

        _plugin.ApplyRandomDiceEffect(plyController);
    }

    #region Hooks

    public HookResult HandlePlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceTimer(plyController, true);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyDiceTimer(plyController, false);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        foreach(CCSPlayerController plyController in Utilities.GetPlayers())
            RemoveOrResetPlyDiceTimer(plyController, false);

        return HookResult.Continue;
    }

    #endregion
}
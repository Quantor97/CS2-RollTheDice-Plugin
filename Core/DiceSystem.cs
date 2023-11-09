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
        if(!plyController.IsValidPly())
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
            plyController.CustomNotify("You already rolled the dice for this round!", FeedbackType.Warning);
            return false;
        }

        _plyDiceTimer[plySteamId] = true;
        return true;
    }

    public void RollDice(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        if(!CanRoll(plyController))
            return;

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
        Utilities.GetPlayers().ForEach(plyController => RemoveOrResetPlyDiceTimer(plyController, false));

        return HookResult.Continue;
    }

    #endregion
}
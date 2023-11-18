using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

public class EffectManager
{
    private RollTheDice _plugin;
    public Dictionary<ulong, EffectBase>? PlyActiveEffect = new();
    public EffectManager(RollTheDice plugin)
    {
        _plugin = plugin;
    }

    public void ResetState()
    {
        PlyActiveEffect?.Clear();
    }

    public void RemoveOrResetPlyActiveEffects(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        var activeEffect = plyController.GetEffect();

        if(activeEffect == null)
            return;

        activeEffect.OnRemove(plyController);
        PlyActiveEffect!.Remove(plyController.SteamID);
    }

    public void RemoveActiveEffect(CCSPlayerController plyController)
    {
        if(!plyController.IsValidPly())
            return;

        var plyID = plyController.SteamID;

        if(!PlyActiveEffect!.ContainsKey(plyID))
            return;

        PlyActiveEffect.Remove(plyID);
    }

    public void AddActiveEffect(CCSPlayerController plyController, EffectBase effect)
    {
        if(!plyController.IsValidPly())
            return;

        var plyID = plyController.SteamID;

        if(PlyActiveEffect!.ContainsKey(plyID))
            return;

        PlyActiveEffect.Add(plyID, effect);
    }

    public EffectBase? GetActiveEffect(CCSPlayerController plyController)
    {
        var plyID = plyController.SteamID;

        if(!PlyActiveEffect!.ContainsKey(plyID))
            return null;

        return PlyActiveEffect[plyID];
    }

    #region Events

    public HookResult HandlePlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController? plyController = @event.Userid;
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController? plyController = @event.Userid;
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        CounterStrikeSharp.API.Utilities.GetPlayers().ForEach(plyController => 
        {
            RemoveOrResetPlyActiveEffects(plyController);
        });

        return HookResult.Continue;
    }

    public HookResult HandlePlayerHurt(EventPlayerHurt @event, GameEventInfo eventInfo)
    {
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        attacker?.GetEventEffect<EventPlayerHurt>()?.OnEvent(@event, eventInfo);
        victim?.GetEventEffect<EventPlayerHurt>()?.OnEvent(@event, eventInfo);

        return HookResult.Continue;
    }

    #endregion
}
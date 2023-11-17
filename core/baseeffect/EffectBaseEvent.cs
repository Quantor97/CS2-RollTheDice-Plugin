
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

public abstract class EffectBaseEvent<TGameEvent> : EffectBase, IEffectGameEventVerbose, IEffectGameEvent<TGameEvent> where TGameEvent : GameEvent
{
    public abstract string MessageOnEvent { get; set; }
    public abstract HookResult OnEvent(TGameEvent @event, GameEventInfo eventInfo);

    public virtual void PrintMessageOnEvent(CCSPlayerController? playerController, string translationKey, params string[] args)
    {
        //playerController!.LogChat(GetEffectPrefix() + MessageOnEvent.__(translationKey, args));
        playerController!.LogCenter(GetEffectPrefix() + MessageOnEvent.__(translationKey, args));
    }
}
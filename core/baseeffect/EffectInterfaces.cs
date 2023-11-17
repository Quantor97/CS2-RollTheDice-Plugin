using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Events;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

public interface IEffect 
{
    void Initialize();
    void OnRemove(CCSPlayerController playerController);
}

public interface IEffectRegular
{
    void OnApply(CCSPlayerController? playerController);
}

public interface IEffectGameEvent<TEvent> where TEvent : GameEvent
{
    HookResult OnEvent(TEvent @event, GameEventInfo eventInfo);
}

public interface IEffectGameEventVerbose
{
    string MessageOnEvent { get; set; }
}

public interface IEffectParamterized
{
    Dictionary<string, string> RawParameters {get; set;}
}

public interface IEffectTimer
{
    void OnTimer(CCSPlayerController playerController);
}

public interface IEffectWorkInProgress
{

}
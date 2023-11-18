using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Timers;

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

public interface IEffectParameter
{
    Dictionary<string, string> RawParameters {get; set;}
}

public interface IEffectTimer
{
    Dictionary<IntPtr, CounterStrikeSharp.API.Modules.Timers.Timer> Timers { get; set; }
    void OnTimerEnd(CCSPlayerController playerController);
}

public interface IEffectWorkInProgress
{

}
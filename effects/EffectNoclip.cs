
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectNoclip : EffectBaseRegular, IEffectParameter, IEffectTimer
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "Noclip".__("effect_name_noclip");
    public override string Description { get; set; } = "You have noclip for {mark}{0}{default} seconds".__("effect_description_noclip");
    public override double Probability { get; set; }  = 1;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters {get; set;} = new();
    public Dictionary<nint, CounterStrikeSharp.API.Modules.Timers.Timer> Timers { get; set; } = new();

    public override void Initialize()
    {
        RawParameters.Add("durationSeconds", "5,0");
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
        if(Timers.ContainsKey(playerController!.Handle))
        {
            playerController.LogChat(GetEffectPrefix() + "You already have this effect");
            return;
        }

        if(!RawParameters.TryGetValue("durationSeconds", out var durationStr))
            return;

        if(!float.TryParse(durationStr, out var durationFl))
            return;

        playerController!.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_NOCLIP;

        var timerRef = Timers;
        StartTimer(ref timerRef, playerController, durationFl);
        PrintDescription(playerController, "effect_description_noclip", durationStr);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
        if(Timers.TryGetValue(playerController!.Handle, out var timer))
        {
            timer.Kill();
            Timers.Remove(playerController!.Handle);
        }
    }

    public void OnTimerEnd(CCSPlayerController playerController)
    {
        if(!playerController!.IsValidPly() || !playerController!.IsAlive())
            return;

        playerController.LogChat(GetEffectPrefix() + "Noclip has ended");
        playerController.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_WALK;
    }
}
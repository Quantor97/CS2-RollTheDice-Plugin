using System.Drawing;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectInvisible : EffectBaseRegular, IEffectParameter, IEffectTimer
{
    public override bool Enabled {get; set; } = true;
    public override string PrettyName {get; set; } = "Invisible".__("effect_name_invisible");
    public override string Description {get; set; } = "You will be invisible for {mark}{0}{default} seconds".__("effect_description_invisible");
    public override double Probability { get; set; }  = 1;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters { get; set; } = new();
    public Dictionary<nint, CounterStrikeSharp.API.Modules.Timers.Timer> Timers {get; set; } = new();

    public override void Initialize()
    {
        RawParameters.Add("durationSeconds", "10,0");
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

        if(!float.TryParse(durationStr, out var durationFloat))
            return;

        playerController!.PlayerPawn.Value.Render = Color.FromArgb(0, 0, 0, 0);
        playerController.RefreshUI();

        var timerRef = Timers;
        StartTimer(ref timerRef, playerController, durationFloat);
        PrintDescription(playerController, "effect_description_invisible", durationStr);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
        if(Timers.TryGetValue(playerController!.Handle, out var timer))
        {
            OnTimerEnd(playerController);

            timer.Kill();
            Timers.Remove(playerController!.Handle);
        }
    }

    public void OnTimerEnd(CCSPlayerController playerController)
    {
        if(!playerController!.IsValidPly())
            return;

        playerController!.PlayerPawn.Value.Render = Color.FromArgb(255,255,255,255);
        playerController.RefreshUI();
        
        playerController.LogChat(GetEffectPrefix() + "You are visible again");
    }
}

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectGodMode : EffectBaseRegular, IEffectParameter, IEffectTimer
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "GodMode".__("effect_name_godmode");
    public override string Description { get; set; } = "Godmode is enabled for {mark}{0}{default} seconds".__("effect_description_godmode");
    public override double Probability { get; set; }  = 3;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters { get; set; } = new();
    public Dictionary<IntPtr, CounterStrikeSharp.API.Modules.Timers.Timer> Timers { get; set; } = new();

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

        if(!float.TryParse(durationStr, out var durationFl))
            return;

        playerController!.PlayerPawn.Value.Health = (int)10e8;
        playerController.RefreshUI();
        PrintDescription(playerController, "effect_description_godmode", durationStr);

        var timerRef = Timers;
        StartTimer(ref timerRef, playerController, durationFl);
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

        var plyHealth = playerController!.PlayerPawn.Value.Health;
        var plyMaxHealth = playerController!.PlayerPawn.Value.MaxHealth;

        if(plyHealth > plyMaxHealth)
            playerController!.PlayerPawn.Value.Health = plyMaxHealth;

        playerController.LogChat(GetEffectPrefix() + "Godmode has ended");
        playerController.RefreshUI();
    }

}
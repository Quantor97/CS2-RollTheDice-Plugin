using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectLessHealth : EffectBaseRegular, IEffectParamterized
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "Less Health".__("effect_name_less_health");
    public override string Description { get; set; } = "Your health is decreased by {mark}{0}".__("effect_description_less_health");
    public override double Probability { get; set; }  = 3;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters { get; set; } = new();

    public override void Initialize()
    {
        RawParameters.Add("healthSubtrahend", "10");
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
        if(!RawParameters.TryGetValue("healthSubtrahend", out var healthStr))
            return;

        if(!int.TryParse(healthStr, out var healthInt))
            return;

        // Health can't be less than 1 otherwise server crashes
        var plyHealth = playerController!.PlayerPawn.Value.Health;
        playerController!.PlayerPawn.Value.Health = Math.Max(plyHealth - healthInt, 1);
        PrintDescription(playerController, "effect_description_less_health", healthStr);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
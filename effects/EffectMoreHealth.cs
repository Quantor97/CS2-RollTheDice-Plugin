using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectMoreHealth : EffectBaseRegular, IEffectParameter
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "More Health".__("effect_name_more_health");
    public override string Description { get; set; } = "Your health is increased by {mark}{0}".__("effect_description_more_health");
    public override double Probability { get; set; }  = 3;
    public Dictionary<string, string> RawParameters {get; set; } = new();
    public override bool ShowDescriptionOnRoll { get; set; } = false;

    public override void Initialize()
    {
        RawParameters.Add("healthSummand", "10");
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
        if(!RawParameters.TryGetValue("healthSummand", out var healthStr))
            return;

        if(!int.TryParse(healthStr, out var healthInt))
            return;

        playerController!.PlayerPawn.Value.Health += healthInt;

        playerController.RefreshUI();
        PrintDescription(playerController, "effect_description_more_health", healthStr);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
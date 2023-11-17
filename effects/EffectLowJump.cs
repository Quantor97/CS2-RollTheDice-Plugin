
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectLowJump : EffectBaseRegular, IEffectWorkInProgress
{
    public override bool Enabled { get; set; } = false;
    public override string PrettyName { get; set; } = "Low Jump".__("effect_name_low_jump");
    public override string Description { get; set; } = "Jump low!".__("effect_description_low_jump");
    public override double Probability { get; set; }  = 3;
    public override bool ShowDescriptionOnRoll { get; set; } = true;

    public override void Initialize()
    {
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
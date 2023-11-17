
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectHighJump : EffectBaseRegular, IEffectWorkInProgress
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "High Jump".__("effect_name_high_jump");
    public override string Description { get; set; } = "You can jump high".__("effect_description_high_jump");
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
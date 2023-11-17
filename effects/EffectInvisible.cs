using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectInvisible : EffectBaseRegular, IEffectParamterized, IEffectWorkInProgress
{
    public override bool Enabled {get; set; } = true;
    public override string PrettyName {get; set; } = "Invisible".__("effect_name_invisible");
    public override string Description {get; set; } = "You will be invisible for {mark}{0}{default} seconds".__("effect_description_invisible");
    public override double Probability { get; set; }  = 1;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters { get; set; } = new();

    public override void Initialize()
    {
        RawParameters.Add("durationSeconds", "10,0");
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
      if(!RawParameters.TryGetValue("durationSeconds", out var durationStr))
          return;

      if(!float.TryParse(durationStr, out var durationFloat))
          return;

      PrintDescription(playerController, "effect_description_invisible", durationStr);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
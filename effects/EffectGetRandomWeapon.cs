using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectGetRandomWeapon : EffectBaseRegular, IEffectParameter
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "Get Random Weapon".__("effect_name_get_random_weapon");
    public override string Description { get; set; } = "You have received a randomly selected weapon: {mark}{0}".__("effect_description_get_random_weapon");
    public override double Probability { get; set; }  = 2;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
    public Dictionary<string, string> RawParameters {get; set; } = new();

    public override void Initialize()
    {
        RawParameters = new Dictionary<string, string>()
        {
            { "p250" , "1"  },
            { "mp9"  , "1"  },
            { "taser", "1" },
            { "nova" , "1"  },
            { "deagle", "1" },
            { "glock", "1" },
            { "usp_silencer", "1" },
            { "hkp2000", "1" },
            { "fiveseven", "1" },
            { "tec9", "1" },
            { "cz75a", "1" },
            { "revolver", "1" },
            { "famas", "1" },
            { "galilar", "1" },
            { "m4a1", "1" },
            { "m4a1_silencer", "1" },
            { "ak47", "1" },
            { "ssg08", "1" },
            { "sg556", "1" },
            { "aug", "1" },
            { "awp", "1" },
            { "g3sg1", "1" },
            { "scar20", "1" },
            { "mac10", "1" },
            { "mp7", "1" },
            { "mp5sd", "1" },
            { "ump45", "1" },
            { "p90", "1" },
            { "bizon", "1" },
            { "mag7", "1" },
            { "negev", "1" },
            { "sawedoff", "1" },
            { "xm1014", "1" },
            { "m249", "1" },
            { "c4", "1" }
        };
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
        if(RawParameters.Count == 0)
            return;

        var randomEntry = RawParameters
            .Where(entry => entry.Value == "1")
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefault();

        if(randomEntry.Key == null)
            return;

        playerController!.GiveNamedItem("weapon_" +randomEntry.Key);
        PrintDescription(playerController, "effect_description_get_random_weapon", randomEntry.Key);
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectLooseRandomWeapon : EffectBaseRegular, IEffectParamterized
{
    public override bool Enabled {get; set; } = true;
    public override string PrettyName {get; set; } = "Loose Random Weapon".__("effect_name_loose_random_weapon");
    public override string Description { get; set; } = "You have been disarmed, loosing a randomly choosen weapon: {mark}{0}".__("effect_description_loose_random_weapon");
    public override double Probability { get; set; }  = 3;
    public override bool ShowDescriptionOnRoll { get; set; } = false;
  public Dictionary<string, string> RawParameters { get; set; } = new();

    public override void Initialize()
    {
    }

    public override void OnApply(CCSPlayerController? playerController)
    {
        var weaponServices = playerController!.PlayerPawn.Value.WeaponServices;

        if(weaponServices == null)
            return;

        var randomWeapon = weaponServices.MyWeapons
            .Where(x => x.Value.DesignerName != "weapon_knife")
            .OrderBy(x => Guid.NewGuid())
            .First();

        if(randomWeapon == null || string.IsNullOrEmpty(randomWeapon!.Value.DesignerName))
        {
            playerController.LogChat(GetEffectPrefix() + "No weapon found to remove");
            return;
        }

        var weaponName = randomWeapon.Value.DesignerName;
        randomWeapon.Value.Remove();
        PrintDescription(playerController, "effect_description_loose_random_weapon", weaponName);

        // Todo: Select knife if activee Weapon is removed
        var knife = weaponServices.MyWeapons.Where(x => x.Value.DesignerName == "weapon_knife").FirstOrDefault();

        if(knife == null)
            return;
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
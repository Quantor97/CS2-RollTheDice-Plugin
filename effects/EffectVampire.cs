using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Effects;

public class EffectVampire : EffectBaseEvent<EventPlayerHurt>, IEffectParameter
{
    public override bool Enabled { get; set; } = true;
    public override string PrettyName { get; set; } = "Vampire".__("effect_name_vampire") ;
    public override string Description { get; set; } = "You will drain health from the player you harm".__("effect_description_vampire") ;
    public override double Probability { get; set; }  = 1;
    public override bool ShowDescriptionOnRoll { get; set; } = true;
    public override string MessageOnEvent { get; set; } = "You drained {mark}{0}{default} health from {mark}{1}".__("effect_event_vampire");
    public Dictionary<string, string> RawParameters { get; set; } = new();

    private bool _allowTeamDamage = false;

    public override void Initialize()
    {
        RawParameters.Add("healthScaleFactor", "0,5");

        var friendlyFire = ConVar.Find("mp_friendlyfire");
        if(friendlyFire is null)
            return;

        _allowTeamDamage = friendlyFire.GetPrimitiveValue<bool>();
    }

    public override HookResult OnEvent(EventPlayerHurt @event, GameEventInfo eventInfo)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        // check if the attacker is a valid player and the victim is a valid player or bot
        if(!attackerController.IsValidPly() || victimController is {IsValid: false, PlayerPawn.IsValid: false})
            return HookResult.Continue;

        // check if the attacker is the victim or if they are on the same team
        if(attackerController == victimController || (!_allowTeamDamage && attackerController.TeamNum == victimController.TeamNum))
            return HookResult.Continue;

        if(!RawParameters.TryGetValue("healthScaleFactor", out string? healthScaledStr))
            return HookResult.Continue;

        string victimName = victimController.PlayerName;
        int damageAmount = Utilities.Helpers.GetDamageInRangePlyHealth(@event, float.Parse(healthScaledStr));

        // Health less than 1 crashes the server
        attackerController.PlayerPawn.Value.Health += damageAmount;
        PrintMessageOnEvent(attackerController, "effect_event_vampire", damageAmount+"", victimName);

        return HookResult.Continue;
    }

    public override void OnRemove(CCSPlayerController? playerController)
    {
    }
}
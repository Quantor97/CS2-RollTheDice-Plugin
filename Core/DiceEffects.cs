using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Timers;

namespace Preach.CS2.Plugins.RollTheDice;
public class DiceEffects 
{
    private RollTheDice _plugin;
    public Dictionary<ulong, Effect>? PlyActiveEffects = new();

    public DiceEffects()
    {
        _plugin = RollTheDice.Instance!;

        CreateDiceEffects();
    }

    public void CreateDiceEffects()
    {
        Effect.Effects?.Clear();
        Effect.EffectCount = 0;
        Effect.TotalCumulativeProbability = 0;

        // Schema:
        // Without hook dependency: new Effect(probability, name, prettyName, effectAction);
        // With hook depedency: new Effect(probability, name, prettyName, effectHookAction, effectHookName);
        // Hooks added so far: PlayerHurt (EventPlayerHurt)

        // Effect: Nothing
        // Purpose: Do nothing
        new Effect(5.0    ,"nothing"             ,"Nothing".__("effect_name_nothing")                   ,EffectNothing)
            .SetDescription("Nothing will happen".__("effect_description_nothing") );


        // Effect: Random Weapon
        // Purpose: Give the player a random weapon
        // Parameters: weaponName
        // Parameter Purpose: Sets the weapon to be given (Randomly)
        new Effect(2.0    ,"random_weapon"       ,"Random Weapon".__("effect_name_random_weapon")       ,EffectRandomWeapon)
            .SetDescription("You got randomly the weapon $(mark){0}".__("effect_description_random_weapon") )
            .SetParameters(new() { 
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
                { "knife", "1" },
                { "bayonet", "1" },
                { "flip", "1" },
                { "gut", "1" },
                { "karambit", "1" },
                { "m9", "1" },
                { "huntsman", "1" },
                { "falchion", "1" },
                { "bowie", "1" },
                { "butterfly", "1" },
                { "daggers", "1" },
                { "ursus", "1" },
                { "navaja", "1" },
                { "stiletto", "1" },
                { "talon", "1" },
                { "classic", "1" },
                { "c4", "1" }
            });

        // Effect: Low Gravity
        // Purpose: Scales the player's gravity
        // Parameters: scaleFactor
        // Parameter Purpose: Scales the player's gravity
        new Effect(3.0    ,"low_gravity"         ,"Low Gravity".__("effect_name_low_gravity")           ,EffectLowGravity)
            .SetDescription("Your gravity is scaled by $(mark){0}".__("effect_description_low_gravity"))
            .SetParameters(new() {
                { "scaleFactor", "0.5" }
            });

        // Effect: High Gravity
        // Purpose: Scales the player's gravity
        // Parameters: scaleFactor
        // Parameter Purpose: Scales the player's gravity
        new Effect(3.0    ,"high_gravity"        ,"High Gravity".__("effect_name_high_gravity")         ,EffectHighGravity)
            .SetDescription("Your gravity is scaled by $(mark){0}".__("effect_description_high_gravity"))
            .SetParameters(new() {
                { "scaleFactor", "1.5" }
            });

        // Effect: More Health
        // Purpose: Adds health to the player
        // Parameters: amount
        // Parameter Purpose: Sets the amount of health to add
        new Effect(2.0    ,"more_health"         ,"More Health".__("effect_name_more_health")           ,EffectMoreHealth)
            .SetDescription("You got $(mark){0}$(default) more health".__("effect_description_more_health"))
            .SetParameters(new() {
                { "amount", "20" }
            });

        // Effect: Less Health
        // Purpose: Removes health from the player
        // Parameters: amount
        // Parameter Purpose: Sets the amount of health to remove
        new Effect(3    ,"less_health"         ,"Less Health".__("effect_name_less_health")           ,EffectLessHealth)
            .SetDescription("You got $(mark){0}$(default) less health".__("effect_description_less_health"))
            .SetParameters(new() { 
                { "amount", "20" }
            });

        // Effect: Increased Speed
        // Purpose: Scales the player's speed
        // Parameters: scaleFactor
        // Parameter Purpose: Scales the player's speed
        new Effect(1.0    ,"increased_speed"     ,"Increased Speed".__("effect_name_increased_speed")   ,EffectIncreaseSpeed)
            .SetDescription("Your speed has been scaled to $(mark){0}".__("effect_description_increased_speed"))
            .SetParameters(new() {
                { "scaleFactor", "3" }
            });

        // Effect: Decreased Speed
        // Purpose: Scales the player's speed
        // Parameters: scaleFactor
        // Parameter Purpose: Scales the player's speed
        new Effect(3.0    ,"decreased_speed"     ,"Decreased Speed".__("effect_name_decreased_speed")   ,EffectDecreaseSpeed)
            .SetDescription("Your speed has been scaled to $(mark){0}".__("effect_description_decreased_speed"))
            .SetParameters(new() {
                { "scaleFactor", "0.5" }
            });

        // Effect: Vampire
        // Purpose: Steals health from the player that was hurt and gives it to Effect owner
        new Effect(2.0   ,"vampire"             ,"Vampire".__("effect_name_vampire")                   ,EffectHookVampire, "PlayerHurt")
            .SetDescription("You will steal health from the player you hurt".__("effect_description_vampire"))
            .SetParameters(new() {
                { "printEvent", "1" },
                { "scaleFactor", "0.5" }
            });

        // Effect: Mirrored Vampire
        // Purpose: Steals health from the Effect owner that was hurt and gives it to Attacker
        // Parameters: scaleFactor
        // Parameter Purpose: Scales the amount of health stolen
        new Effect(2.0    ,"mirrored_vampire"    ,"Mirrored Vampire".__("effect_name_mirrored_vampire") ,EffectHookMirroredVampire, "PlayerHurt")
            .SetDescription("Applying Damages to players will be refelected to you".__("effect_description_mirrored_vampire"))
            .SetParameters(new() {
                { "scaleFactor", "0.5" },
                { "printEvent", "1"}
            });

        // Effet: Invisible
        // Purpose: Makes the player invisible
        // Parameters: duration
        // Parameter Purpose: Sets the duration of the effect
        new Effect(1.0    ,"invisible"           ,"Invisible".__("effect_name_invisible")               ,EffectInvisible)
            .SetDescription("You will be invisible for $(mark){0}$(default) seconds".__("effect_description_invisible"))
            .SetParameters(new() {
                { "duration", "10" }
            });
    }

    #region Effects

    private void EffectNothing(Effect effect, CCSPlayerController plyController)
    {
        Helpers.PrintDescription(effect, plyController);
    }

    private void EffectLowGravity(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("scaleFactor", out string? scaleFactorParam))
            return;
        

        plyController.PlayerPawn.Value.GravityScale *= Helpers.TypeCastParameter<float>(scaleFactorParam);
        Helpers.PrintDescription(effect, plyController, scaleFactorParam);
    }


    private void EffectHighGravity(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("scaleFactor", out string? scaleFactorParam))
            return;

        plyController.PlayerPawn.Value.GravityScale *= Helpers.TypeCastParameter<float>(scaleFactorParam);
        Helpers.PrintDescription(effect, plyController, scaleFactorParam);
    }

    private void EffectMoreHealth(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("amount", out string? healthAmount))
            return;

        plyController.PlayerPawn.Value.Health += Helpers.TypeCastParameter<int>(healthAmount);
        Helpers.PrintDescription(effect, plyController, healthAmount);
    }

    private void EffectLessHealth(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("amount", out string? healthAmount))
            return;

        var plyHealth = plyController.PlayerPawn.Value.Health;
        plyController.PlayerPawn.Value.Health = Math.Max(plyHealth - Helpers.TypeCastParameter<int>(healthAmount), 1);
        Helpers.PrintDescription(effect, plyController, healthAmount);
    }

    private void EffectRandomWeapon(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null)
            return;

        string weaponName = effect.Parameters.Where(x => x.Value != "0").Select(x => x.Key).ToList().ElementAt(new Random().Next(0, effect.Parameters.Count));
        plyController.GiveNamedItem("weapon_" + weaponName);
        Helpers.PrintDescription(effect, plyController, weaponName);
    }
    private void EffectInvisible(Effect effect, CCSPlayerController plyController)
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("duration", out string? durationParam))
            return;

        var duration = Helpers.TypeCastParameter<int>(durationParam);
        if(duration <= 0)
            return;

        plyController.ChangeColor(Color.FromArgb(255, 0, 255, 255));
        var tmpTimer = _plugin.AddTimer(2, () => {
            if(!plyController.IsValidPly())
                return;

            plyController.ChangeColor(Color.FromArgb(255, 255, 255, 255));
        }, TimerFlags.REPEAT);

    }

    private void EffectIncreaseSpeed(Effect effect, CCSPlayerController plyController) 
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("scaleFactor", out string? scaleFactorParam))
            return;

        plyController.PlayerPawn.Value.MovementServices!.Maxspeed *= Helpers.TypeCastParameter<float>(scaleFactorParam);
        Helpers.PrintDescription(effect, plyController, scaleFactorParam);
    }

    private void EffectDecreaseSpeed(Effect effect, CCSPlayerController plyController) 
    {
        if(effect.Parameters == null || !effect.Parameters.TryGetValue("scaleFactor", out string? scaleFactorParam))
            return;

        plyController.PlayerPawn.Value.MovementServices!.Maxspeed *= Helpers.TypeCastParameter<float>(scaleFactorParam);
        Helpers.PrintDescription(effect, plyController, scaleFactorParam);
    }

    private void EffectHookVampire(Effect effect, GameEvent @gameEvent, GameEventInfo info) 
    {
        if(@gameEvent is not EventPlayerHurt @event)
            return;

        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(attackerController == victimController)
            return;

        int damageAmount = Helpers.GetDamageInRangePlyHealth(effect, @event);
        string victimName = victimController.PlayerName;

        attackerController.PlayerPawn.Value.Health += (int) damageAmount;

        if(effect.Parameters == null || !effect.Parameters.TryGetValue("printEvent", out string? printLocalParam) || printLocalParam != "1")
            return;

        attackerController.CustomPrint($"[$(mark2){effect.PrettyName}$(default)] You stole $(mark){damageAmount}$(default) health from $(mark){victimName}"
                .__("vampire_effect", effect.Name, damageAmount+"", victimName));
    }

    private void EffectHookMirroredVampire(Effect effect, GameEvent @gameEvent, GameEventInfo info) 
    {
        if(@gameEvent is not EventPlayerHurt @event)
            return;

        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(attackerController == victimController)
            return;

        int damageAmount = Helpers.GetDamageInRangePlyHealth(effect, @event);
        string victimName = victimController.PlayerName;
        int attackerHealth = attackerController.PlayerPawn.Value.Health;

        // Health less than 1 crashes the server
        attackerController.PlayerPawn.Value.Health = Math.Max( attackerHealth - damageAmount, 1);

        if(effect.Parameters == null || !effect.Parameters.TryGetValue("printEvent", out string? printLocalParam) || printLocalParam != "1")
            return;

        attackerController.CustomPrint($"[$(mark2){effect.PrettyName}$(default)] {victimName} stole $(mark){damageAmount}$(default) health from $(mark)You"
                .__("mirrored_vampire_effect", effect.Name, victimName, damageAmount+""));
    }

    #endregion
}
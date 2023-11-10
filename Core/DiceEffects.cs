using System.Collections;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

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

        new Effect(1.0    ,"nothing"             ,"Nothing".__("effect_name_nothing")                   ,EffectNothing);
        new Effect(1.0    ,"random_weapon"       ,"Random Weapon".__("effect_name_random_weapon")       ,EffectRandomWeapon);
        new Effect(1.0    ,"low_gravity"         ,"Low Gravity".__("effect_name_low_gravity")           ,EffectLowGravity).SetFactor(0.5);
        new Effect(1.0    ,"high_gravity"        ,"High Gravity".__("effect_name_high_gravity")         ,EffectHighGravity).SetFactor(1.5);
        new Effect(1.0    ,"more_health"         ,"More Health".__("effect_name_more_health")           ,EffectMoreHealth).SetFactor(20);
        new Effect(1.0    ,"less_health"         ,"Less Health".__("effect_name_less_health")           ,EffectLessHealth).SetFactor(20);
        new Effect(1.0    ,"increased_speed"     ,"Increased Speed".__("effect_name_increased_speed")   ,EffectIncreaseSpeed);
        new Effect(1.0    ,"decreased_speed"     ,"Decreased Speed".__("effect_name_decreased_speed")   ,EffectDecreaseSpeed);
        new Effect(10.0   ,"vampire"             ,"Vampire".__("effect_name_vampire")                   ,EffectVampire, "PlayerHurt").SetFactor(0.5);
        new Effect(1.0    ,"mirrored_vampire"    ,"Mirrored Vampire".__("effect_name_mirrored_vampire") ,EffectMirroredVampire, "PlayerHurt").SetFactor(0.5);
        new Effect(1.0    ,"invisible"           ,"Invisible".__("effect_name_invisible")               ,EffectInvisible);
    }

    #region Helpers

    private int GetDamageInRangePlyHealth(Effect effect, EventPlayerHurt @event)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        // Count damage that is lower than or equal the victim's health
        float damageAmount = @event.DmgHealth; 
        int victimHealth = victimController.PlayerPawn.Value.Health;
        damageAmount = victimHealth < 0 ? damageAmount+victimHealth : damageAmount;

        return (int) (damageAmount * effect.Parameter);
    }

    #endregion

    #region Effects

    private void EffectNothing(Effect effect, CCSPlayerController plyController)
    {
    }

    private void EffectLowGravity(Effect effect, CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.GravityScale = (float)effect.Parameter;
    }


    private void EffectHighGravity(Effect effect, CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.GravityScale = (float)effect.Parameter;
    }

    private void EffectMoreHealth(Effect effect, CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.Health += (int)effect.Parameter;
    }

    private void EffectLessHealth(Effect effect, CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.Health -= (int)effect.Parameter;
    }

    private void EffectRandomWeapon(Effect effect, CCSPlayerController plyController)
    {
        // Todo
    }
    private void EffectInvisible(Effect effect, CCSPlayerController plyController)
    {
        // Todo
    }

    private void EffectIncreaseSpeed(Effect effect, CCSPlayerController plyController) 
    {
        plyController.PlayerPawn.Value.Speed *= 3f;
    }

    private void EffectDecreaseSpeed(Effect effect, CCSPlayerController plyController) 
    {
        plyController.PlayerPawn.Value.Speed *= .5f;
    }

    private void EffectVampire(Effect effect, GameEvent @gameEvent, GameEventInfo info) 
    {
        if(@gameEvent is not EventPlayerHurt @event)
            return;

        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(attackerController == victimController)
            return;

        int damageAmount = GetDamageInRangePlyHealth(effect, @event);
        string victimName = victimController.PlayerName;

        attackerController.PlayerPawn.Value.Health += (int) damageAmount;
        attackerController.CustomPrint($"[$(mark2){effect.PrettyName}$(default)] You stole $(mark){damageAmount}$(default) health from $(mark){victimName}"
                .__("vampire_effect", effect.Name, damageAmount+"", victimName));
    }

    private void EffectMirroredVampire(Effect effect, GameEvent @gameEvent, GameEventInfo info) 
    {
        if(@gameEvent is not EventPlayerHurt @event)
            return;

        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(attackerController == victimController)
            return;

        int damageAmount = GetDamageInRangePlyHealth(effect, @event);
        string victimName = victimController.PlayerName;
        int attackerHealth = attackerController.PlayerPawn.Value.Health;

        // Health less than 1 crashes the server
        attackerController.PlayerPawn.Value.Health = Math.Max( attackerHealth - damageAmount, 1);
        attackerController.CustomPrint($"[$(mark2){effect.PrettyName}$(default)] {victimName} stole $(mark){damageAmount}$(default) health from $(mark)You"
                .__("mirrored_vampire_effect", effect.Name, victimName, damageAmount+""));
    }

    #endregion
}
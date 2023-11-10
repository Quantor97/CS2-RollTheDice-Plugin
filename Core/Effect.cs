
using System.Collections;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace Preach.CS2.Plugins.RollTheDice;
public class Effect 
{
    // Effects is used to store all effects
    public static List<Effect> Effects = new();

    // Effect count is used to determine the order of effects
    public static int EffectCount = 0;

    // Cumulative probability is used to determine the chance of an effect being rolled
    public static double TotalCumulativeProbability = 0;


    // Enabled is used to determine if the effect is enabled
    public  bool Enabled { get; set; } = true;

    // Name is used to identify the effect
    public string Name { get; set; } = "";

    // PrettyName is used to display the effect name to the player
    public string PrettyName { get; set; } = "";

    // Factor is used to determine the effect factor
    public double Parameter { get; set; } = -1;

    // Roll number is used to determine the order of effects
    public int RollNumber { get; set; }

    // Probability is used to determine the chance of an effect being rolled
    public double Probability { get; set; } = -1;

    // Cumulative probability is used to determine the chance of an effect being rolled
    public double CumulativeProbability { get; set; }

    // Effect action is used to invoke an action when the effect is rolled
    public Action<Effect, CCSPlayerController>? EffectAction { get; set; }

    // Effect hook action is used to invoke an action when the effect is rolled and the hook is called
    public Action<Effect, GameEvent, GameEventInfo>? EffectHookAction { get; set; }

    // Effect hook name is used to identify the hook
    public string? EffectHookName { get; set; }

    public Effect(double probability, string name, string prettyName)
    {
        Effects.Add(this);
        RollNumber = ++EffectCount;
        Probability = Probability < 0 ? probability : Probability;
        TotalCumulativeProbability += Probability;

        CumulativeProbability = TotalCumulativeProbability;
        Name = string.IsNullOrEmpty(Name) ? name : Name;
        PrettyName = string.IsNullOrEmpty(PrettyName) ? prettyName : PrettyName;
    }


    // Effect with action (Invoked when player rolls the dice)
    public Effect(double probability, string name, string prettyName, Action<Effect, CCSPlayerController> effectAction)
        : this(probability, name, prettyName)
    {
        EffectAction = effectAction;
    }

    // Effect with hook action (Invoked when player rolls the dice and hook is called)
    public Effect(double probability, string name, string prettyName, Action<Effect, GameEvent, GameEventInfo> effectHookAction, string effectHookName)
        : this(probability, name, prettyName)
    {
        EffectHookName = effectHookName;
        EffectHookAction = effectHookAction;
    }

    public void SetFactor(double factor)
    {
        Parameter = factor;
    }
}
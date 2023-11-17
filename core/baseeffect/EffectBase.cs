using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
public abstract class EffectBase : IEffect
{
    // Effects is used to store all effects
    public static List<EffectBase> Effects = new();

    // Effect count is used to determine the order of effects
    public static int EffectCount = 0;

    // Cumulative probability is used to determine the chance of an effect being rolled
    public static double TotalCumulativeProbability = 0;

    // Cumulative probability is used to determine the chance of an effect being rolled
    public double CumulativeProbability { get; set;}

    // Roll number is used to determine the order of effects
    public int RollNumber { get; set; }

    // Name is used to identify the effect
    public string Name => GetType().Name;

    // Enabled is used to determine if the effect is enabled
    public abstract bool Enabled { get; set; }

    // ShowDescriptionOnRoll is used to determine if the effect description should be shown to the player on roll
    public abstract bool ShowDescriptionOnRoll { get; set; }

    // PrettyName is used to display the effect name to the player
    public abstract string PrettyName { get; set; }
    
    // Description is used to display the effect description to the player
    public abstract string Description { get; set; }

    // Probability is used to determine the chance of an effect being rolled
    public abstract double Probability { get; set; }

    // RemoveEffect is used to remove the effect from the player
    public abstract void OnRemove(CCSPlayerController? playerController);

    // Initialize is used to initialize the effect
    public abstract void Initialize();

    public EffectBase()
    {
        Effects.Add(this);
        Probability = Probability < 0 ? 0 : Probability;

        if(Enabled)
        {
            RollNumber = ++EffectCount;
            TotalCumulativeProbability += Probability;
            CumulativeProbability = TotalCumulativeProbability;
        }

    }

    public string GetEffectPrefix()
    {
        return $"[{{mark2}}{PrettyName}{{default}}] ";
    }

    public static void ResetEffects()
    {
        Effects?.Clear();
        EffectCount = 0;
        TotalCumulativeProbability = 0;
    }

    public static EffectBase? GetEffect(string name)
    {
        return Effects?.FirstOrDefault(x => x.Name == name);
    }

    public static List<EffectBase>? GetEffects()
    {
        return Effects;
    }
}
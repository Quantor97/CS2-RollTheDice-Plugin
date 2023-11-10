using System.Collections;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;
internal class DiceEffects 
{
    private ArrayList _diceEffects = null!;
    private RollTheDice _plugin;
    private double _maxProbability = 0;
    public Dictionary<ulong, string>? PlyActiveEffects;

    public DiceEffects(RollTheDice plugin)
    {
        _plugin = plugin;
        PlyActiveEffects = new Dictionary<ulong, string>();

        CreateDiceEffects();
    }

    private void MakeCumulativeProbabilities(ref ArrayList effects)
    {
        int counter = 0;
        double comulativeProbability = 0;
        foreach(var effectData in effects)
        {
            double probability = (double)((ArrayList) effectData)[0]!;
            comulativeProbability += probability;

            // Remove the probability from the array and add the comulative probability
            ArrayList data = (ArrayList)effectData;
            data.RemoveAt(0);
            data.Insert(0, comulativeProbability);

            data.Add(++counter);

            _diceEffects.Add(data);
        }

        _maxProbability = comulativeProbability;
    }

    private void CreateDiceEffects()
    {
        _diceEffects = new ArrayList();

        var effects = new ArrayList()
        {
            new ArrayList { 1.0, "Low Gravity",  EffectLowGravity },
            new ArrayList { 1.0, "High Gravity", EffectHighGravity },
            new ArrayList { 1.0, "More Health",  EffectMoreHealth },
            new ArrayList { 1.0, "Less Health",  EffectLessHealth },
            new ArrayList { 1.0, "Increased Speed",  EffectIncreaseSpeed },
            new ArrayList { 1.0, "Decreased Speed",  EffectDecreaseSpeed },
            new ArrayList { 1.0, "Vampire",  EffectVampire },
            new ArrayList { 5.0, "Invisible",  EffectInvisible },
        };

        MakeCumulativeProbabilities(ref effects);
    }

    private ArrayList? GetDiceEffectByRoll(double roll)
    {
        if(_diceEffects == null)
        {
            PluginFeedback.PrintBroadcast("Dice effects are null (Contact Server Owner)", FeedbackType.Error);
            return null;
        }

        foreach(var effect in _diceEffects!)
        {
            double probability = (double)((ArrayList) effect)[0]!;
            string effectName = (string)((ArrayList) effect)[1]!;

            if(roll <= probability)
                return (ArrayList)effect;
        }

        ArrayList effectNothing = new ArrayList { -1, "Nothing", EffectNothing, _diceEffects.Count+1 };
        return effectNothing;
    }

    public void RollAndApplyEffect(CCSPlayerController plyController)
    {
        if(plyController == null || !plyController.IsValid)
            return;

        double roll = Random.Shared.NextDouble() * _maxProbability; 
        ArrayList effectData = GetDiceEffectByRoll(roll)!;

        if(effectData == null)
            return;

        string effectName = (string)effectData[1]!;
        Action<CCSPlayerController> effectAction = (Action<CCSPlayerController>)effectData[2]!;
        int rollNum = (int)effectData[3]!;

        ulong plyId = _plugin.GetPlyId(plyController);
        if(PlyActiveEffects!.ContainsKey(plyId))
            PlyActiveEffects[plyId] = effectName;
        else
            PlyActiveEffects.Add(plyId, effectName);

        plyController.CustomPrint($"You rolled a $(mark){rollNum}$(default) and got $(mark){effectName}");
        effectAction(plyController);
    }

    private void RemoveOrResetPlyActiveEffects(CCSPlayerController plyController)
    {
        if(plyController == null || !plyController.IsValid)
            return;

        ulong plyId = _plugin.GetPlyId(plyController);

        if(!PlyActiveEffects!.ContainsKey(plyId))
            return;

        PlyActiveEffects.Remove(plyId);
    }

    #region Hooks

    public HookResult HandleRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        PlyActiveEffects!.Clear();

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController plyController = @event.Userid;
        RemoveOrResetPlyActiveEffects(plyController);

        return HookResult.Continue;
    }

    public HookResult HandlePlayerHurtVampire(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        if(attackerController == victimController)
            return HookResult.Continue;

        if(PlyActiveEffects == null)
            return HookResult.Continue;

        ulong attackerId = _plugin.GetPlyId(attackerController);

        if(!PlyActiveEffects.ContainsKey(attackerId) || PlyActiveEffects[attackerId] != "Vampire") 
            return HookResult.Continue;

        if(attackerController == null || !attackerController.IsValid)
            return HookResult.Continue;

        string victimName = victimController.PlayerPawn.Value.DesignerName;

        float damageAmount = @event.DmgHealth; 
        int victimHealth = victimController.PlayerPawn.Value.Health;
        damageAmount = victimHealth < 0 ? damageAmount+victimHealth : damageAmount;

        attackerController.PlayerPawn.Value.Health += (int) damageAmount;
        attackerController.CustomPrint($"You stole $(mark){damageAmount}$(default) health from $(mark){victimName}");

        return HookResult.Continue;
    }

    #endregion

    #region Effects

    private void EffectNothing(CCSPlayerController plyController)
    {
        // Todo
    }

    private void EffectLowGravity(CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.GravityScale = .5f;
    }

    private void EffectInvisible(CCSPlayerController plyController)
    {
        // Todo
        plyController.PlayerPawn.Value.RenderMode = RenderMode_t.kRenderNone;
    }

    private void EffectHighGravity(CCSPlayerController plyController)
    {
        plyController.PlayerPawn.Value.GravityScale = 1.5f;
    }

    private void EffectMoreHealth(CCSPlayerController plyController)
    {
        int healthAdd = 20;

        plyController.PlayerPawn.Value.Health += healthAdd;
    }

    private void EffectLessHealth(CCSPlayerController plyController)
    {
        int healthAdd = 20;

        plyController.PlayerPawn.Value.Health -= healthAdd;
    }

    private void EffectRandomWeapon(CCSPlayerController plyController)
    {

    }

    private void EffectIncreaseSpeed(CCSPlayerController plyController) 
    {
        plyController.PlayerPawn.Value.Speed *= 3f;
    }

    private void EffectDecreaseSpeed(CCSPlayerController plyController) 
    {
        plyController.PlayerPawn.Value.Speed *= .5f;
    }

    private void EffectVampire(CCSPlayerController playerController) 
    {
    }

    #endregion
}
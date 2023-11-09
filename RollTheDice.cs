using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace Preach.CS2.Plugins.RollTheDice;
public class RollTheDice : BasePlugin
{
    public override string ModuleName => "Roll The Dice or RTD is a plugin that allows players to roll the dice and get a random effect.";
    public override string ModuleVersion => "0.5.0";
    public override string ModuleAuthor => "Preach";

    private ChatCommands? _chatCommands;
    private DiceSystem? _diceSystem;
    private DiceEffects? _diceEffects;
    private Config? _config;
    public static readonly bool DEBUG = false;

    public override void Load(bool hotReload)
    {
        _config = new Config();
        _config.CheckConfig(ModuleDirectory);

        _chatCommands = new ChatCommands(this);
        _diceSystem = new DiceSystem(this);
        _diceEffects = new DiceEffects(this);
    }

    public ulong GetPlyId(CCSPlayerController plyController)
    {
        return plyController.SteamID;
    }

    public void PlyRollTheDice(CCSPlayerController plyController)
    {
        _diceSystem?.RollDice(plyController);
    }

    public void ApplyDiceEffect(CCSPlayerController plyController, double effect)
    {
        _diceEffects?.ApplyEffect(plyController, effect);
    }

    #region Events

    [GameEventHandler]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        _diceEffects?.HandlePlayerHurtVampire(@event, info);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        _diceEffects?.HandlePlayerDeath(@event, info);
        _diceSystem?.HandlePlayerDeath(@event, info);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        _diceSystem?.HandlePlayerDisconnect(@event, info);
        _diceEffects?.HandlePlayerDisconnect(@event, info);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        _diceSystem?.HandleRoundStart(@event, info);
        _diceEffects?.HandleRoundStart(@event, info);
        return HookResult.Continue;
    }

    #endregion
}
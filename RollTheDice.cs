using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;

namespace Preach.CS2.Plugins.RollTheDice;
public class RollTheDice : BasePlugin
{
    public static readonly bool DEBUG = false;
    public override string ModuleName => "Roll The Dice";
    public override string ModuleDescription => "Roll The Dice or RTD is a plugin that allows players to roll the dice and get a random effect.";
    public override string ModuleVersion => "0.5.0";
    public override string ModuleAuthor => "Preach";

    public static DiceSystem? DiceSystem;
    public static DiceEffects? DiceEffects;
    public static Config? Config;
    public static TranslationConfig? TranslationConfig;
    public static RollTheDice? Instance;

    public override void Load(bool hotReload)
    {
        Instance = this;

        Config = new Config(ModuleDirectory);
        Config.LoadConfig();

        var commands = new Commands();
        DiceSystem = new DiceSystem();
        DiceEffects = new DiceEffects();

        TranslationConfig = new TranslationConfig(ModuleDirectory);
        TranslationConfig.TranslationData = TranslationConfig.LoadConfig()!;

        // DiceEffects have to be loaded after the config is loaded
        Config.GetOrGenerateEffectsConfig();

        // Custom commands in case the actual commands don't work
        // var chatCommands = new ChatCommands(this);
    }

    public void PlyRollTheDice(CCSPlayerController plyController)
    {
        DiceSystem?.PreRollDice(plyController);
    }

    public void ReloadConfig()
    {
        Config!.LoadConfig();
        TranslationConfig!.UpdateLanguage();
        DiceSystem!.ResetState();

        PluginFeedback.WriteConsole("Config reloaded!", FeedbackType.Info);
    }
}
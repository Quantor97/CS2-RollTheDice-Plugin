using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDiceV2.Utilities;
public class Commands 
{
    RollTheDice _plugin;
    public Commands(RollTheDice instance)
    {
        _plugin = instance;
        CreateCommands();
    }

    private void ConfigCallback(CCSPlayerController? plyController, string configType, int args = 0, CommandInfo? info = null)
    {
        var isPlyValid = plyController!.IsValidPly();

        if(args != info?.ArgCount)
        {
            string errMsg = "Please provide a config name!";
            if(isPlyValid)
                plyController!.LogChat(errMsg, LogType.ERROR);
            else 
                Log.PrintServerConsole(errMsg, LogType.ERROR);
            
            return;
        }

        var msg = $"Changing {configType} config...";
        if(isPlyValid)
            plyController!.LogChat(msg, LogType.INFO);
        else 
            Log.PrintServerConsole(msg, LogType.INFO);

        switch(configType)
        {
            case "all":
                _plugin.ReloadConfig();
                break;
            case "effect":
                _plugin.EffectConfig!.UpdateConfig(info.ArgByIndex(1));
                break;
            case "translation":
                _plugin.TranslationConfig!.UpdateConfig(info.ArgByIndex(1));
                break;
        }

        var successMessage = $"{configType} config changed!";
        if(isPlyValid)
            plyController!.LogChat(successMessage, LogType.SUCCSS);
        else 
            Log.PrintServerConsole(successMessage, LogType.SUCCSS);
    }

    private void CreateCommands()
    {
        _plugin.AddCommand("dice", "Roll the dice!", 
            [CommandHelper(0, "Rolling the dice my guy", CommandUsage.CLIENT_ONLY)] (ply, info) => {
                _plugin.DiceSystem!.CheckPlayerStatus(ply!);
            });

        _plugin.AddCommand("rtd_config_all", "Reload the config file!".__("cmd_description_reload_config"), 
            [RequiresPermissions("@css/root")] (ply, info) => {
                ConfigCallback(ply, "all", 1, info);
            });

        _plugin.AddCommand("rtd_config_effect", "Change the effect config!".__("cmd_description_effect_config"), 
            [RequiresPermissions("@css/root")] (ply, info) => {
                ConfigCallback(ply, "effect", 2, info);
            });

        _plugin.AddCommand("rtd_config_language", "Change the translation config!".__("cmd_description_translation_config"), 
            [RequiresPermissions("@css/root")] (ply, info) => {
                ConfigCallback(ply, "translation", 2, info);
            });
    }
}
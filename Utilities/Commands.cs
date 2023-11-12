using System.Collections;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;

namespace Preach.CS2.Plugins.RollTheDice;

internal class Commands 
{
    private RollTheDice _plugin;

    public Commands()
    {
        _plugin = RollTheDice.Instance!;

        AddCommands();
    }

    private void AddCommands()
    {
        // Roll the dice
        _plugin.AddCommand("rtd", "Roll the dice!".__("cmd_description_rtd"), (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });

        // Roll the dice alias
        _plugin.AddCommand("r", "Roll the dice!".__("cmd_description_rtd_alias1"), (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });

        // Roll the dice alias
        _plugin.AddCommand("rollthedice", "Roll the dice!".__("cmd_description_rtd_alias2"), (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });

        // Reload config
        _plugin.AddCommand("rtd_reloadconfig", "Reload the config file!".__("cmd_description_reload_config"), [RequiresPermissions("@css/generic")] (ply, info) => {
            _plugin.ReloadConfig();

            if(ply.IsValidPly())
                ply!.CustomPrint("Config reloaded!", FeedbackType.Chat);
            
            PluginFeedback.WriteConsole("Config reloaded!", FeedbackType.Info);
        });
    }

}
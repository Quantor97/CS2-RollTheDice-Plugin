using System.Collections;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

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

        _plugin.AddCommand("reloadconfig", "Reload the config file!".__("cmd_description_reload_config"), (ply, info) => {
            _plugin.ReloadConfig();
        });
    }
}
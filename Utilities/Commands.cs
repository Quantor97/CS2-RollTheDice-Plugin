using System.Collections;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;

internal class Commands 
{
    private RollTheDice _plugin;

    public Commands(RollTheDice plugin)
    {
        _plugin = plugin;

        AddCommands();
    }

    private void AddCommands()
    {
        // Roll the dice
        _plugin.AddCommand("rtd", "Roll the dice!", (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });

        // Roll the dice alias
        _plugin.AddCommand("r", "Roll the dice!", (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });

        // Roll the dice alias
        _plugin.AddCommand("rollthedice", "Roll the dice!", (ply, info) => {
            if(!ply.IsValidPly())
                return;

            _plugin.PlyRollTheDice(ply!);
        });
    }
}
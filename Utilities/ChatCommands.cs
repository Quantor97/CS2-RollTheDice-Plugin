using System.Collections;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Preach.CS2.Plugins.RollTheDice;

internal class ChatCommands
{
    private Dictionary<string, ArrayList>? _chatCommands;
    private string _chatCommandPrefix = "+";
    private RollTheDice _plugin;

    public ChatCommands(RollTheDice plugin)
    {
        _plugin = plugin;

        CreateChatCommands();
    }

    private void CreateChatCommands()
    {
        _chatCommands = _chatCommands ?? new Dictionary<string, ArrayList>();

        // Create chat commands here
    }

    private void CreateChatCommand(string command, string description, int args, string usage, Action<CCSPlayerController, string[]> action)
    {
        ArrayList commandData = new()  
        {
            description,
            args,
            usage,
            action
        };

        _chatCommands?.Add(command, commandData);
    }

    public HookResult HandleChatCommands(EventPlayerChat @event, GameEventInfo info)
    {
        CCSPlayerController plyController = Utilities.GetPlayerFromIndex(@event.Userid);
        string chatMessage = @event.Text;
        bool isTeam = @event.Teamonly;

        if(!plyController.IsValidPly())
            return HookResult.Continue;

        if(!chatMessage.StartsWith(_chatCommandPrefix))
            return HookResult.Continue;

        string[] chatMessageExploded = chatMessage.Split(" ");
        string command = chatMessageExploded[0].Replace(_chatCommandPrefix, "");
        string[] args = chatMessageExploded.Skip(1).ToArray();

        if(!_chatCommands!.ContainsKey(command))
            return HookResult.Continue;

        ArrayList commandData = _chatCommands[command];
        if(commandData == null)
            return HookResult.Continue;

        string description = (string)commandData[0]!;
        int argsCount = (int)commandData[1]!;
        string usage = (string)commandData[2]!;
        Action<CCSPlayerController, string[]> action = (Action<CCSPlayerController, string[]>)commandData[3]!;

        if(args.Length != argsCount)
        {
            plyController.CustomPrint($"Usage : $(mark){_chatCommandPrefix + command} {usage}");
            return HookResult.Continue;
        }

        action!(plyController, args);

        return HookResult.Continue;
    }
}
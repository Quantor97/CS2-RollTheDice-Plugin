using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins;

namespace Preach.CS2.Plugins.RollTheDice;

internal static class PluginFeedback 
{
    public static readonly string Prefix = "\x01[\x04RollTheDice\x01]";

    public static string GetTypeColor(FeedbackType type)
    {
        return type switch
        {
            FeedbackType.Chat => "\x06",
            FeedbackType.Error => "\x07",
            FeedbackType.Warning => "\x09",
            FeedbackType.Debug => "\x09",
            FeedbackType.Info => "\x09",
            _ => "\x01"
        };
    }

    // Currently not in use
    private static string GetFormatedType(FeedbackType type) 
    {
        var bracketStart = "\x07[";
        var brackerEnd = "\x07]";

        return type switch
        {
            FeedbackType.Error => bracketStart + "\x02 Error " + brackerEnd,
            FeedbackType.Debug => bracketStart + "\x10 Debug " + brackerEnd,
            // FeedbackType.Info => bracketStart + "\x04 Info " + brackerEnd,
            FeedbackType.Info => bracketStart + "\x10 Info " + brackerEnd,
            _ => ""
        };
    }

    private static void SetConsoleColors(FeedbackType? feedbackType)
    {
        var type = feedbackType ?? FeedbackType.Chat;

        switch(type)
        {
            case FeedbackType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                break;
            case FeedbackType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                break;
            case FeedbackType.Debug:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                break;
            case FeedbackType.Info:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }
    }

    private static string GetTypeWithPrefix(FeedbackType type)
    {
        return Prefix + GetFormatedType(type) + " ";
    }

    private static string InterpretColors(string message, FeedbackType feedbackType)
    {
        var typeColor = GetTypeColor(feedbackType);
        return message.Replace("$(mark)", "\x0f").Replace("$(default)", typeColor).Replace("$(mark2)", "\x10");
    }

    private static string GetMessageOutput(bool forConsole, string message, FeedbackType? feedbackType, bool console = false)
    {
        var type = feedbackType ?? FeedbackType.Chat;
        var typeWithPrefix = GetTypeWithPrefix(type);

        if(console)
            return typeWithPrefix + message;

        var typeColor = GetTypeColor(type);
        message = InterpretColors(message, type);

        return typeWithPrefix + typeColor + message;
    }

    public static void PrintChat(CCSPlayerController ply, string message, FeedbackType? feedbackType = FeedbackType.Chat, bool withConsole = false)
    {
        if(!ply.IsValidPly() || feedbackType == FeedbackType.Debug && !RollTheDice.DEBUG)
            return;

        string output = GetMessageOutput(false, message, feedbackType);

        ply.PrintToChat(output);
        ply.PrintToConsole(output);
    }

    public static void PrintBroadcast(string message, FeedbackType? feedbackType = FeedbackType.Chat, bool withConsole = false)
    {
        string output = GetMessageOutput(false, message, feedbackType);

        Server.PrintToChatAll(output);

        if(withConsole)
            WriteConsole(message, feedbackType);
    }

    public static void WriteConsole(string message, FeedbackType? feedbackType = FeedbackType.Chat)
    {
        string output = GetMessageOutput(true, message, feedbackType);

        SetConsoleColors(feedbackType);
        Console.WriteLine(output);
        Console.ResetColor();
    }
}
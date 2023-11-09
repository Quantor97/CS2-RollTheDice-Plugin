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
            _ => "\x07"
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
            _ => ""
        };
    }

    private static string GetTypeWithPrefix(FeedbackType type)
    {
        return Prefix + GetFormatedType(type);
    }

    private static string InterpretColors(string message, FeedbackType feedbackType)
    {
        var typeColor = GetTypeColor(feedbackType);
        return message.Replace("$(mark)", "\x0f").Replace("$(default)", typeColor);
    }

    private static string GetMessageOutput(bool forConsole, string message, FeedbackType? feedbackType)
    {
        var type = feedbackType ?? FeedbackType.Chat;
        var typeColor = GetTypeColor(type);
        var typeWithPrefix = GetTypeWithPrefix(type);
        message = InterpretColors(message, type);

        return forConsole ? typeWithPrefix + " " + message : typeWithPrefix + typeColor + " " + message;
    }

    public static void PrintChat(string message, FeedbackType? feedbackType = FeedbackType.Chat, CCSPlayerController? ply = null, bool withConsole = false)
    {
        if(feedbackType == FeedbackType.Debug && !RollTheDice.DEBUG)
            return;

        string output = GetMessageOutput(false, message, feedbackType);

        if(ply != null)
        {
            ply.PrintToChat(output);

            if(withConsole)
                ply.PrintToConsole(output);
        }
        else
        {
            Server.PrintToChatAll(output);

            if(withConsole)
                Server.PrintToConsole(output);
        }
    }
}
using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins;

namespace Preach.CS2.Plugins.RollTheDice;
internal class PluginFeedback 
{
    public enum FeedbackType
    {
        Chat,
        Console,
        Error,
        Warning,
        Debug
    }

    public static readonly string Prefix = "\x01[\x04RollTheDice\x01]";

    private PluginFeedback()
    {
    }

    public static string GetColor(FeedbackType type)
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
    private static string GetType(FeedbackType type) 
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
        return Prefix + GetType(type);
    }

    private static string InterpretMessage(string message, FeedbackType feedbackType)
    {
        var typeColor = GetColor(feedbackType);
        return message.Replace("$(mark)", "\x0f").Replace("$(default)", typeColor);
    }

    private static string GetOutput(bool forConsole, string message, FeedbackType? feedbackType)
    {
        var type = feedbackType ?? FeedbackType.Chat;
        var typeColor = GetColor(type);
        var typeWithPrefix = GetTypeWithPrefix(type);
        message = InterpretMessage(message, type);

        return forConsole ? typeWithPrefix + " " + message : typeWithPrefix + typeColor + " " + message;
    }

    public static void Chat(string message, FeedbackType? feedbackType, CCSPlayerController? ply)
    {
        if(feedbackType == FeedbackType.Debug && !RollTheDice.DEBUG)
            return;

        string output = GetOutput(false, message, feedbackType);

        if(ply != null)
            ply.PrintToChat(output);
        else
            Server.PrintToChatAll(output);
    }

    public static void Chat(string message, FeedbackType? feedbackType)
    {
        Chat(message, feedbackType, null);
    }

    public static void Chat(string message)
    {
        Chat(message, null, null);
    }

    public static void ChatWithConsole(string message, FeedbackType? feedbackType, CCSPlayerController? ply)
    {
        string output = GetOutput(true, message, feedbackType);

        if(ply != null)
            ply.PrintToConsole(output);
        else 
            Server.PrintToConsole(output);
        
        Chat(message, feedbackType, ply);
    }

    public static void ChatWithConsole(string message, FeedbackType? feedbackType)
    {
        ChatWithConsole(message, feedbackType, null);
    }

    public static void ChatWithConsole(string message)
    {
        ChatWithConsole(message, null, null);
    }

}
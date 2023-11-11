
using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins;

namespace Preach.CS2.Plugins.RollTheDice;
public static class CustomExtensions
{
    public static void CustomPrint(this CCSPlayerController plyController, string message, FeedbackType? feedbackType = FeedbackType.Chat)
    {
        if(!plyController.IsValidPly())
            return;

        PluginFeedback.PrintChat(plyController, message, feedbackType);
    }

    public static ulong GetPlyId(this CCSPlayerController? plyController)
    {
        return plyController?.SteamID ?? 0;
    }

    public static bool IsValidPly(this CCSPlayerController? plyController)
    {
        return plyController != null && plyController.IsValid && !plyController.IsBot;
    }

    public static string __(this string val, string translateKey, params string[] args)
    {
        // Check if TranslationData is valid
        Localization? translationConfig = TranslationConfig.TranslationData;
        if(translationConfig == null)
            return val;

        // Check if Key is valid and return the value
        string? translation = translationConfig.GetTranslation(translateKey, args);
        if(translation == null)
        {
            translationConfig.Data.TryAdd(translateKey, val);
            return val;
        }

        return translation;
    }

}
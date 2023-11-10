
using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins;

namespace Preach.CS2.Plugins.RollTheDice;
public static class CCSPlayerExtensions
{
    public static void CustomPrint(this CCSPlayerController plyController, string message, FeedbackType? feedbackType = FeedbackType.Chat)
    {
        if(!plyController.IsValidPly())
            return;

        PluginFeedback.PrintChat(plyController, message, feedbackType);
    }

    public static bool IsValidPly(this CCSPlayerController? plyController)
    {
        return plyController != null && plyController.IsValid && !plyController.IsBot;
    }
}
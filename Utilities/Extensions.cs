
using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Preach.CS2.Plugins;

namespace Preach.CS2.Plugins.RollTheDice;
public static class CCSPlayerExtensions
{
    public static void CustomNotify(this CCSPlayerController? plyController, string message, FeedbackType? feedbackType = FeedbackType.Chat)
    {
        if(!plyController.IsValidPly())
            return;

        PluginFeedback.PrintChat(message, feedbackType, plyController);
    }

    public static bool IsValidPly(this CCSPlayerController? plyController)
    {
        return plyController != null && plyController.IsValid && !plyController.IsBot;
    }
}
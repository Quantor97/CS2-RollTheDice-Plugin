
using System.Collections;
using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace Preach.CS2.Plugins.RollTheDice;

internal static class Helpers
{
    public static JsonElement? GetJsonElement(Effect? effect, string key)
    {
        var effectsList = Config.ConfigData.Effects;

        if(!effectsList.TryGetValue(effect!.Name, out Dictionary<string, object>? effectData))
            return null;
            
        if(!effectData.TryGetValue(key, out object? value) || !(value is JsonElement valueElement))
        {
            PluginFeedback.WriteConsole($"Failed to get '{key}' for effect '{effect.Name}' from config.json", FeedbackType.Error);
            return null;
        }

        return valueElement;
    }

    public static JsonElement? GetJsonElement(string key)
    {
        var generalConfig = Config.ConfigData.General;
            
        if(!generalConfig.TryGetValue(key, out object? value) || !(value is JsonElement valueElement))
        {
            PluginFeedback.WriteConsole($"Failed to get '{key}' from config.json", FeedbackType.Error);
            return null;
        }

        return valueElement;
    }

    public static bool IntepretStringArguments(string rawText, string[] args, out string result)
    {
        result = rawText;
        if(args == null || args.Length == 0)
            return false;

        for(int i = 0; i < args.Length; i++)
            result = rawText.Replace("{"+i+"}", args[i]);
        
        return true;
    }

}
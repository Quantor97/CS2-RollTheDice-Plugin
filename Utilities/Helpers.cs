
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
    
    public static int GetDamageInRangePlyHealth(Effect effect, EventPlayerHurt @event)
    {
        CCSPlayerController attackerController = @event.Attacker;
        CCSPlayerController victimController = @event.Userid;

        // Count damage that is lower than or equal the victim's health
        float damageAmount = @event.DmgHealth; 
        int victimHealth = victimController.PlayerPawn.Value.Health;
        damageAmount = victimHealth < 0 ? damageAmount+victimHealth : damageAmount;

        if(effect.Parameters == null || !effect.Parameters.TryGetValue("scaleFactor", out string? scaleFactorParam))
            return (int) damageAmount;

        return (int) (damageAmount * float.Parse(scaleFactorParam));
    }

    public static void PrintDescription(Effect effect, CCSPlayerController plyController, params string[] args)
    {
        if(effect.Description == null || !RollTheDice.Config!.GetConfigValue<bool>("PrintEffectDescription"))
            return;
        
        if(Helpers.IntepretStringArguments(effect.Description, args, out string result))
        {
            plyController.CustomPrint(result);
            return;
        }

        plyController.CustomPrint(effect.Description!);
    }

    public static T TypeCastParameter<T>(string param)
    {
        try {
            return (T)Convert.ChangeType(param, typeof(T));
        } catch(Exception e)
        {
            PluginFeedback.WriteConsole($"Parameter {param} is in a wrong format! Check config.json", FeedbackType.Error);
            PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
            PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Error);
        }

        return default!;
    }


}
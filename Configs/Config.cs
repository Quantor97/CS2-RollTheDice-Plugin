using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using CounterStrikeSharp.API.Modules.Utils;

namespace Preach.CS2.Plugins.RollTheDice;

public class Config
{
	public static ConfigData ConfigData = new();
	public static string? ConfigPath;

	public Config(string moduleDirectory)
	{
		ConfigPath = Path.Join(moduleDirectory, "config.json");
	}

	private static void GenerateDefaultEffectsConfig()
	{
		if(Effect.EffectCount == 0)
		{
			PluginFeedback.WriteConsole("No effects found!", FeedbackType.Error);
			return;
		}

		PluginFeedback.WriteConsole("Generating default effects config...", FeedbackType.Info);

		Dictionary<string, Dictionary<string, object>> configEffects = new();
		Effect.Effects.ForEach(effect =>
		{
			Dictionary<string, object> effectData = new()
			{
				{ "Enabled", effect.Enabled },
				{ "Probability", effect.Probability },
				{ "Parameters", effect.Parameters }
			};

			configEffects.Add(effect.Name, effectData);
		});

		ConfigData.Effects = configEffects;
	}

	public static void GenerateDefaultGeneralConfig()
	{
		PluginFeedback.WriteConsole("Generating default general config...", FeedbackType.Info);

		Dictionary<string, object> configGeneral = new() 
		{
			{ "Language", "en" },
			{ "DiceRollsPerRound", 1 },
			{ "BroadcastDiceRoll", true },
			{ "BroadcastPluginCommand", true },
			{ "UnicastDiceRoll", false },
			{ "UnicastEffectDescription", false },
			{ "UnicastRollAmount", false },
			{ "TsCanRoll", true},
			{ "CTsCanRoll", false},
			{ "ResetRollsOnRoundStart", true },
			{ "ResetRollsOnDeath", true },
		};

		ConfigData.General = configGeneral;
	}

	public void LoadConfig()
	{
		PluginFeedback.WriteConsole("Checking config file...", FeedbackType.Info);

		bool fileExists = File.Exists(ConfigPath!);	
		if(!fileExists)
			CreateAndWriteFile(ConfigPath!);

		using (FileStream fs = new FileStream(ConfigPath!, FileMode.Open, FileAccess.Read))
		using (StreamReader sr = new StreamReader(fs))
		{
			ConfigData = JsonSerializer.Deserialize<ConfigData>(sr.ReadToEnd())!;
		}

		SubstituteConfigObjectsForGeneral();
		SubstituteConfigObjectsForEffects();
	}

	private void CreateAndWriteFile(string path)
	{
		PluginFeedback.WriteConsole("Creating config file...", FeedbackType.Info);

		using (FileStream fs = File.Create(path))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

		GenerateDefaultGeneralConfig();

		string jsonConfig = JsonSerializer.Serialize(ConfigData, new JsonSerializerOptions { WriteIndented = true});
		File.WriteAllText(path, jsonConfig);
	}

	public void GetOrGenerateEffectsConfig()
	{
		if(ConfigData.Effects.Count != 0)
		{
			SubstituteConfigObjectsForEffects();
			PluginFeedback.WriteConsole("Config loaded!!", FeedbackType.Info);
			return;
		}

		GenerateDefaultEffectsConfig();

		try {
			string jsonConfig = JsonSerializer.Serialize(ConfigData, new JsonSerializerOptions { WriteIndented = true});
			File.WriteAllText(ConfigPath!, jsonConfig);
		} 
		catch(Exception e)
		{
			PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
			PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Chat);
		}
		
		PluginFeedback.WriteConsole("Config loaded!", FeedbackType.Info);
	}

    public T GetConfigValue<T>(string key)
    {
        try {
            if(ConfigData.General.TryGetValue(key, out var output))
            {
                if(typeof(T).IsAssignableFrom(typeof(int)))
                {
                    return (T)output;
                }
                else if(typeof(T) == typeof(bool))
                {
                    JsonElement value = (JsonElement)output;
                    return (T)((object) value.GetBoolean());
                }
            }
        }
        catch(Exception e)
        {
            PluginFeedback.WriteConsole($"Error while getting config value for {key}", FeedbackType.Error);
            PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
            PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Error);
        }
        
        return default(T)!;
    }

	private void SubstituteConfigObjectsForEffects()
	{
		var effects = Effect.Effects;

		if(effects == null)
			return;

		if(ConfigData.Effects.Count == 0)
			return;

		Effect.TotalCumulativeProbability = 0;

		try 
		{
			foreach(Effect effect in effects)
			{
				var enabledProperty = Helpers.GetJsonElement(effect, "Enabled");
				if(enabledProperty != null)
					effect.Enabled = ((JsonElement)enabledProperty).GetBoolean();

				var parameterProperty = Helpers.GetJsonElement(effect, "Parameters");
				if(parameterProperty != null)
					effect.Parameters = ((JsonElement)parameterProperty).EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString()!);

				var probabilityProperty = Helpers.GetJsonElement(effect, "Probability");
				if(probabilityProperty != null)
					effect.Probability = ((JsonElement)probabilityProperty).GetDouble();

				Effect.TotalCumulativeProbability += effect.Enabled ? effect.Probability : 0;
				effect.CumulativeProbability = Effect.TotalCumulativeProbability;
			} 

		}
		catch(Exception e)
		{
			PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
			PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Chat);
		}
	}

	public void SubstituteConfigObjectsForGeneral()
	{
		var general = ConfigData.General;

		if(general.Count == 0)
			return;

		try 
		{
			var language = Helpers.GetJsonElement("Language");
			if(language != null)
				general["Language"] = ((JsonElement)language).GetString()!;

			var diceRollPerRound = Helpers.GetJsonElement("DiceRollsPerRound");
			if(diceRollPerRound != null)
				general["DiceRollPerRound"] = ((JsonElement)diceRollPerRound).GetInt32();

/*
			var diceRollBroadcast = Helpers.GetJsonElement("DiceRollMessageBroadcast");
			if(diceRollBroadcast != null)
				general["DiceRollMessageBroadcast"] = ((JsonElement)diceRollBroadcast).GetBoolean();

			var diceRollLocal = Helpers.GetJsonElement("DiceRollMessageLocal");
			if(diceRollLocal != null)
				general["DiceRollMessageLocal"] = ((JsonElement)diceRollLocal).GetBoolean();

			var ctsCanRoll = Helpers.GetJsonElement("CTsCanRoll");
			if(ctsCanRoll != null)
				general["CTsCanRoll"] = ((JsonElement)ctsCanRoll).GetBoolean();

			var tsCanRoll = Helpers.GetJsonElement("TsCanRoll");
			if(tsCanRoll != null)
				general["TsCanRoll"] = ((JsonElement)tsCanRoll).GetBoolean();
*/
			
		}
		catch(Exception e)
		{
			PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
			PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Chat);
		}
	}


}

public class ConfigData
{
	public Dictionary<string, object> General { get; set; } = new();
	public Dictionary<string, Dictionary<string, object>> Effects { get; set; } = new();
}
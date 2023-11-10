using System.Collections;
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

	private void SubstituteConfigObjectsForEffects()
	{
		var effects = Effect.Effects;

		if(effects == null)
			return;

		Effect.TotalCumulativeProbability = 0;

		try 
		{
			foreach(Effect effect in effects)
			{
				var enabledProperty = Helpers.GetJsonElement(effect, "Enabled");
				if(enabledProperty != null)
					effect.Enabled = ((JsonElement)enabledProperty).GetBoolean();

				var parameterProperty = Helpers.GetJsonElement(effect, "Parameter");
				if(parameterProperty != null)
					effect.Parameter = ((JsonElement)parameterProperty).GetDouble();

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

	private void SubstituteConfigObjectsForGeneral()
	{
		var general = ConfigData.General;

		try 
		{
			var language = Helpers.GetJsonElement("Language");
			if(language != null)
				general["Language"] = ((JsonElement)language).GetString()!;

			var diceRollPerRound = Helpers.GetJsonElement("DiceRollPerRound");
			if(diceRollPerRound != null)
				general["DiceRollPerRound"] = ((JsonElement)diceRollPerRound).GetInt32();

			var diceRollBroadcast = Helpers.GetJsonElement("DiceRollMessageBroadcast");
			if(diceRollBroadcast != null)
				general["DiceRollMessageBroadcast"] = ((JsonElement)diceRollBroadcast).GetBoolean();

			var diceRollLocal = Helpers.GetJsonElement("DiceRollMessageLocal");
			if(diceRollLocal != null)
				general["DiceRollMessageLocal"] = ((JsonElement)diceRollLocal).GetBoolean();
			
		}
		catch(Exception e)
		{
			PluginFeedback.WriteConsole(e.Message, FeedbackType.Error);
			PluginFeedback.WriteConsole(e.StackTrace!, FeedbackType.Chat);
		}
	}

	private void SubstituteConfigObjects()
	{
		SubstituteConfigObjectsForEffects();
		SubstituteConfigObjectsForGeneral();

		PluginFeedback.WriteConsole("Config loaded!", FeedbackType.Info);
	}

	public void LoadConfig()
	{
		PluginFeedback.WriteConsole("Loading config...", FeedbackType.Info);
		bool fileExists = File.Exists(ConfigPath!);	
		if(!fileExists)
			CreateAndWriteFile(ConfigPath!);

		if(fileExists)
		{
			using (FileStream fs = new FileStream(ConfigPath!, FileMode.Open, FileAccess.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
				ConfigData = JsonSerializer.Deserialize<ConfigData>(sr.ReadToEnd())!;
			}
		}

		if(fileExists)
			SubstituteConfigObjects();
	}

	private static void CreateAndWriteFile(string path)
	{
		PluginFeedback.WriteConsole("Creating config...", FeedbackType.Info);
		using (FileStream fs = File.Create(path))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

		GetDiceEffects();
		GetDefaultGeneralConfig();

		string jsonConfig = JsonSerializer.Serialize(ConfigData, new JsonSerializerOptions { WriteIndented = true});
		File.WriteAllText(path, jsonConfig);
	}

	private static void GetDiceEffects()
	{
		if(Effect.EffectCount == 0)
		{
			PluginFeedback.WriteConsole("No effects found!", FeedbackType.Error);
			return;
		}

		Dictionary<string, Dictionary<string, object>> configEffects = new();
		Effect.Effects.ForEach(effect =>
		{
			Dictionary<string, object> effectData = new()
			{
				{ "Enabled", effect.Enabled },
				{ "Parameter", effect.Parameter },
				{ "Probability", effect.Probability }
			};

			configEffects.Add(effect.Name, effectData);
		});

		ConfigData.Effects = configEffects;
	}

	public static void GetDefaultGeneralConfig()
	{
		Dictionary<string, object> configGeneral = new() 
		{
			{ "Language", "en" },
			{ "DiceRollPerRound", 1 },
			{ "DiceRollMessageBroadcast", true },
			{ "DiceRollMessageLocal", false },
		};

		ConfigData.General = configGeneral;
	}

}

public class ConfigData
{
	public Dictionary<string, object> General { get; set; } = new();
	public Dictionary<string, Dictionary<string, object>> Effects { get; set; } = new();
}
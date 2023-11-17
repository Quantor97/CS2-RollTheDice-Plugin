using System.Text.Json;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;

namespace Preach.CS2.Plugins.RollTheDiceV2.Config;

public class EffectConfig
{
	private RollTheDice _plugin;
	public EffectConfigData Data = new();
	public string? ConfigPath;
	private string _moduleDirectory;
	private string _currentConfig = "default";

	public EffectConfig(RollTheDice plugin)
	{
		_plugin = plugin;
		
		var effectConfig =_plugin.Config.EffectConfig ?? "default";
		_moduleDirectory = Path.Combine(_plugin.ModuleDirectory, "../../configs/plugins/RollTheDice");
		ConfigPath = Path.Join(_moduleDirectory, $"effects/{effectConfig}.json");

		LoadConfig();
	}

    public void UpdateConfig(string configName = "")
    {
        Log.PrintServerConsole("EffectConfig: Checking config file...", LogType.INFO);

        var previousConfig = _currentConfig;
        _currentConfig = string.IsNullOrEmpty(configName) ? _plugin.Config.EffectConfig ?? "default" : configName;

        if(previousConfig != _currentConfig)
            Log.PrintServerConsole($"EffectConfig: Config changed from {previousConfig} to {_currentConfig}", LogType.INFO);

        ConfigPath = Path.Join(_moduleDirectory, $"effects/{_currentConfig}.json");
        LoadConfig();
    }

	public string GetEffectConfigPath(string effectName)
	{
		return Path.Join(Path.GetDirectoryName(ConfigPath)!, $"{effectName}.json");
	}
 
	private void CreateAndWriteFile()
	{
		Log.PrintServerConsole("EffectConfig: Creating config file...", LogType.INFO);

        string directory = Path.GetDirectoryName(ConfigPath)!;

        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

		using (FileStream fs = File.Create(ConfigPath!))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

		string jsonConfig = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true});
		File.WriteAllText(ConfigPath!, jsonConfig);
	}

	public void LoadConfig()
	{
		Log.PrintServerConsole("EffectConfig: Checking config file...", LogType.INFO);

		bool fileExists = File.Exists(ConfigPath!);	
		if(!fileExists)
			CreateAndWriteFile();

		using (FileStream fs = new FileStream(ConfigPath!, FileMode.Open, FileAccess.Read))
		using (StreamReader sr = new StreamReader(fs))
		{
			Data = JsonSerializer.Deserialize<EffectConfigData>(sr.ReadToEnd())!;
		}

		SubstituteConfigObjectsForEffects();

		Log.PrintServerConsole("EffectConfig: config loaded!", LogType.INFO);
	}

	private void GenerateDefaultEffectsConfig()
	{
		if(EffectBase.EffectCount  == 0)
		{
			Log.PrintServerConsole("EffectConfig: No effects found!", LogType.ERROR);
			return;
		}

		Log.PrintServerConsole("EffectConfig: Generating default config...", LogType.INFO);

		Dictionary<string, Dictionary<string, object>> configEffects = new();
		EffectBase.Effects.ForEach(effect =>
		{
			Dictionary<string, object> effectData = new()
			{
				{ "Enabled", effect.Enabled },
				{ "Probability", effect.Probability },
			};

			if(effect is IEffectParamterized effectParameterized)
			{
				effectData.Add("Parameters", effectParameterized.RawParameters);
			}

			configEffects.Add(effect.Name, effectData);
		});

		Data.Effects = configEffects;
		Log.PrintServerConsole("EffectConfig: Default config generated!", LogType.INFO);
	}

	public void GetOrGenerateEffectsConfig()
	{
		if(Data.Effects.Count != 0)
		{
			SubstituteConfigObjectsForEffects();
			return;
		}

		Log.PrintServerConsole("EffectConfig: No config found, generating default config...", LogType.INFO);

		GenerateDefaultEffectsConfig();

		try {
			string jsonConfig = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true});
			File.WriteAllText(ConfigPath!, jsonConfig);
		} 
		catch(Exception e)
		{
			Log.PrintServerConsole(e.Message, LogType.ERROR);
			Log.PrintServerConsole(e.StackTrace!, LogType.DEFAULT);
		}
		
		Log.PrintServerConsole("EffectConfig: Config loaded!", LogType.INFO);
	}

	private void SubstituteConfigObjectsForEffects()
	{
		var effects = EffectBase.Effects;

		if(effects == null || Data.Effects.Count == 0)
			return;

		EffectBase.TotalCumulativeProbability = 0;

		try 
		{
			foreach(EffectBase effect in effects)
			{
				var enabledProperty = Helpers.GetJsonElement(effect, "Enabled");
				if(enabledProperty != null)
					effect.Enabled = ((JsonElement)enabledProperty).GetBoolean();

				var probabilityProperty = Helpers.GetJsonElement(effect, "Probability");
				if(probabilityProperty != null)
					effect.Probability = ((JsonElement)probabilityProperty).GetDouble();

				if(effect is IEffectParamterized effectParameterized)
				{
					var parameterProperty = Helpers.GetJsonElement(effect, "Parameters");
					if(parameterProperty != null)
						effectParameterized.RawParameters = ((JsonElement)parameterProperty).EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString()!);
				}

				// to get the name translated
				var effectName = effect.PrettyName;

				EffectBase.TotalCumulativeProbability += effect.Enabled ? effect.Probability : 0;
				effect.CumulativeProbability = EffectBase.TotalCumulativeProbability;
			} 

		}
		catch(Exception e)
		{
			Log.PrintServerConsole(e.Message, LogType.ERROR);
			Log.PrintServerConsole(e.StackTrace!, LogType.DEFAULT);
		}
	}
}

public class EffectConfigData
{
	public Dictionary<string, Dictionary<string, object>> Effects { get; set; } = new();
}
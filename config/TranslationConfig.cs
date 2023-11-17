
using System.Text.Json;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2;

public class TranslationConfig
{
    private RollTheDice _plugin;
	public  TranslationConfigData TranslationData = new();
	public  string? ConfigPath;
    private string _moduleDirectory = "";
    private string _currentLanguage = "en";

	public TranslationConfig(RollTheDice plugin)
	{
		_plugin = plugin;
        _moduleDirectory = Path.Combine(_plugin.ModuleDirectory, "../../configs/plugins/RollTheDice");
        ConfigPath = Path.Join(_moduleDirectory, $"translations/{_currentLanguage}.json");
	}

    public int UpdateConfig(string configName = "")
    {
        Log.PrintServerConsole("TranslationConfig: Checking config file...", LogType.INFO);

        var previousLanguage = _currentLanguage;
        _currentLanguage = string.IsNullOrEmpty(configName) ? _plugin.Config.Language ?? "en" : configName;

        if(previousLanguage != _currentLanguage)
            Log.PrintServerConsole($"TranslationConfig: Language changed from {previousLanguage} to {_currentLanguage}", LogType.INFO);

        ConfigPath = Path.Join(_moduleDirectory, $"translations/{_currentLanguage}.json");
        LoadConfig();

        return TranslationData.Data.Count;
    }

	public void LoadConfig()
	{
		if(!File.Exists(ConfigPath))
		{
			CreateAndWriteFile(true);
			return;
		}

		using (FileStream fs = new FileStream(ConfigPath!, FileMode.Open, FileAccess.Read))
		using (StreamReader sr = new StreamReader(fs))
		{
			TranslationData.Data = JsonSerializer.Deserialize<Dictionary<string, string>>(sr.ReadToEnd())!;
		}

        Log.PrintServerConsole("TranslationConfig: config loaded!", LogType.INFO);
	}

    public void CreateDefaultTranslation()
    {
        Log.PrintServerConsole("TranslationConfig: Generating default config for en...", LogType.INFO);

        Dictionary<string, string> configTranslation = new() 
        {
            { "dice_already_rolled", "You cannot roll the dice anymore for this round!" },
            { "dice_rolled_local", "You rolled {mark}{0}{default} and got {mark}{1}{default}!" },
            { "dice_rolled_broadcast", "{mark}{0}{default} rolled a {mark}{1}{default} and got {mark}{2}{default}!" },
            { "dice_rolls_left" , "You have {mark}{0}{default} rolls left for this round!"},
            { "dice_cant_roll_dead" , "You cannot roll the dice while dead!"},
            { "dice_wrong_team" , "You can not roll as a {mark}{0}"},
            { "dice_notify_round_start" , "Enter {mark}!dice{default} in chat to roll the dice!"}
        };

        TranslationData.Data = TranslationData.Data
            .Concat(configTranslation)
            .ToLookup(pair => pair.Key, pair => pair.Value)
            .ToDictionary(group => group.Key, group => group.First());
    }

    public void GetOrGenerateTranslationForEffects(int amount)
    {
        if(TranslationData.Data == null)
            return;

        if(TranslationData.Data.Count == amount)
            return;

        Log.PrintServerConsole($"TranslationConfig: Unmatched translation count. Updating file for {_currentLanguage} ...", LogType.INFO);
        CreateAndWriteFile(false);
    }

	private void CreateAndWriteFile(bool writeEmpty = false)
	{
        Log.PrintServerConsole($"TranslationConfig: Creating/Updating file for {_currentLanguage} ...", LogType.INFO);

        string directory = Path.GetDirectoryName(ConfigPath)!;

        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

		using (FileStream fs = File.Create(ConfigPath!))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

        CreateDefaultTranslation();

		string jsonConfig = JsonSerializer.Serialize(TranslationData.Data, new JsonSerializerOptions { WriteIndented = true});
		File.WriteAllText(ConfigPath!, jsonConfig);

        Log.PrintServerConsole($"TranslationConfig: File for {_currentLanguage} created/updated!", LogType.INFO);
	}
}

public class TranslationConfigData
{
    public Dictionary<string, string> Data = new();

    public string InterpretStringArgs(string str, string[] args)
    {
        string output = str;

        for(int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var argEffectName = "effect_name_"+arg;
            if(Data.ContainsKey(argEffectName))
                args[i] = Data[argEffectName]!;

            output = output.Replace("{"+i+"}", args[i]);
        }

        return output;
    }

    public string? GetTranslation(string key, string[] args)
    {
        if(!Data.ContainsKey(key))
            return null;

        string result = Data[key]!;

        if(args.Length != 0)
            result = InterpretStringArgs(Data[key], args);

        return result;
    }

}
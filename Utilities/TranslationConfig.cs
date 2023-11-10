
using System.Collections;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;

namespace Preach.CS2.Plugins.RollTheDice;

public class TranslationConfig
{
	public static Localization TranslationData = new();
	public static string? ConfigPath;
    private string _moduleDirectory = "";
    private string _currentLanguage = "en";

	public TranslationConfig(string moduleDirectory)
	{
        _moduleDirectory = moduleDirectory;
		ConfigPath = Path.Join(moduleDirectory, $"translations/{_currentLanguage}.json");
        UpdateLanguage();
	}

    public void UpdateLanguage()
    {
        var previousLanguage = _currentLanguage;
        _currentLanguage = Config.ConfigData.General["Language"] as string ?? _currentLanguage;

        if(previousLanguage == _currentLanguage)
            return;

        PluginFeedback.WriteConsole($"Language changed from {previousLanguage} to {_currentLanguage}", FeedbackType.Info);
        ConfigPath = Path.Join(_moduleDirectory, $"translations/{_currentLanguage}.json");
        LoadConfig();
    }

	public Localization? LoadConfig()
	{
		if(!File.Exists(ConfigPath))
		{
			CreateAndWriteFile(ConfigPath!);
			return null;
		}

		using (FileStream fs = new FileStream(ConfigPath!, FileMode.Open, FileAccess.Read))
		using (StreamReader sr = new StreamReader(fs))
		{
			TranslationData.Data = JsonSerializer.Deserialize<Dictionary<string, string>>(sr.ReadToEnd())!;
		}

        return TranslationData;
	}

    public void CreateDefaultTranslation()
    {
        Dictionary<string, string> configTranslation = new() 
        {
            { "cmd_description_rtd", "Roll the dice!" },
            { "cmd_description_rtd_alias1", "Roll the dice!" },
            { "cmd_description_rtd_alias2", "Roll the dice!" },
            { "cmd_description_reload_config", "Reload the config file!" },
            { "effect_name_nothing", "Nothing" },
            { "effect_name_random_weapon", "Random Weapon" },
            { "effect_name_low_gravity", "Low Gravity" },
            { "effect_name_high_gravity", "High Gravity" },
            { "effect_name_more_health", "More Health" },
            { "effect_name_less_health", "Less Health" },
            { "effect_name_increased_speed", "Increased Speed" },
            { "effect_name_decreased_speed", "Decreased Speed" },
            { "effect_name_vampire", "Vampire" },
            { "effect_name_mirrored_vampire", "Mirrored Vampire" },
            { "effect_name_invisible", "Invisible" },
            { "vampire_effect", "[$(mark){0}$(default)] You stole $(mark){1}$(default) health from $(mark){2}" },
            { "mirrored_vampire_effect", "[$(mark){0}$(default)] $(mark){1}$(default) stole $(mark){2}$(default) health from you" },
            { "dice_already_rolled", "You cannot roll the dice anymore for this round!" },
            { "dice_rolled_local", "You rolled $(mark){0}$(default) and got $(mark){1}$(default)!" },
            { "dice_rolled_broadcast", "$(mark){0}$(default) rolled a $(mark){1}$(default) and got $(mark){2}$(default)!" },
            { "dice_rolls_left" , "You have $(mark){0}$(default) rolls left for this round!"}
        };

        TranslationData.Data = configTranslation;
    }

	private void CreateAndWriteFile(string path)
	{
        string directory = Path.GetDirectoryName(path)!;

        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

		using (FileStream fs = File.Create(path))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

        if(_currentLanguage == "en")
            CreateDefaultTranslation();

		string jsonConfig = JsonSerializer.Serialize(TranslationData.Data, new JsonSerializerOptions { WriteIndented = true});
		File.WriteAllText(path, jsonConfig);
	}
}

public class Localization
{
    public Dictionary<string, string> Data = new();

    private void InterpretString(string str, string[] args, out string result)
    {
        result = str;

        for(int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var argEffectName = "effect_name_"+arg;
            if(Data.ContainsKey(argEffectName))
                args[i] = Data[argEffectName]!;

            result = result.Replace("{"+i+"}", args[i]);
        }
    }
    public string? GetTranslation(string key, string[] args)
    {
        if(!Data.ContainsKey(key))
            return null;

        InterpretString(Data[key], args, out string result);
        return result;
    }

}
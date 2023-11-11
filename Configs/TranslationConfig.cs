
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

        if(previousLanguage != _currentLanguage)
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
        PluginFeedback.WriteConsole("Generating default translation for en...", FeedbackType.Info);

        Dictionary<string, string> configTranslation = new() 
        {
            { "cmd_description_rtd", "Roll the dice!" },
            { "cmd_description_rtd_alias1", "Roll the dice!" },
            { "cmd_description_rtd_alias2", "Roll the dice!" },
            { "cmd_description_reload_config", "Reload the config file!" },
            { "effect_name_nothing", "Nothing" },
            { "effect_description_nothing", "Nothing will happen" },
            { "effect_name_random_weapon", "Random Weapon" },
            { "effect_description_random_weapon", "You have received a random weapon: $(mark){0}" },
            { "effect_name_low_gravity", "Low Gravity" },
            { "effect_description_low_gravity", "Your gravity is now scaled to $(mark){0}" },
            { "effect_name_high_gravity", "High Gravity" },
            { "effect_description_high_gravity", "Your gravity is now scaled to $(mark){0}" },
            { "effect_name_more_health", "More Health" },
            { "effect_description_more_health", "You have gained $(mark){0}$(default) more health" },
            { "effect_name_less_health", "Less Health" },
            { "effect_description_less_health", "You have lost $(mark){0}$(default) health" },
            { "effect_name_increased_speed", "Increased Speed" },
            { "effect_description_increased_speed", "Your speed has been scaled to $(mark){0}" },
            { "effect_name_decreased_speed", "Decreased Speed" },
            { "effect_description_decreased_speed", "Your speed has been scaled to $(mark){0}" },
            { "effect_name_vampire", "Vampire" },
            { "effect_description_vampire", "You will steal health from the player you damage"},
            { "effect_name_mirrored_vampire", "Mirrored Vampire" },
            { "effect_description_mirrored_vampire", "Damage applied to players will be refelected back to you"},
            { "effect_name_invisible", "Invisible" },
            { "effect_description_invisible", "You will be invisible" },
            { "vampire_effect", "[$(mark){0}$(default)] You stole $(mark){1}$(default) health from $(mark){2}" },
            { "mirrored_vampire_effect", "[$(mark){0}$(default)] $(mark){1}$(default) stole $(mark){2}$(default) health from you" },
            { "dice_already_rolled", "You cannot roll the dice anymore for this round!" },
            { "dice_rolled_local", "You rolled $(mark){0}$(default) and got $(mark){1}$(default)!" },
            { "dice_rolled_broadcast", "$(mark){0}$(default) rolled a $(mark){1}$(default) and got $(mark){2}$(default)!" },
            { "dice_rolls_left" , "You have $(mark){0}$(default) rolls left for this round!"},
            { "dice_cant_roll_dead" , "You cannot roll the dice while dead!"},
            { "dice_wrong_team" , "You can not roll as a $(mark){0}"},
            { "dice_notify_round_start" , "Enter $(mark)!rtd$(default) in chat to roll the dice!"}
        };

        TranslationData.Data = configTranslation;
    }

	private void CreateAndWriteFile(string path)
	{
        PluginFeedback.WriteConsole($"Creating translation file for {_currentLanguage} ...", FeedbackType.Info);

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

    public void InterpretStringArgs(string str, string[] args, out string result)
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

        InterpretStringArgs(Data[key], args, out string result);
        return result;
    }

}
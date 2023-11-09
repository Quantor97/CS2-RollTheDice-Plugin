using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;

namespace Preach.CS2.Plugins.RollTheDice;

internal class Config
{
	public static ConfigData ConfigData = new();

	public void CheckConfig(string moduleDirectory)
	{
		string path = Path.Join(moduleDirectory, "config.json");

		if (!File.Exists(path))
			CreateAndWriteFile(path);

		using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
		using (StreamReader sr = new StreamReader(fs))
		{
			// Deserialize the JSON from the file and load the configuration.
			ConfigData = JsonSerializer.Deserialize<ConfigData>(sr.ReadToEnd())!;
		}

		if (ConfigData != null && ConfigData.ChatPrefix != null)
			ConfigData.ChatPrefix = ModifiedChatPrefix(ConfigData.ChatPrefix);
	}

	private static void CreateAndWriteFile(string path)
	{
		using (FileStream fs = File.Create(path))
		{
			// File is created, and fs will automatically be disposed when the using block exits.
		}

		Console.WriteLine($"File created: {File.Exists(path)}");

		ConfigData config = new ConfigData
		{
			ChatPrefix = "{Green}[DamageInfo]",
			CenterPrint = true,
			RoundEndPrint = true,
			FFAMode = false,
		};

		// Serialize the config object to JSON and write it to the file.
		string jsonConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions()
		{
			WriteIndented = true
		});

		File.WriteAllText(path, jsonConfig);
	}

	// Essential method for replacing chat colors from the config file, the method can be used for other things as well.
	private string ModifiedChatPrefix(string msg)
	{
		if (msg.Contains("{"))
		{
			string modifiedValue = msg;
			foreach (FieldInfo field in typeof(ChatColors).GetFields())
			{
				string pattern = $"{{{field.Name}}}";
				if (msg.Contains(pattern, StringComparison.OrdinalIgnoreCase))
				{
					modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)!.ToString());
				}
			}
			return modifiedValue;
		}

		return string.IsNullOrEmpty(msg) ? "[DamageInfo]" : msg;
	}
}

internal class ConfigData
{
    public string? ChatPrefix { get; set; }
    public bool CenterPrint { get; set; }
    public bool RoundEndPrint { get; set; }
    public bool FFAMode { get; set; }
}
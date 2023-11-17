using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Config;
using Preach.CS2.Plugins.RollTheDiceV2.Core;
using Preach.CS2.Plugins.RollTheDiceV2.Core.BaseEffect;
using Preach.CS2.Plugins.RollTheDiceV2.Utilities;

namespace Preach.CS2.Plugins.RollTheDiceV2.Config;

public class GeneralConfig : BasePluginConfig
{
	[JsonPropertyName("Language")]
	public string Language {get; set;} = "en";

	[JsonPropertyName("Effect config file")]
	public string EffectConfig {get; set;} = "default";

	[JsonPropertyName("Rolls per round")]
	public int RollsPerRound {get; set;} = 1;

	[JsonPropertyName("Remove last effect on roll")]
	public bool RemoveLastEffect {get; set;} = false;

	[JsonPropertyName("Ts can roll")]
	public bool TsCanRoll {get; set;} = true;

	[JsonPropertyName("CTs can roll")]
	public bool CTsCanRoll {get; set;} = false;

	[JsonPropertyName("Reset effects and roll count for all players at round start")]
	public bool ResetOnRoundStart {get; set;} = true;

	[JsonPropertyName("Reset effects and roll for LocalPlayer at death")]
	public bool ResetOnDeath {get; set;} = false;

	[JsonPropertyName("Print roll message for all teams")]
	public bool BroadcastOnRollMessage {get; set;} = true;

	[JsonPropertyName("Print roll message for Ts")]
	public bool BroadcastOnRollMessageTerrorists {get; set;} = true;

	[JsonPropertyName("Print roll message for CTs")]
	public bool BroadcastOnRollMessageCounterTerrorists {get; set;} = false;

	[JsonPropertyName("Print plugin command information")]
	public bool BroadcastPluginCommandInformation {get; set;} = true;

	[JsonPropertyName("Print roll message for LocalPlayer")]
	public bool UnicastOnRollMessage {get; set;} = false;

	[JsonPropertyName("Print effect description for LocalPlayer")]
	public bool UnicastEffectDescription {get; set;} = true;

	[JsonPropertyName("Print effect informations for LocalPlayer")]
	public bool UnicastEffectInformationsOnEvent {get; set;} = true;

	[JsonPropertyName("Print roll amount for LocalPlayer")]
	public bool UnicastRollAmount {get; set;} = true;

}
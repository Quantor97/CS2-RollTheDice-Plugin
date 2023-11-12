<div align="center">

<div align="center">
  <img src="https://i.imgur.com/S8dB2wg.png" alt="CS2-RollTheDice-Plugin Banner" height=350px" width="350px">
</div>

# CS2-RollTheDice-Plugin

![GitHub release (latest by date)](https://img.shields.io/github/v/release/Quantor97/CS2-RollTheDice-Plugin)
![GitHub issues](https://img.shields.io/github/issues/Quantor97/CS2-RollTheDice-Plugin)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://raw.githubusercontent.com/Quantor97/CS2-RollTheDice-Plugin/main/LICENSE.md)


With this plugin, players have the exciting opportunity to roll the dice, triggering unique and unpredictable effects.
Originally designed for the JailBreak GameMode, it seamlessly integrates into any other GameMode, offering versatility in its usage.

</div>

## 🌟 Features
- **Varied Effects**: Each dice roll grants the player a distinct effect, ranging from power-ups to challenges, 
- **Dice Rolling Mechanism**: Players can roll the dice, adding an element of chance and excitement to the gameplay.
- **Localization**: Make the plugin accessible to your local audience by translating it into your language.
- **Customizable Effects**: Configure the likelihood of each effect occurring to fine-tune the gaming experience.
- **Configurability:** Enjoy the flexibility to fine-tune and configure various aspects of the plugin to suit your server's preferences.

## 🎲 Roll Effects

Players can roll the dice for a chance to get:

| Effect              | Description                                                                          | Functional    | Note |
|---------------------|--------------------------------------------------------------------------------------|---------------|--------|
| Nothing             | Nothing                                                                              | ✔️           |     |
| Random Weapon       | Give the player a random weapon                                                      | ✔️           |     |
| Low Gravity         | Scale the player's gravity                                                           | ✔️           |     |
| High Gravity        | Scale the player's gravity                                                           | ✔️           |     |
| More Health         | Add health to the player                                                             | ✔️           | Health status isn't visual updating  |
| Less Health         | Remove health from the player                                                        | ✔️           | Health status isn't visual updating   |
| Increased Speed     | Scale the player's speed                                                             | ❌           | |
| Decreased Speed     | Scale the player's speed                                                             | ✔️           | |
| Vampire             | Absorb health from the player upon dealing damage and transfer it to the attacker.   | ✔️           | |
| Mirrored Vampire    | The damage dealt to players is reflected back onto the attacking player.             | ✔️           | |
| Invisible           | Make the player invisible for specific duration                                      | ❌           | |

(Current status for version 1.0.0)

## 📝 Commands

| Command                            | Description                                     | Usage                                    |
| ---------------------------------- | ----------------------------------------------- | ---------------------------------------- |
| `dice`                             | Roll the dice to trigger a random effect.       | `!dice`                             |
| `rtd_reloadconfig`                 | Reload the configuration files                  | `!rtd_reloadconfig`                      |

## ⚙️ Configuration

After the initialization of the plugin, two files and a folder are created. Firstly, the Config.json file is generated, which holds the plugin settings. Secondly, the en.json file is created along with the 'translation' folder. The Config.json is intended for adjusting effects and general settings, while en.json is used for translation.
In config.json, all effects provided by the plugin are listed in the "effects" section, and there is a "general" section providing general settings for the plugin.

To switch the plugin's language, modify the 'language' attribute in the config.json and then either manually create or let the plugin generate a file in the 'translations' folder with the identical name specified in the 'config' file. Translate your content within that file. For guidance, you can refer to the default en.json. I suggest using ChatGPT for translation to prevent formatting issues

A description for the **general** section in config.json:
- **Language**: Determines the language for all translations.
- **DiceRollsPerRound**: Specifies the number of dice rolls allowed per round for each player.
- **BroadcastDiceRoll**: If true, everyone on the server can witness the outcome.
- **BroadcastDiceRollTerroristOnly**: If true, only Terrorists can see the dice rolls of other players.
- **BroadcastDiceRollCounterTerroristOnly**: If true, only Counter-Terrorists can see the dice rolls of other players.
- **BroadcastPluginCommand**: If true, a plugin command info is broadcasted at the beginning of each round.
- **UnicastDiceRoll**: If true, the LocalPlayer can see their own dice rolls.
- **UnicastEffectDescription**: If true, the effect description is displayed for the LocalPlayer.
- **UnicastRollAmount**: If true, the number of rolls available to the player is shown.
- **TsCanRoll**: Determines whether Terrorists are allowed to roll the dice.
- **CTsCanRoll**: Determines whether Counter-Terrorists are allowed to roll the dice.
- **ResetRollsOnRoundStart**: If true, all rolls and effects are reset at the start of each round.
- **ResetRollsOnDeath**: If true, rolls and effects are reset for a player upon death.

To manipulate the effects, the config.json displays three attributes for each effect: "Enabled," "Probability," and "Parameters."

- **Enabled**: This attribute, as the name suggests, toggles the effect on or off.
- **Probability**: This is the entry factor for the effect. It's not a direct probability but a factor determining the occurrence of the effect. Please note that this is not a probability in the traditional sense; it's a factor that influences the likelihood of the effect happening. Don't be surprised; I designed it this way to avoid having to adjust probabilities every time you tweak a value.
- **Parameters**: These are arguments specific to each effect. All values must be specified as strings; otherwise, it won't work. Refer to the standard parameters in the config file for guidance.

## 🔗 Dependencies

To ensure the CS2-RollTheDice-Plugin works correctly, you'll need to have the following dependencies installed on your server:

- **Metamod for CS2**: An essential mod that allows for the use of plugins on your server. Download it from [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master).

- **CounterStrikeSharp (Version 30 or higher)**: A library necessary for the plugin's functionality. Ensure you have at least version 30. Get it from [CounterStrikeSharp on GitHub](https://github.com/roflmuffin/CounterStrikeSharp).

Please follow the installation instructions provided on their respective websites to install these dependencies correctly before attempting to install the CS2-RollTheDice-Plugin.

## 🚀 Installation

1. Download the [latest release](https://github.com/Quantor97/CS2-RollTheDice-Plugin/releases)
2. Make sure you have CounterStrikeSharp and Metamod installed
3. Place the addons folder into your server's CounterStrikeSharp plugin directory.
4. Restart your server to see the plugin in action.

## 🗺️ Roadmap
- **Convars**: Convert the general settings into convars.
- **Localization Enhancement**: Automatically detect the player's region and select the appropriate translation, if available.
- **Effect Implementation**: Introduce the remaining planned effects and possibly add new ones.

## 💡 Contributing

If you want to contribute to the CS2-RollTheDice-Plugin, your pull requests are welcome.

## 🙌 Support & Donations

Appreciate the plugin? Consider supporting its ongoing development:

  <a href="https://www.buymeacoffee.com/quantor97">
    <img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" style="height: 50px !important;width: 217px !important;" >
  </a>

For private commissions and paid plugin development, \
I am open for business: [![Discord](https://img.shields.io/badge/Discord-preach9655-%237289DA.svg?style=flat-square&logo=discord&logoColor=white)](discord://-/users/preach9655)

## 📜 License

This project is under the MIT License.

---

<div align="center">

### ☕ Enjoying My Work?

Consider [buying me a coffee](https://www.buymeacoffee.com/quantor97) to fuel the development of more amazing features!

Looking for custom plugin development? Reach out to me at [![Discord](https://img.shields.io/badge/Discord-preach9655-%237289DA.svg?style=flat-square&logo=discord&logoColor=white)](discord://-/users/preach9655) for paid services.

</div>

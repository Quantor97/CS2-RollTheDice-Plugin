using System.Drawing;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Preach.CS2.Plugins.RollTheDiceV2.Utilities;

public static class Log 
{
    private static string Prefix => "[{red}RollTheDice{default}]";

    private static Dictionary<string, char> Colors = new Dictionary<string, char>()
    {
        { "red", ChatColors.Red },
        { "darkred", ChatColors.Darkred },
        { "green", ChatColors.Green },
        { "blue", ChatColors.Blue },
        { "darkblue", ChatColors.DarkBlue },
        { "yellow", ChatColors.Yellow },
        { "magenta", ChatColors.Magenta },
        { "white", ChatColors.White },
        { "default", ChatColors.Default },
        { "mark", ChatColors.Blue },
        { "mark2", ChatColors.Lime }
    };

    private static string GetLogTypePrefix(LogType? type)
    {
        return type switch 
        {
            LogType.DEFAULT => "",
            LogType.SUCCSS => "{green}[SUCCESS]{default}",
            LogType.INFO => "{blue}[INFO]{default}",
            LogType.WARNING => "{yellow}[WARNING]{default}",
            LogType.ERROR => "{red}[ERROR]{default}",
            LogType.DEBUG => "{magenta}[DEBUG]{default}",
            _ => ""
        };
    }

    private static string ReplaceColorCodes(string input, bool removeColorCode = false)
    {
        string pattern = @"\{(\w+)*\}";
        Regex regex = new Regex(pattern);

        return regex.Replace(input, match =>
        {
            string colorName = match.Groups[1].Value.ToLower();
            if (Colors.ContainsKey(colorName))
            {
                return !removeColorCode ? Colors[colorName].ToString() : "";
            }
            else
            {
                return !removeColorCode ? Colors["default"].ToString() : "";
            }
        });
    }

    private static string GetFormatedText(string message, LogType? type = LogType.DEFAULT, bool replaceColorsEmpty = false)
    {
        var formatedLog = Prefix + GetLogTypePrefix(type) + " " + message;

        if(replaceColorsEmpty)
            return ReplaceColorCodes(formatedLog, true);

        return ReplaceColorCodes(formatedLog);
    }

    public static void PrintCenter(CCSPlayerController playerController, string message, LogType? type = LogType.DEFAULT)
    {
        playerController.PrintToCenter(GetFormatedText(message, type, true));
    }

    public static void PrintChat(CCSPlayerController playerController, string message, LogType? type = LogType.DEFAULT)
    {
        playerController.PrintToChat(GetFormatedText(message, type));
    }

    public static void PrintChatAll(string message, LogType? type = LogType.DEFAULT, bool printServerConsole = false)
    {
        Server.PrintToChatAll(GetFormatedText(message, type));

        if(printServerConsole)
            PrintServerConsole(message, LogType.INFO);
    }

    public static void PrintServerConsole(string message, LogType? type = LogType.INFO)
    {
        if(!RollTheDice.DEBUG && type == LogType.DEBUG)
            return;

        switch(type)
        {
            case LogType.INFO:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LogType.ERROR:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogType.WARNING:
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                break;
            case LogType.DEBUG:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                break;
            case LogType.SUCCSS:
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                break;
        }

       Console.WriteLine(GetFormatedText(message, type, true));
       Console.ResetColor();
    }
}
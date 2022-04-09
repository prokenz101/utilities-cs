using System.Numerics;
using System.Text.RegularExpressions;

namespace utilities_cs {
    /// <summary>
    /// The hierarchy of all command-classes for all commands in utilities-cs.
    /// </summary>
    public class Command {
        public string? CommandName { get; set; }
        public string[]? Aliases { get; set; }
        public static Dictionary<string, Func<string[], bool, bool, string?>> FCommands = new();
        public static Dictionary<string, Action<string[]>> RCommands = new();
        /// <summary>
        /// Executes a command in either the rCommands dictionary or the fCommands dictionary.
        /// </summary>
        /// <param name="cmd">The name of the command to be excuted.</param>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        /// <returns>A string of the output of the command. This can also be null.</returns>
        public static string? ExecuteCommand(string cmd, string[] args, bool copy = true, bool notif = true) {
            if (FCommands.ContainsKey(cmd)) {
                string? output = FCommands[cmd].Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else if (RCommands.ContainsKey(cmd)) {
                RCommands[cmd].Invoke(args);
                return null;
            } else if (Force.AreAnyForced()) {
                args = Enumerable.Concat(new string[] { "cmd" }, args).ToArray<string>();
                string? output = Force.forced!.Function!.Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Welp.",
                        "It seems utilities couldn't understand what command you were trying to use.",
                        "6"
                    }
                ); return null;
            }
        }

        /// <summary>
        /// A simple method that checks if a certain command exists.
        /// </summary>
        /// <param name="cmd">The name of the command</param>
        /// <returns>True or False based on if the command exists, or not.</returns>
        public static bool Exists(string cmd) {
            if (FCommands.ContainsKey(cmd)) {
                return true;
            } else if (RCommands.ContainsKey(cmd)) {
                return true;
            } else {
                return false;
            }
        }

        public static object? GetMethod(string commandName) {
            if (Command.Exists(commandName)) {
                if (FCommands.ContainsKey(commandName)) {
                    return FCommands[commandName];
                } else if (RCommands.ContainsKey(commandName)) {
                    return RCommands[commandName];
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// The class that supports formattable commands.
    /// </summary>
    public class FormattableCommand : Command {
        public Func<string[], bool, bool, string?>? Function;
        public bool UseInAllCommand;
        public string? AllCommandMode;
        public static List<FormattableCommand> FormattableCommands = new();
        public FormattableCommand(
            string commandName,
            Func<string[], bool, bool, string?> function,
            string[]? aliases = null,
            bool useInAllCommand = false,
            string allCommandMode = "none"
        ) {
            //* setting all attributes for instance
            CommandName = commandName; Function = function; Aliases = aliases;
            UseInAllCommand = useInAllCommand; AllCommandMode = allCommandMode;
            if (aliases != null) {
                FCommands.Add(commandName, function);
                foreach (string alias in aliases) { FCommands.Add(alias, function); }
            } else {
                FCommands.Add(commandName, function);
            }

            FormattableCommands.Add(this);
        }

        /// <summary>
        /// A non-static command that allows you to execute a command immediately.
        /// </summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        //! Mostly unused method. Only used for testing purposes.
        public void Execute(string[] args, bool copy, bool notif) {
            if (this.Function != null) {
                string? output = this.Function.Invoke(args, copy, notif);
                if (output != null) { Console.WriteLine(output); }
            }
        }

        /// <summary>
        /// Lists all currently registered FormattableCommands.
        /// </summary>
        /// <returns>A string with all currently registered Commands, seperated by newlines.</returns>
        public static string ListAllFCommands() {
            List<string> fCommandsList = new();
            foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> i in Command.FCommands) {
                fCommandsList.Add(i.Key);
            }

            return string.Join("\n", fCommandsList);
        }

        /// <summary>
        /// Finds the command in the fCommands dictionary and then executes it.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        /// <returns>The output of the method that is ran. Value can be null.</returns>
        public static string? FindAndExecute(string cmd, string[] args, bool copy, bool notif) {
            if (FCommands.ContainsKey(cmd)) {
                string? output = FCommands[cmd].Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns every command that supports use in the 'all' command.
        /// </summary>
        /// <param name="mode">Mode for the command, fancy/encoding</param>
        /// <returns></returns>
        public static List<FormattableCommand> GetMethodsSupportedByAll(string mode) {
            List<FormattableCommand> methodsSupportedByAll = new();

            if (FormattableCommands != null) {
                foreach (FormattableCommand i in FormattableCommands) {
                    if (i.UseInAllCommand && i.AllCommandMode == mode) {
                        methodsSupportedByAll.Add(i);
                    }
                }
            }

            return methodsSupportedByAll;
        }

        /// <summary>
        /// Returns a FormattableCommand using the name of that command.
        /// </summary>
        /// <param name="cmd">The name of the command that is used to find the method and return it.</param>
        /// <returns>The method of that command name.</returns>
        public static Func<string[], bool, bool, string?>? GetFMethod(string cmd) {
            if (FCommands.ContainsKey(cmd)) {
                Func<string[], bool, bool, string?> func = FCommands[cmd];
                return func;
            } else {
                return null;
            }
        }

        public static FormattableCommand? GetFormattableCommand(string cmd) {
            foreach (FormattableCommand i in FormattableCommands) {
                if (i.CommandName == cmd) {
                    return i;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// The class that supports regular commands.
    /// </summary>
    public class RegularCommand : Command {
        public Action<string[]>? Function;
        public RegularCommand(
            string commandName,
            Action<string[]> function,
            string[]? aliases = null
        ) {
            //* setting all attributes for instance
            CommandName = commandName.ToLower(); Function = function; Aliases = aliases;
            if (aliases != null) {
                RCommands.Add(commandName, function);
                foreach (string alias in aliases) { RCommands.Add(alias, function); }
            } else {
                RCommands.Add(commandName, function);
            }
        }

        /// <summary>
        /// Lists all currently registered Regular Commands.
        /// </summary>
        /// <returns>A string with every RegularCommand, seperated by newlines.</returns>
        public static string ListAllRCommands() {
            List<string> rCommandsList = new();
            foreach (KeyValuePair<string, Action<string[]>> i in Command.RCommands) {
                rCommandsList.Add(i.Key);
            }

            return string.Join("\n", rCommandsList);
        }

        /// <summary>A non-static method that executes a command immediately.</summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        //! Mostly unused method. Only used for testing purposes.
        public void Execute(string[] args) {
            if (this.Function != null) {
                this.Function.Invoke(args);
            }
        }
    }

    /// <summary>
    /// The class containing all methods that are used for registering commands to the dictionaries.
    /// </summary>
    public class RegisterCommands {
        /// <summary>
        /// The method that registers all regular commands.
        /// </summary>
        public static void RegisterAllRCommands() {
            RegularCommand autoclick = new(
                "autoclick",
                Autoclick.Autoclicker
            );

            RegularCommand settings = new(
                commandName: "settings",
                function: (string[] args) => {
                    string mode = args[1];
                    switch (mode) {
                        case "modify":
                            try {
                                string setting = args[2];
                                string value = args[3];
                                SettingsJSON currentSettings = SettingsModification.GetSettings();
                                SettingsModification.ModifySetting(currentSettings, setting, value);
                            } catch (IndexOutOfRangeException) {
                                Utils.NotifCheck(true,
                                    new string[] {
                                        "Huh.",
                                        "It seems you did not input a setting/value.",
                                        "3"
                                    }
                                );
                            }
                            break;

                        case "reset":
                            SettingsModification.CreateDirAndJson();
                            Utils.NotifCheck(
                                true,
                                new string[] { "Reset.", "All settings have been reset to default.", "4" }
                            ); break;

                        case "list":
                            string settings = SettingsModification.ListAllSettings();
                            Utils.CopyCheck(true, settings);
                            Utils.NotifCheck(
                                true,
                                new string[] { "Success!", "The settings have been copied to your clipboard.", "3" }
                            );
                            break;

                        case "open":
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Opening settings file.",
                                    "Opening settings.json in your default editor.",
                                    "3"
                                }
                            );
                            SettingsModification.OpenSettingsJSON();
                            break;

                        case "refresh":
                            if (
                                SettingsModification.GetSettings().disableClipboardManipulation
                                && SettingsModification.GetSettings().autoPaste
                            ) {
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                        "Hey!",
                                        @"disableClipboardManipulation and autoPaste are mutually exclusive.
They cannot both be true at the same time."
                                    }
                                ); break;
                            } else {
                                UtilitiesAppContext.currentSettings = SettingsModification.GetSettings();
                                break;
                            }

                        default:
                            Utils.NotifCheck(true, new string[] { "Huh.", "It seems that was not a valid mode.", "3" });
                            break;
                    }
                }
            );

            RegularCommand force = new(
                commandName: "force",
                function: (string[] args) => {
                    string commandName = args[1];

                    //* check if command exists
                    if (Command.Exists(commandName)) {
                        //* check if command is already forced
                        if (Force.AreAnyForced()) {
                            if (Force.IsSpecificCmdForced(commandName)) {
                                Utils.NotifCheck(
                                    true,
                                    new string[] { "Huh.", "It seems that command has already been enabled.", "4" }
                                );
                                return;
                            } else {
                                Utils.NotifCheck(
                                    true,
                                    new string[] { "Huh.", "A command has already been enabled.", "3" }
                                );
                                return;
                            }
                        } else {
                            //* enable command
                            Force.ForceCommand(commandName);
                            Utils.NotifCheck(true, new string[] { "Success!", "That command has been enabled.", "3" });
                        }
                    } else {
                        Utils.NotifCheck(true, new string[] { "Huh.", "That command does not exist.", "3" });
                    }
                }
            );

            RegularCommand unforce = new(
                commandName: "unforce",
                function: (string[] args) => {
                    //* check if command is enabled
                    if (Force.AreAnyForced()) {
                        //* disable command
                        Utils.NotifCheck(
                            true,
                            new string[] { "Success!", $"The {Force.forced!.CommandName} command has been disabled.", "3" }
                        );
                        Force.UnForceCommand();
                    } else {
                        Utils.NotifCheck(true, new string[] { "Huh.", "That command was never enabled.", "3" });
                    }
                },
                aliases: new string[] { "un-force" }
            );

            RegularCommand factorial = new(
                commandName: "factorial",
                function: async (string[] args) => {
                    if (Utils.IndexTest(args)) {
                        return;
                    }

                    try {
                        await Task.Run(() => {
                            int n = int.Parse(args[1]);
                            int i = 1;
                            System.Numerics.BigInteger v = 1;

                            while (i <= n) {
                                v *= i;
                                i += 1;
                            }

                            Utils.CopyCheck(true, v.ToString());
                            Utils.NotifCheck(
                                true, new string[] { v.ToString(), $"The factorial is: {v.ToString()}", "5" }
                            );
                            return v.ToString();
                        });
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Huh.",
                        "It seems that the number you inputted was not a number.",
                        "4"
                            }
                        );
                        return;
                    }
                }
            );

            RegularCommand exit = new(
                commandName: "exit",
                function: (string[] args) => {
                    HookManager.UnregisterAllHooks();
                    Application.Exit();
                },
                aliases: new string[] { "quit" }
            );

            RegularCommand format = new(
                commandName: "format",
                function: (string[] args) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return;
                    }

                    Dictionary<string, string> formatdict = new();
                    System.Text.RegularExpressions.Regex re =
                        new System.Text.RegularExpressions.Regex(@"{(?<command>[^}]+)}");

                    System.Text.RegularExpressions.MatchCollection matches = re.Matches(text);

                    foreach (System.Text.RegularExpressions.Match? i in matches) {
                        if (i != null) {
                            System.Text.RegularExpressions.GroupCollection groups = i.Groups;
                            System.Text.RegularExpressions.Group mainGroup = groups["command"];

                            string cmd = mainGroup.ToString();
                            string[] splitcommand = cmd.Split(" ");

                            string? output = FormattableCommand.FindAndExecute(
                                splitcommand[0],
                                splitcommand, false, false
                            );

                            if (output == null) {
                                output = "errored";
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                        "Huh.",
                                        "Perhaps that was not a real command.",
                                        "4"
                                    }
                                ); formatdict[cmd] = output;
                            } else {
                                formatdict[cmd] = output;
                            }
                        }
                    }

                    Utils.CopyCheck(true, formatdict.ReplaceKeyInString(text));
                    Utils.NotifCheck(true, new string[] { "Success!", "Message copied to clipboard.", "3" });
                }
            );

            RegularCommand help = new(
                commandName: "help",
                function: (string[] args) => {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(
                            "cmd",
                            $"/c start https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki"
                        ) { CreateNoWindow = true }
                    );
                }
            );

            RegularCommand notification = new(
                commandName: "notification",
                function: (string[] args) => {
                    string text = string.Join(" ", args[1..]);

                    Dictionary<Match, GroupCollection>? matchToGroups = Utils.RegexFind(
                        text,
                        @"[""'](?<title>.*?)[""'],? [""'](?<subtitle>.*?)[""'],? (?<duration>\d+)",
                        useIsMatch: true,
                        () => {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Huh.",
                            "The parameters were not inputted properly.",
                            "3"
                                }
                            );
                        }
                    );

                    if (matchToGroups != null) {
                        foreach (KeyValuePair<Match, GroupCollection> kvp in matchToGroups) {
                            GroupCollection groups = kvp.Value;

                            string title = groups["title"].ToString();
                            string subtitle = groups["subtitle"].ToString();
                            int duration = int.Parse(groups["duration"].ToString());

                            Utils.NotifCheck(true, new string[] { title, subtitle, duration.ToString() });
                            return;
                        }
                    }
                },
                aliases: new string[] { "notify", "notif" }
            );

            RegularCommand remind = new(
                commandName: "remind",
                function: async (string[] args) => {
                    string text = string.Join(" ", args[1..]);

                    Dictionary<Match, GroupCollection>? matchToGroups = Utils.RegexFind(
                        text,
                        @"(?<time>\d+)(?<unit>h|m|s)(?<text> .*)?",
                        useIsMatch: true,
                        () => {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Huh.",
                                    "It seems the parameters were not given properly.",
                                    "3"
                                }
                            );
                        }
                    );

                    if (matchToGroups != null) {
                        List<int> timeEnumerable = new();
                        List<char> unitEnumerable = new();
                        List<string> textEnumerable = new();

                        foreach (KeyValuePair<Match, GroupCollection> kvp in matchToGroups) {
                            timeEnumerable.Add(int.Parse(kvp.Value["time"].ToString())); //* float
                            unitEnumerable.Add(kvp.Value["unit"].ToString().ToCharArray()[0]); //* char
                            textEnumerable.Add(kvp.Value["text"].ToString()); //* string
                        }

                        int time = timeEnumerable[0];
                        char unit = unitEnumerable[0];
                        string reminderText = textEnumerable[0];

                        Dictionary<char, string[]> timeOptions = new() {
                            { 's', new string[] { "1", "second" } },
                            { 'm', new string[] { "60", "minute" } },
                            { 'h', new string[] { "3600", "hour" } }
                        };

                        await Task.Run(() => { //* Task for reminder.
                            if (timeOptions.ContainsKey(unit)) {
                                int multiplier = int.Parse(timeOptions[unit][0]);
                                string word = timeOptions[unit][1].ToString();
                                int timeSeconds = (time * 1000) * multiplier;

                                Task.Delay(timeSeconds).Wait();

                                if (time == 1 && reminderText == string.Empty) {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! You set a reminder for 1 {word} and it's time!",
                                            "6"
                                        }
                                    );
                                } else if (reminderText == string.Empty) {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! You set a reminder for {time} {word}s and it's time!",
                                            "6"
                                        }
                                    );
                                } else {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! Your reminder was: {reminderText}",
                                            "6"
                                        }
                                    );
                                }
                            }
                        });
                    }
                },
                aliases: new string[] { "reminder" }
            );

            RegularCommand googleSearch = new(
                commandName: "-",
                function: (string[] args) => {
                    string searchQuery = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://google.com/search?q={searchQuery}"
                    ) { CreateNoWindow = true });
                },
                aliases: new string[] { "gs" }
            );

            RegularCommand youtubeSearch = new(
                commandName: "youtube",
                function: (string[] args) => {
                    string searchQuery = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://youtube.com/results?search_query={searchQuery}"
                    ) { CreateNoWindow = true });
                },
                aliases: new string[] { "yt" }
            );

            RegularCommand imageSearch = new(
                commandName: "images",
                function: (string[] args) => {
                    string searchQuery = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd",
                        $"/c start https://www.google.com/search?q={searchQuery}^&safe=strict^&tbm=isch^&sxsrf=ALeKk029ouHDkHfq3RFVc8WpFzOvZZ8s4g%3A1624376552976^&source=hp^&biw=1536^&bih=763^&ei=6ATSYIOrOduJhbIPzda7yAs^&oq=hello^&gs_lcp=CgNpbWcQAzIFCAAQsQMyBQgAELEDMgIIADICCAAyAggAMgIIADICCAAyBQgAELEDMgUIABCxAzICCAA6BwgjEOoCECc6BAgjECc6CAgAELEDEIMBUNIGWKcJYLELaABwAHgAgAGPAogByAqSAQUwLjEuNZgBAKABAaoBC2d3cy13aXotaW1nsAEK^&sclient=img^&ved=0ahUKEwiDv62byqvxAhXbREEAHU3rDrkQ4dUDCAc^&uact=5"
                    ) { CreateNoWindow = true });
                }
            );

            RegularCommand translate = new(
                commandName: "translate",
                function: (string[] args) => {
                    string lang = args[1];
                    string text = string.Join('+', args[2..]);

                    //* checking if lang is english

                    foreach (var englishLangAliases in Translate.englishDict.Keys) {
                        if (lang == englishLangAliases) {
                            Translate.toEnglish(text);
                            return;
                        }
                    }

                    //* if lang is not english, then use toOtherLang()

                    foreach (var langAliases in Translate.languages.Keys) {
                        if (langAliases.Contains(lang)) {
                            Translate.toOtherLang(Translate.languages[langAliases], text);
                            break;
                        }
                    }
                }
            );
        }

        /// <summary>
        /// The method that registers all formattable commands.
        /// </summary>
        public static void RegisterAllFCommands() {
            FormattableCommand all = new(
                commandName: "all",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string category = args[1];
                    string? all = All.returnCategory(args[1..], category, copy, notif);
                    if (all != null) {
                        Utils.CopyCheck(copy, all);
                        Utils.NotifCheck(notif, new string[] {
                            "Success!", "Text copied to clipboard.", "2"
                        });
                        return all;
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Huh.",
                                "It seems you did not input a valid category.",
                                "4"
                            }
                        );
                        return null;
                    }
                }
            );

            FormattableCommand base32 = new(
                commandName: "base32",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[2..]);
                    string mode = args[1];

                    if (text.Contains("=")) {
                        text = text.Replace("=", string.Empty);
                    }

                    if (mode == "to") {
                        //* convert text to base32
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
                        string strToBase32 = Base32.ToBase32String(bytes)!;

                        Utils.CopyCheck(copy, strToBase32);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return strToBase32;
                    } else if (mode == "from") {
                        try {
                            string base32ToString = System.Text.Encoding.Default.GetString(
                                Base32.FromBase32String(text)!
                        );
                            Utils.CopyCheck(copy, base32ToString);
                            Utils.NotifCheck(notif, new string[] {
                                "Success!", $"The message was: {base32ToString}", "8"
                            });
                            return base32ToString;
                        } catch (ArgumentException) {
                            Utils.NotifCheck(true, new string[] {
                                "Huh.", "Are you sure that text was actual Base32?", "3"
                            });
                            return null;
                        }
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                "Huh.",
                "It seems you did not input a proper mode for Base32 to convert to.",
                "4"
                            }
                        );
                        return null;
                    }
                },
                aliases: new string[] { "b32" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand base64 = new(
                commandName: "base64",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    if (IsBase64.IsBase64String(text)) {
                        try {
                            string ans = Base64Convert.Base64Decode(text);
                            Utils.CopyCheck(copy, ans);
                            Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {ans}", "6" });
                            return ans;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Huh.",
                            "An exception occured when converting this text to Base64.",
                            "4"
                                }
                            );
                            return null;
                        }
                    } else {
                        string ans = Base64Convert.Base64Encode(text);
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif, new string[] { "Success!", "The message was copied to your clipboard.", "3" }
                        ); return ans;
                    }
                },
                aliases: new string[] { "b64" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand ascii85 = new(
                commandName: "ascii85",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string mode = args[1];
                    string text = string.Join(" ", args[2..]);
                    var ascii85 = new Ascii85();
                    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(text);

                    switch (mode) {
                        case "to":
                            try {
                                string encodedWithArrows = ascii85.Encode(byteArray);
                                if (encodedWithArrows.StartsWith("<~") && encodedWithArrows.EndsWith("~>")) {
                                    string encoded = encodedWithArrows.Remove(encodedWithArrows.Length - 2)[2..];

                                    Utils.CopyCheck(copy, encoded);
                                    Utils.NotifCheck(true, new string[] { "Success!", "Message copied to clipboard.", "3" });
                                    return encoded;
                                } else {
                                    Utils.CopyCheck(copy, encodedWithArrows);
                                    Utils.NotifCheck(true, new string[] { "Success!", "Message copied to clipboard.", "3" });
                                    return encodedWithArrows;
                                }
                            } catch {
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                        "Something went wrong.",
                                        "Looks like something went wrong when trying to convert your text to Base85.",
                                        "4"
                                    }
                                );
                                return null;
                            }

                        case "from":
                            try {
                                if (!(text.StartsWith("<~") && text.EndsWith("~>"))) {
                                    text = "<~" + text + "~>";
                                }

                                string decoded = System.Text.Encoding.ASCII.GetString(ascii85.Decode(text));
                                Utils.CopyCheck(copy, decoded);
                                Utils.NotifCheck(true, new string[] { "Success!", "Message copied to clipboard.", "3" });
                                return decoded;
                            } catch {
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                        "Something went wrong.",
                                        "Looks like something went wrong when trying to convert your text from Base85.",
                                        "4"
                                    }
                                );
                                return null;
                            }

                        default:
                            Utils.NotifCheck(true, new string[] { "Huh.", "Perhaps that was not an actual mode.", "3" });
                            return null;
                    }
                },
                aliases: new string[] { "base85", "b85" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand binary = new(
                commandName: "binary",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    if (!Utils.FormatValid("01 ", text)) {
                        byte[] ConvertToByteArray(string str, System.Text.Encoding encoding) {
                            return encoding.GetBytes(str);
                        }

                        string ToBinary(Byte[] data) {
                            return string.Join(
                                " ",
                                data.Select(
                                    byt => Convert.ToString(byt, 2).PadLeft(8, '0')
                                )
                            );
                        }

                        string ans = ToBinary(ConvertToByteArray(text, System.Text.Encoding.ASCII));
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        Utils.CopyCheck(copy, ans);
                        return ans;

                    } else {
                        try {
                            string[] textList = text.Split(" ");

                            var chars = from split in textList
                                        select ((char)Convert.ToInt32(split, 2)).ToString();
                            Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {string.Join("", chars)}", "10" });
                            Utils.CopyCheck(copy, string.Join("", chars));
                            return string.Join("", chars);
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Huh.",
                                    @"Something went wrong with converting this binary.",
                                    "3"
                                }
                            ); return null;
                        }
                    }
                },
                aliases: new string[] { "bin" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand bubbletext = new(
                commandName: "bubbletext",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var bubbleChar = new Dictionary<string, string>() {
                        {"a", "ⓐ"},
                        {"b", "ⓑ"},
                        {"c", "ⓒ"},
                        {"d", "ⓓ"},
                        {"e", "ⓔ"},
                        {"f", "ⓕ"},
                        {"g", "ⓖ"},
                        {"h", "ⓗ"},
                        {"i", "ⓘ"},
                        {"j", "ⓙ"},
                        {"k", "ⓚ"},
                        {"l", "ⓛ"},
                        {"m", "ⓜ"},
                        {"n", "ⓝ"},
                        {"o", "ⓞ"},
                        {"p", "ⓟ"},
                        {"q", "ⓠ"},
                        {"r", "ⓡ"},
                        {"s", "ⓢ"},
                        {"t", "ⓣ"},
                        {"u", "ⓤ"},
                        {"v", "ⓥ"},
                        {"w", "ⓦ"},
                        {"x", "ⓧ"},
                        {"y", "ⓨ"},
                        {"z", "ⓩ"},
                        {"A", "Ⓐ"},
                        {"B", "Ⓑ"},
                        {"C", "Ⓒ"},
                        {"D", "Ⓓ"},
                        {"E", "Ⓔ"},
                        {"F", "Ⓕ"},
                        {"G", "Ⓖ"},
                        {"H", "Ⓗ"},
                        {"I", "Ⓘ"},
                        {"J", "Ⓙ"},
                        {"K", "Ⓚ"},
                        {"L", "Ⓛ"},
                        {"M", "Ⓜ"},
                        {"O", "Ⓞ"},
                        {"N", "Ⓝ"},
                        {"P", "Ⓟ"},
                        {"Q", "Ⓠ"},
                        {"R", "Ⓡ"},
                        {"S", "Ⓢ"},
                        {"T", "Ⓣ"},
                        {"U", "Ⓤ"},
                        {"V", "Ⓥ"},
                        {"W", "Ⓦ"},
                        {"X", "Ⓧ"},
                        {"Y", "Ⓨ"},
                        {"Z", "Ⓩ"},
                        {"1", "①"},
                        {"2", "②"},
                        {"3", "③"},
                        {"4", "④"},
                        {"5", "⑤"},
                        {"6", "⑥"},
                        {"7", "⑦"},
                        {"8", "⑧"},
                        {"9", "⑨"},
                        {"0", "⓪"}
                    };

                    foreach (char b in text) {
                        var replaced = bubbleChar.GetValueOrDefault(b.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(b.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                aliases: new string[] { "bubble" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand commaseperator = new(
                commandName: "commaseperator",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string strNum = string.Join(' ', args[1..]);

                    try {
                        //* Checking if number is an actual number
                        System.Numerics.BigInteger.Parse(strNum);
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Huh.",
                        "It seems you did not input anything to seperate with commas.",
                        "2"
                            }
                        );
                        return null;
                    }

                    System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(strNum);
                    string ans = String.Format("{0:n0}", num);

                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Number copied to clipboard.", "3" });
                    return ans;
                },
                aliases: new string[] { "cms" }
            );

            FormattableCommand copypaste = new(
                commandName: "copypaste",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    Dictionary<string, string> cpDict = new() {
                        { "aigu e", "é" },
                        { "aigu E", "É" },
                        { "grave a", "à" },
                        { "grave e", "è" },
                        { "grave u", "ù" },
                        { "grave A", "À" },
                        { "grave E", "È" },
                        { "grave U", "Ù" },
                        { "chapeau a", "â" },
                        { "chapeau e", "ê" },
                        { "chapeau i", "î" },
                        { "chapeau o", "ô" },
                        { "chapeau u", "û" },
                        { "chapeau A", "Â" },
                        { "chapeau E", "Ê" },
                        { "chapeau I", "Î" },
                        { "chapeau O", "Ô" },
                        { "chapeau U", "Û" },
                        { "trema e", "ë" },
                        { "trema i", "ï" },
                        { "trema u", "ü" },
                        { "bullet", "•" },
                        { "trema E", "Ë" },
                        { "trema I", "Ï" },
                        { "trema U", "Ü" },
                        { "cedille c", "ç" },
                        { "cedille C", "Ç" },
                        { "3164", "ㅤ" },
                        { "hangul filler", "ㅤ" },
                        { "divison", "÷" },
                        { "divide", "÷" },
                        { "multi", "×" },
                        { "!=", "≠" },
                        { "congruence", "≅" },
                        { "greater than or equal to", "≥" },
                        { ">=", "≥" },
                        { "lesser than or equal to", "≤" },
                        { "<=", "≤" },
                        { "shrug", @"¯\_(ツ)_/¯" },
                        { "trademark", "™️" },
                        { "tm", "™️" },
                        { "registered", "®" },
                        { "rtm", "®" },
                        { "copyright", "©️" },
                        { "music", "♩♪♫♬" },
                        { "therefore", "∴" },
                        { "<==", "⇐" },
                        { "==>", "⇒" },
                        { "<-", "⭠" },
                        { "->", "⭢" },
                        { "<<-", "↞" },
                        { "->>", "↠" },
                        { "int16maxvalue", Int16.MaxValue.ToString() },
                        { "int32maxvalue", Int32.MaxValue.ToString() },
                        { "int64maxvalue", Int64.MaxValue.ToString() }
                    };

                    if (cpDict.ContainsKey(text)) {
                        Utils.CopyCheck(copy, cpDict[text]);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return cpDict[text];
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Welp.",
                        "It seems that utilities could not understand what word you were trying to copypaste.",
                        "3"
                            }
                        );
                        return null;
                    }
                },
                aliases: new string[] { "cp" }
            );

            FormattableCommand creepy = new(
                commandName: "creepy",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var creepyChar = new Dictionary<string, string>() {
                    {"a", "á̷͍̖̐̐͘"}, {"b", "ḃ̶̢̹̖"}, {"c", "c̸̢̧̰̙͔̲̿̈́͌̉̀͘"}, {"d", "d̸͉͛̈́̊̍͘"}, {"e", "ḗ̸̫̽"},
                    {"f", "f̸̡̹̱̹̺͋͒͋"}, {"g", "g̴̼̙̜͒̄̈́̚͝"}, {"h", "h̴̜̕"}, {"i", "í̸͓̬͚̘̆"}, {"j", "j̶̯͋̋͋"},
                    {"k", "k̴̛̰̻͈͘͘͜"}, {"l", "l̸͔̠̝̪̯͇͐̓͆"}, {"m", "m̴̲̗͗̽̂͌"}, {"n", "n̸͈͇̳̈̾̿̄ͅ"}, {"o", "o̵̧̜̖͈̲͔͂͋́͝"},
                    {"p", "p̶̡̯̳͓̣͂̈́́͘"}, {"q", "q̴̡͓̭̠̂͋̈́̔"}, {"r", "r̶͍̎"}, {"s", "s̴͈͎̙̘̱̋ͅ"}, {"t", "ţ̶̠̜̙͚̎͗"},
                    {"u", "ų̸̙̭͋ͅ"}, {"v", "v̶̗͂̑̕̚"}, {"w", "w̸͉͂̈́̅̌̊"}, {"x", "x̴͕̞̙̮͐͐͒"}, {"y", "ÿ̵̠͍̪̠̩́"},
                    {"z", "z̶̞͖̓̚"}, {"A", "A̷̡͍̩͉̱̹͑̒̀̑͝"}, {"B", "B̵̯̭̄̀̾̑"}, {"C", "C̷̗̽͛"}, {"D", "D̴͖͈̯̜̭̊̓̏͆̆͘"},
                    {"E", "Ḙ̷̦̠̍"}, {"F", "F̶̛̮̤̈́̿̈́͂̂"}, {"G", "Ĝ̶̨̢̺̻̹̦̅͆̈́͗"}, {"H", "H̸̼͖̦̗͛͗͐̿̀̀ͅ"}, {"I", "Į̶̛̩͙̭͕́̌̏̚"},
                    {"J", "J̷̜̀͆̄͛̆"}, {"K", "Ḵ̴̨̧̨͔̾"}, {"L", "Ḻ̶̰̱̹͎͈̔"}, {"M", "M̵̠̲̞̿̋̐̕̕͝"}, {"N", "Ṅ̷̻"},
                    {"O", "O̸̞̍̐"}, {"P", "P̵͈͊͋͂͗͝"}, {"Q", "Q̸̡͉̥̱͕̩̄̈́"}, {"R", "R̵̻̺̯͗̇͜"}, {"S", "S̴͖̬̀̇̃͋̈"},
                    {"T", "T̵͓̫̠̈́̂̀̓́̍ͅ"}, {"U", "Ụ̷̡͚̻͇͆͑̉͋͝"}, {"V", "V̴̟̪͓͓̩̳̄̀͌̾̕"}, {"W", "W̵̞̯͛̿"}, {"X", "X̷͈͍̬́"},
                    {"Y", "Ỳ̶̖̣͌͜"}, {"Z", "Z̴̗͈̬̱̩̆̊͗"}, {" ", " "}
                };

                    foreach (char cr in text) {
                        var replaced = creepyChar.GetValueOrDefault(cr.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(cr.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand wingdings = new(
                commandName: "wingdings",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    Dictionary<string, string> wingdingsChar = new() {
                        { "a", "♋︎" },
                        { "b", "♌︎" },
                        { "c", "♍︎" },
                        { "d", "♎︎" },
                        { "e", "♏︎" },
                        { "f", "♐︎" },
                        { "g", "♑︎" },
                        { "h", "♒︎" },
                        { "i", "♓︎" },
                        { "j", "🙰" },
                        { "k", "🙵" },
                        { "l", "●︎" },
                        { "m", "❍︎" },
                        { "n", "■︎" },
                        { "o", "□︎" },
                        { "p", "◻︎" },
                        { "q", "❑︎" },
                        { "r", "❒︎" },
                        { "s", "⬧︎" },
                        { "t", "⧫︎" },
                        { "u", "◆︎" },
                        { "v", "❖︎" },
                        { "w", "⬥︎" },
                        { "x", "⌧︎" },
                        { "y", "⍓︎" },
                        { "z", "⌘︎" },
                        { "A", "✌︎" },
                        { "B", "👌︎" },
                        { "C", "👍︎" },
                        { "D", "👎︎" },
                        { "E", "☜︎" },
                        { "F", "☞︎" },
                        { "G", "☝︎" },
                        { "H", "☟︎" },
                        { "I", "✋︎" },
                        { "J", "☺︎" },
                        { "K", "😐︎" },
                        { "L", "☹︎" },
                        { "M", "💣︎" },
                        { "N", "☠︎" },
                        { "O", "⚐︎" },
                        { "P", "🏱︎" },
                        { "Q", "✈︎" },
                        { "R", "☼︎" },
                        { "S", "💧︎" },
                        { "T", "❄︎" },
                        { "U", "🕆︎" },
                        { "V", "✞︎" },
                        { "W", "🕈︎" },
                        { "X", "✠︎" },
                        { "Y", "✡︎" },
                        { "Z", "☪︎" },
                        { "!", "✏︎" },
                        { "\"", "✂︎" },
                        { "#", "✁︎" },
                        { "$", "👓︎" },
                        { "%", "🕭︎" },
                        { "&", "🕮︎" },
                        { "'", "🕯︎" },
                        { "(", "🕿︎" },
                        { ")", "✆︎" },
                        { "*", "🖂︎" },
                        { "+", "🖃︎" },
                        { ",", "📪︎" },
                        { "-", "📫︎" },
                        { ".", "📬︎" },
                        { "/", "📭︎" },
                        { @"\", "ॐ︎" },
                        { "0", "📁︎" },
                        { "1", "📂︎" },
                        { "2", "📄︎" },
                        { "3", "🗏︎" },
                        { "4", "🗐︎" },
                        { "5", "🗄︎" },
                        { "6", "⌛︎" },
                        { "7", "🖮︎" },
                        { "8", "🖰︎" },
                        { "9", "🖲︎" },
                        { ":", "🖳︎" },
                        { ";", "🖴︎" },
                        { "<", "🖫︎" },
                        { "=", "🖬︎" },
                        { ">", "✇︎" },
                        { "?", "✍︎" },
                        { "@", "@" },
                        { "[", "☯︎" },
                        { "]", "☸︎" },
                        { "^", "♈︎" },
                        { "_", "♉︎" },
                        { "`", "♊︎" },
                        { "{", "❀︎" },
                        { "|", "✿︎" },
                        { "}", "❝︎" },
                        { "~", "❞︎" },
                    };

                    foreach (char i in text) {
                        if (wingdingsChar.ContainsKey(i.ToString())) {
                            converted.Add(wingdingsChar[i.ToString()]);
                        } else {
                            converted.Add(i.ToString());
                        }
                    }

                    string result = string.Join("", converted);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return result;
                },
                aliases: new string[] { "wd" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand cuberoot = new(
                commandName: "cuberoot",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    //* testing if string is a double
                    try {
                        Convert.ToDouble(text);
                    } catch (FormatException) {
                        Utils.NotifCheck(true, new string[] { "Huh.", "It seems you did not input a number." });
                        return null;
                    }

                    //* checking if there are commas in the number
                    if (text.Contains(",")) {
                        text = text.Replace(",", string.Empty);
                    }

                    double num = Convert.ToDouble(text);
                    string result = Math.Pow(num, ((double)1 / 3)).ToString();

                    System.Text.RegularExpressions.Regex re =
                        new System.Text.RegularExpressions.Regex(@"\d+\.99999+(?:\d+)");

                    if (re.IsMatch(result)) {
                        result = Math.Ceiling(Convert.ToDouble(result)).ToString();
                    }

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, new string[] { "Success!", $"The answer is: {result}", "4" });
                    return result;
                },
                aliases: new string[] { "cbrt" }
            );

            FormattableCommand cursive = new(
                commandName: "cursive",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var cursiveChar = new Dictionary<string, string>() {
                    {"a", "𝓪"}, {"b", "𝓫"}, {"c", "𝓬"}, {"d", "𝓭"}, {"e", "𝓮"},
                    {"f", "𝓯"}, {"g", "𝓰"}, {"h", "𝓱"}, {"i", "𝓲"}, {"j", "𝓳"},
                    {"k", "𝓴"}, {"l", "𝓵"}, {"m", "𝓶"}, {"n", "𝓷"}, {"o", "𝓸"},
                    {"p", "𝓹"}, {"q", "𝓺"}, {"r", "𝓻"}, {"s", "𝓼"}, {"t", "𝓽"},
                    {"u", "𝓾"}, {"v", "𝓿"}, {"w", "𝔀"}, {"x", "𝔁"}, {"y", "𝔂"},
                    {"z", "𝔃"}, {"A", "𝓐"}, {"B", "𝓑"}, {"C", "𝓒"}, {"D", "𝓓"},
                    {"E", "𝓔"}, {"F", "𝓕"}, {"G", "𝓖"}, {"H", "𝓗"}, {"I", "𝓘"},
                    {"J", "𝓙"}, {"K", "𝓚"}, {"L", "𝓛"}, {"M", "𝓜"}, {"N", "𝓝"},
                    {"O", "𝓞"}, {"P", "𝓟"}, {"Q", "𝓠"}, {"R", "𝓡"}, {"S", "𝓢"},
                    {"T", "𝓣"}, {"U", "𝓤"}, {"V", "𝓥"}, {"W", "𝓦"}, {"Y", "𝓨"},
                    {"X", "𝓧"}, {"Z", "𝓩"}, {" ", " "}
                };

                    foreach (char c in text) {
                        var replaced = cursiveChar.GetValueOrDefault(c.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(c.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand doublestruck = new(
                commandName: "doublestruck",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var dbsChar = new Dictionary<string, string>() {
                    {"a", "𝕒"}, {"b", "𝕓"}, {"c", "𝕔"}, {"d", "𝕕"}, {"e", "𝕖"},
                    {"f", "𝕗"}, {"g", "𝕘"}, {"h", "𝕙"}, {"i", "𝕚"}, {"j", "𝕛"},
                    {"k", "𝕜"}, {"l", "𝕝"}, {"m", "𝕞"}, {"n", "𝕟"}, {"o", "𝕠"},
                    {"p", "𝕡"}, {"q", "𝕢"}, {"r", "𝕣"}, {"s", "𝕤"}, {"t", "𝕥"},
                    {"u", "𝕦"}, {"v", "𝕧"}, {"w", "𝕨"}, {"x", "𝕩"}, {"y", "𝕪"},
                    {"z", "𝕫"}, {"A", "𝔸"}, {"B", "𝔹"}, {"C", "ℂ"}, {"D", "𝔻"},
                    {"E", "𝔼"}, {"F", "𝔽"}, {"H", "ℍ"}, {"I", "𝕀"}, {"J", "𝕁"},
                    {"K", "𝕂"}, {"L", "𝕃"}, {"M", "𝕄"}, {"N", "ℕ"}, {"O", "𝕆"},
                    {"P", "ℙ"}, {"Q", "ℚ"}, {"R", "ℝ"}, {"S", "𝕊"}, {"T", "𝕋"},
                    {"U", "𝕌"}, {"V", "𝕍"}, {"W", "𝕎"}, {"X", "𝕏"}, {"Y", "𝕐"},
                    {"Z", "ℤ"}, {"1", "𝟙"}, {"2", "𝟚"}, {"3", "𝟛"}, {"4", "𝟜"},
                    {"5", "𝟝"}, {"6", "𝟞"}, {"7", "𝟟"}, {"8", "𝟠"}, {"9", "𝟡"},
                    {"0", "𝟘"}, {" ", " "}
                };

                    foreach (char d in text) {
                        var replaced = dbsChar.GetValueOrDefault(d.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(d.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                aliases: new string[] { "dbs" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand emojify = new(
                commandName: "emojify",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);

                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    List<string> converted = new();
                    Dictionary<string, string> specialChar = new Dictionary<string, string> {
                        {" ", ":black_large_square:"},
                        {"?", ":question:"},
                        {"!", ":exclamation:"},
                        {"1", ":one:"},
                        {"2", ":two:"},
                        {"3",":three:"},
                        {"4", ":four:"},
                        {"5", ":five:"},
                        {"6", ":six:"},
                        {"7", ":seven:"},
                        {"8", ":eight:"},
                        {"9", ":nine:"},
                        {"0", ":zero:"},
                    };

                    foreach (char i in text) {
                        if (Utils.FormatValid(
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                           i.ToString()
                        )) {
                            converted.Add($":regional_indicator_{i.ToString().ToLower()}:");
                        } else if (specialChar.ContainsKey(i.ToString())) {
                            converted.Add(specialChar[i.ToString()]);
                        } else {
                            converted.Add(i.ToString());
                        }
                    }

                    Utils.CopyCheck(copy, string.Join(" ", converted));
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return string.Join(" ", converted);
                }
            );

            FormattableCommand leet = new(
                commandName: "leet",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    text = text.ToUpper();
                    Dictionary<string, string> leetChar = new() {
                        { "E", "3" },
                        { "I", "1" },
                        { "O", "0" },
                        { "A", "4" },
                        { "S", "5" }
                    };
                    List<string> converted = new();

                    foreach (char i in text) {
                        if (leetChar.ContainsKey(i.ToString())) {
                            converted.Add(leetChar[i.ToString()]);
                        } else {
                            converted.Add(i.ToString());
                        }
                    }

                    string result = string.Join("", converted);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return result;
                },
                aliases: new string[] { "numberize", "numberise" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand pi = new(
                commandName: "pi",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    //* check if num given is an actual number
                    try {
                        int num = int.Parse(args[1]);

                        //* first 10,000 digits of pi
                        string pi = PiDigits.piDigits;
                        Func<string, string> handleOutput = (string result) => {
                            Utils.CopyCheck(copy, result);
                            Utils.NotifCheck(
                                notif,
                                new string[] { "Success", "The digits have been copied to clipboard.", "1" }
                            );
                            return result;
                        };

                        if (num <= 0) {
                            throw new ArgumentException();
                        } else if (num > 10000) {
                            throw new OverflowException();
                        } else {
                            return handleOutput("3." + pi[0..num]);
                        }

                    } catch (FormatException) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Something went wrong.",
                                "Perhaps the number you inputted was not a real number.",
                                "5"
                            }
                        );
                        return null;
                    } catch (ArgumentException) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Hey!",
                                "You can't get a zero or negative amount of pi digits.",
                                "4"
                            }
                        );
                        return null;
                    } catch (OverflowException) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "That's a bit too far.",
                                "The limit to getting pi values is 10,000.",
                                "4"
                            }
                        );
                        return null;
                    }
                }
            );

            FormattableCommand lorem = new(
                commandName: "lorem",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    //* lorem string
                    string lorem = LoremIpsum.LoremIpsumFull;
                    if (args[1] == "all") {
                        Utils.CopyCheck(copy, lorem);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return lorem;
                    } else {
                        try {
                            int numOfWords = int.Parse(args[1]);
                            //* split lorem into array by space
                            string[] loremArray = lorem.Split(" ");

                            if (numOfWords >= 1 && !(numOfWords > 10000)) {
                                string[] result = loremArray[0..(numOfWords)];

                                Utils.CopyCheck(copy, string.Join(" ", result));
                                Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                                return string.Join(" ", result);
                            } else if (numOfWords > 10000) {
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                        "That's a bit too far.", "The limit to getting lorem values is 10,000.", "4"
                                    }
                                );
                                return null;
                            } else {
                                Utils.NotifCheck(
                                    true,
                                    new string[] { "Hey!", "You can't get a zero or negative amount of words.", "4" }
                                ); return null;
                            }
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] { "Huh.", "It seems you did not input the parameters correctly.", "4" }
                            ); return null;
                        }
                    }

                }
            );

            FormattableCommand exponent = new(
                commandName: "exponent",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    Dictionary<string, string> exponents = new() {
                        { "-", "⁻" },
                        { "=", "⁼" },
                        { "+", "⁺" },
                        { "1", "¹" },
                        { "2", "²" },
                        { "3", "³" },
                        { "4", "⁴" },
                        { "5", "⁵" },
                        { "6", "⁶" },
                        { "7", "⁷" },
                        { "8", "⁸" },
                        { "9", "⁹" },
                        { "0", "⁰" },
                        { "a", "ᵃ" },
                        { "b", "ᵇ" },
                        { "c", "ᶜ" },
                        { "d", "ᵈ" },
                        { "e", "ᵉ" },
                        { "f", "ᶠ" },
                        { "g", "ᵍ" },
                        { "h", "ʰ" },
                        { "i", "ᶦ" },
                        { "j", "ʲ" },
                        { "k", "ᵏ" },
                        { "l", "ˡ" },
                        { "m", "ᵐ" },
                        { "n", "ⁿ" },
                        { "o", "ᵒ" },
                        { "p", "ᵖ" },
                        { "r", "ʳ" },
                        { "s", "ˢ" },
                        { "t", "ᵗ" },
                        { "u", "ᵘ" },
                        { "v", "ᵛ" },
                        { "w", "ʷ" },
                        { "x", "ˣ" },
                        { "y", "ʸ" },
                        { "z", "ᶻ" },
                        { "(", "⁽" },
                        { ")", "⁾" },
                    };

                    foreach (char e in text) {
                        var replaced = exponents.GetValueOrDefault(e.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(e.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                aliases: new string[] { "ep" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand flip = new(
                commandName: "flip",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var flippedChar = new Dictionary<string, string>() {
                    {"a", "ɐ"}, {"b", "q"}, {"c", "ɔ"}, {"d", "p"}, {"e", "ǝ"},
                    {"f", "ɟ"}, {"g", "ƃ"}, {"h", "ɥ"}, {"i", "ᴉ"}, {"j", "ɾ"},
                    {"k", "ʞ"}, {"l", "l"}, {"m", "ɯ"}, {"n", "u"}, {"o", "o"},
                    {"p", "d"}, {"r", "ɹ"}, {"s", "s"}, {"t", "ʇ"}, {"u", "n"},
                    {"v", "ʌ"}, {"w", "ʍ"}, {"x", "x"}, {"y", "ʎ"}, {"z", "z"},
                    {"A", "∀"}, {"B", "q"}, {"C", "Ɔ"}, {"D", "p"}, {"E", "Ǝ"},
                    {"F", "Ⅎ"}, {"G", "פ"}, {"H", "H"}, {"I", "I"}, {"J", "ſ"},
                    {"K", "ʞ"}, {"L", "˥"}, {"M", "W"}, {"N", "N"}, {"O", "O"},
                    {"P", "Ԁ"}, {"Q", "Q"}, {"R", "ɹ"}, {"S", "S"}, {"T", "┴"},
                    {"U", "∩"}, {"V", "Λ"}, {"W", "M"}, {"X", "X"}, {"Y", "⅄"}, {"Z", "Z"}
                };

                    foreach (char f in text) {
                        var replaced = flippedChar.GetValueOrDefault(f.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(f.ToString());
                        }
                    }

                    converted.Reverse();
                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                aliases: new string[] { "flipped", "upside-down" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand shuffle = new(
                commandName: "shuffle",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);

                    //* shuffle text
                    var chars = text.ToCharArray();
                    var random = new Random();
                    for (int i = chars.Length - 1; i > 0; i--) {
                        int r = random.Next(i + 1);
                        var tmp = chars[i];
                        chars[i] = chars[r];
                        chars[r] = tmp;
                    }

                    var answer = new string(chars);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                }
            );

            FormattableCommand fraction = new(
                commandName: "fraction",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    Dictionary<char, string[]> fractionDict = new Dictionary<char, string[]> {
                        { '0', new string[] {"⁰", "₀"} },
                        { '1', new string[] {"¹", "₁"} },
                        { '2', new string[] {"²", "₂"} },
                        { '3', new string[] {"³", "₃"} },
                        { '4', new string[] {"⁴", "₄"} },
                        { '5', new string[] {"⁵", "₅"} },
                        { '6', new string[] {"⁶", "₆"} },
                        { '7', new string[] {"⁷", "₇"} },
                        { '8', new string[] {"⁸", "₈"} },
                        { '9', new string[] {"⁹", "₉"} },
                        { '=', new string[] {"⁼", "₌"} },
                        { '+', new string[] {"⁺", "₊"} },
                        { '-', new string[] {"⁻", "₋"} },
                        { '(', new string[] {"⁽", "₍"} },
                        { ')', new string[] {"⁾", "₎"} },
                        { 'a', new string[] {"ᵃ", "ₐ"} },
                        { 'b', new string[] {"ᵇ", "failed"} },
                        { 'c', new string[] {"ᶜ", "failed"} },
                        { 'd', new string[] {"ᵈ", "failed"} },
                        { 'e', new string[] {"ᵉ", "ₑ"} },
                        { 'f', new string[] {"ᶠ", "failed"} },
                        { 'g', new string[] {"ᵍ", "failed"} },
                        { 'h', new string[] {"ʰ", "ₕ"} },
                        { 'i', new string[] {"ⁱ", "ᵢ"} },
                        { 'j', new string[] {"ʲ", "ⱼ"} },
                        { 'k', new string[] {"ᵏ", "ₖ"} },
                        { 'l', new string[] {"ˡ", "ₗ"} },
                        { 'm', new string[] {"ᵐ", "ₘ"} },
                        { 'n', new string[] {"ⁿ", "ₙ"} },
                        { 'o', new string[] {"ᵒ", "ₒ"} },
                        { 'p', new string[] {"ᵖ", "ₚ"} },
                        { 'r', new string[] {"ʳ", "ᵣ"} },
                        { 's', new string[] {"ˢ", "ₛ"} },
                        { 't', new string[] {"ᵗ", "ₜ"} },
                        { 'u', new string[] {"ᵘ", "ᵤ"} },
                        { 'v', new string[] {"ᵛ", "ᵥ"} },
                        { 'w', new string[] {"ʷ", "failed"} },
                        { 'x', new string[] {"ˣ", "ₓ"} },
                        { 'y', new string[] {"ʸ", "failed"} },
                        { 'z', new string[] {"ᶻ", "failed"} },
                    };

                    string[] slashSplit = text.Split("/");
                    string numerator = slashSplit[0];
                    string denominator = slashSplit[1];

                    foreach (char x in numerator) {
                        if (fractionDict.ContainsKey(x)) {
                            string i = fractionDict[x][0];
                            converted.Add(i);
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Something went wrong.",
                            "Either the code is broken, or you did not input the parameters correctly.",
                            "4"
                                }
                            );
                            return null;
                        }
                    }

                    converted.Add("⁄");

                    foreach (char x in denominator) {
                        if (fractionDict.ContainsKey(x)) {
                            string i = fractionDict[x][1];
                            if (i != "failed") {
                                converted.Add(i);
                            } else {
                                Utils.NotifCheck(
                                    true,
                                    new string[] {
                                "Hey!",
                                @"It seems you tried to input a character that's not supported.",
                                "4"
                                    }
                                );
                                return null;
                            }
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Something went wrong.",
                            "Either the code is broken, or you did not input the parameters correctly.",
                            "4"
                                }
                            );
                            return null;
                        }
                    }

                    string fraction = string.Join("", converted);
                    Utils.CopyCheck(copy, fraction);
                    Utils.NotifCheck(
                        notif, new string[] { "Success!", "Message copied to clipboard.", "3" }
                    );
                    return fraction;
                },
                aliases: new string[] { "fc" }
            );

            FormattableCommand gzip = new(
                commandName: "gzip",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string mode = args[1];
                    string text = string.Join(" ", args[2..]);
                    if (mode == "to") {
                        try {
                            string compressed = GZip.Compress(text);

                            Utils.CopyCheck(copy, compressed);
                            Utils.NotifCheck(
                                notif, new string[] { "Success!", "Message copied to clipboard.", "2" }
                            );
                            return compressed;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Something went wrong.",
                            "An error occured when trying to convert your text to GZip.",
                            "4"
                                }
                            );
                            return null;
                        }
                    } else if (mode == "from") {
                        try {
                            string decompressed = GZip.Decompress(text);

                            Utils.CopyCheck(copy, decompressed);
                            Utils.NotifCheck(
                                notif, new string[] { "Success!", $"The text was: {decompressed}", "2" }
                            );
                            return decompressed;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                            "Something went wrong.",
                            "An error occured when trying to decompress your text from GZip to ASCII.",
                            "4"
                                }
                            );
                            return null;
                        }
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Huh.",
                                "It seems you did not input a valid mode.",
                                "4"
                            }
                        ); return null;
                    }
                },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand hcf = new(
                commandName: "hcf",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args);

                    List<int> numsInt = Utils.RegexFindAllInts(text);
                    List<System.Numerics.BigInteger> numsBigIntegers = new();

                    foreach (int num in numsInt) {
                        numsBigIntegers.Add(num);
                    }

                    try {
                        if (numsInt.Count > 1) {
                            System.Numerics.BigInteger answer =
                            HCF.findGCD(
                                numsBigIntegers.ToArray<System.Numerics.BigInteger>(),
                                numsBigIntegers.ToArray().Length
                            );
                            Utils.CopyCheck(copy, answer.ToString());
                            Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                            return answer.ToString();
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Something went wrong.", "You need to input at least two numbers.", "4"
                                }
                            );
                            return null;
                        }
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Huh.",
                                "It seems you did not input the numbers properly. Try 'hcf 15 70' as an example.",
                                "8"
                            }
                        );

                        return null;
                    }
                },
                aliases: new string[] { "gcd" }
            );

            FormattableCommand average = new(
                commandName: "average",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<int> nums = Utils.RegexFindAllInts(text);

                    if (nums.Count > 1) {
                        //* find sum of all nums in "nums"
                        int sum = 0;
                        foreach (int num in nums) {
                            sum += num;
                        }

                        // find average of sum
                        int average = sum / nums.Count;

                        Utils.CopyCheck(copy, average.ToString());
                        Utils.NotifCheck(notif, new string[] { "Success!", $"The average was {average}", "5" });
                        return average.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Something went wrong.",
                                "You need to input at least two numbers.",
                                "4"
                            }
                        );
                        return null;
                    }
                },
                aliases: new string[] { "avg" }
            );

            FormattableCommand hexadecimal = new(
                commandName: "hexadecimal",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string[] textList = text.Split(" ");
                    string hexWithDash = string.Join("-", textList);

                    if (Hex.IsHex(string.Join("", args[1..]))) {
                        try {
                            string textFromHex = System.Text.Encoding.ASCII.GetString(Hex.toText(hexWithDash));
                            Utils.CopyCheck(copy, textFromHex);
                            Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {textFromHex}", "10" });
                            return textFromHex;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Something went wrong.",
                                    "Something went wrong when trying to convert your text from hexadecimal",
                                    "4"
                                }
                            );
                            return null;
                        }
                    } else {
                        try {
                            string hexFromText = Hex.toHex(text);
                            Utils.CopyCheck(copy, hexFromText);
                            Utils.NotifCheck(notif, new string[] { "Success!", $"Message copied to clipboard.", "3" });
                            return hexFromText;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Something went wrong.",
                                    "Something went wrong when trying to convert your text into hexadecimal.",
                                    "4"
                                }
                            );
                            return null;
                        }
                    }
                },
                aliases: new string[] { "hex" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand ascii = new(
                commandName: "ascii",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    Func<string, string> toAscii = (string text) => {
                        List<string> ascii = new();
                        foreach (char i in text) {
                            ascii.Add(((int)i).ToString());
                        }

                        return string.Join(" ", ascii);
                    };

                    Func<string, List<int>, string> fromAscii = (string ascii, List<int> nums) => {
                        List<string> chars = new();
                        foreach (int i in nums) {
                            chars.Add(((char)i).ToString());
                        }

                        return string.Join("", chars);
                    };

                    Action<bool, bool, string> notifAndCopy = (bool copy, bool notif, string fromAsciiText) => {
                        Utils.CopyCheck(copy, fromAsciiText);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    };

                    string text = string.Join(" ", args[1..]);
                    if (Utils.FormatValid("0123456789 ", text)) {
                        List<int> values = Utils.RegexFindAllInts(text);
                        List<bool> valuesAreValid = new();
                        foreach (int i in values) {
                            if (i.ToString().Length == 2 | i.ToString().Length == 3) {
                                valuesAreValid.Add(true);
                            } else {
                                valuesAreValid.Add(false);
                            }
                        }

                        if (!valuesAreValid.Contains(false)) {
                            string fromAsciitext = fromAscii(string.Join(" ", values), values);

                            Utils.CopyCheck(copy, fromAsciitext);
                            Utils.NotifCheck(
                                notif,
                                new string[] { "Success!", $"The message was: {fromAsciitext}", "6" }
                            ); return fromAsciitext;
                        } else {
                            string ascii = toAscii(text);
                            notifAndCopy(copy, notif, ascii);
                            return ascii;
                        }
                    } else {
                        string ascii = toAscii(text);
                        notifAndCopy(copy, notif, ascii);
                        return ascii;
                    }
                },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand lcm = new(
                commandName: "lcm",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);

                    List<int> numsInt = Utils.RegexFindAllInts(text);
                    List<BigInteger> numsBigInteger = new();

                    foreach (int num in numsInt) {
                        numsBigInteger.Add(num);
                    }

                    try {
                        if (numsInt.Count > 1) {
                            BigInteger answer =
                            LCMClass.lcmExec(numsBigInteger.ToArray<BigInteger>());
                            Utils.CopyCheck(copy, answer.ToString());
                            Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                            return answer.ToString();
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Something went wrong.",
                                    "You need to input at least two numbers.",
                                    "4"
                                }
                            );
                            return null;
                        }
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Huh.",
                        "It seems you did not input a number. Try 'lcm 15 70' as an example.",
                        "8"
                            }
                        );
                        return null;
                    }
                }
            );

            FormattableCommand length = new(
                commandName: "length",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    string len = $@"Character count: {text.Length.ToString()}
Word count: {args[1..].Length}";
                    Utils.NotifCheck(notif, new string[] { "Success!", len, "5" });
                    return len;
                },
                aliases: new string[] { "len" }
            );

            FormattableCommand characterDistribution = new(
                commandName: "characterdistribution",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    HashSet<char> uniqueChars = new();
                    foreach (char c in text) { uniqueChars.Add(c); }

                    Dictionary<char, string> charDistrDict = new();
                    foreach (char i in uniqueChars) { charDistrDict.Add(i, $"{i}: {text.Count(f => (f == i))}\n"); }

                    List<char> firstLetters = charDistrDict.Keys.ToList();
                    firstLetters.Sort();
                    List<string> charDistr = new();

                    foreach (var i in firstLetters) { charDistr.Add(charDistrDict[i]); }
                    string result = string.Join("", charDistr);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        new string[] {
                            "Success!", "The character distribution has been copied to your clipboard.", "3"
                        }
                    ); return result;
                },
                aliases: new string[] { "chardistr", "chardistribution", "characterdistr" }
            );

            FormattableCommand lowercase = new(
                commandName: "lowercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    string lowerText = text.ToLower();
                    Utils.CopyCheck(copy, lowerText);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return lowerText;
                },
                aliases: new string[] { "lower" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand mathitalic = new(
                commandName: "mathitalic",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    Dictionary<string, string> mathitalicChar = new() {
                        { "a", "𝑎" },
                        { "b", "𝑏" },
                        { "c", "𝑐" },
                        { "d", "𝑑" },
                        { "e", "𝑒" },
                        { "f", "𝑓" },
                        { "g", "𝑔" },
                        { "h", "ℎ" },
                        { "i", "𝑖" },
                        { "j", "𝑗" },
                        { "k", "𝑘" },
                        { "l", "𝑙" },
                        { "m", "𝑚" },
                        { "n", "𝑛" },
                        { "o", "𝑜" },
                        { "p", "𝑝" },
                        { "q", "𝑞" },
                        { "r", "𝑟" },
                        { "s", "𝑠" },
                        { "t", "𝑡" },
                        { "u", "𝑢" },
                        { "v", "𝑣" },
                        { "w", "𝑤" },
                        { "x", "𝑥" },
                        { "y", "𝑦" },
                        { "z", "𝑧" },
                        { "A", "𝐴" },
                        { "B", "𝐵" },
                        { "C", "𝐶" },
                        { "D", "𝐷" },
                        { "E", "𝐸" },
                        { "F", "𝐹" },
                        { "G", "𝐺" },
                        { "H", "𝐻" },
                        { "I", "𝐼" },
                        { "J", "𝐽" },
                        { "K", "𝐾" },
                        { "L", "𝐿" },
                        { "M", "𝑀" },
                        { "N", "𝑁" },
                        { "O", "𝑂" },
                        { "P", "𝑃" },
                        { "Q", "𝑄" },
                        { "R", "𝑅" },
                        { "S", "𝑆" },
                        { "T", "𝑇" },
                        { "U", "𝑈" },
                        { "V", "𝑉" },
                        { "W", "𝑊" },
                        { "X", "𝑋" },
                        { "Y", "𝑌" },
                        { "Z", "𝑍" },
                    };

                    foreach (char d in text) {
                        var replaced = mathitalicChar.GetValueOrDefault(d.ToString(), "");
                        if (replaced != "") {
                            converted.Add(replaced!);
                        } else {
                            converted.Add(d.ToString());
                        }
                    }

                    var answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                aliases: new string[] { "mai" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand morse = new(
                commandName: "morse",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]).ToLower();
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    if (Utils.FormatValid("-./ ", text)) {
                        return Morse.toText(text, copy, notif);
                    } else {
                        return Morse.toMorse(text, copy, notif);
                    }
                },
                aliases: new string[] { "morsecode" },
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand divide = new(
                commandName: "divide",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<int> ints = Utils.RegexFindAllInts(text);

                    int dividedNum = ints[0] / ints[1]; int remainder = ints[0] % ints[1];

                    Func<string, string> returnNum = (string ans) => {
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(notif, new string[] { "Success!", ans, "5" });
                        return ans;
                    };

                    if (remainder != 0 && ints.Count > 1) {
                        return returnNum($"Answer: {dividedNum} and Remainder: {remainder}");
                    } else if (remainder == 0) {
                        return returnNum($"Answer: {dividedNum}");
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] { "Something went wrong.", "You need to input atleast two numbers.", "3" }
                        );
                        return null;
                    }
                }
            );

            FormattableCommand percentage = new(
                commandName: "percentage",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    //* making regex
                    Regex findNumberFromPercentage = new(@"(?<percent>\d+(\.\d+)?)% of (?<number>\d+(\.\d+)?)");
                    Regex findPercentageFromNumbers = new(@"get (?<num1>\d+|\d+\.\d+) and (?<num2>\d+|\d+\.\d+)");

                    if (findNumberFromPercentage.IsMatch(text)) {
                        MatchCollection matches = findNumberFromPercentage.Matches(text);
                        float percent = (float.Parse(matches[0].Groups["percent"].Value));
                        float number = float.Parse(matches[0].Groups["number"].Value);

                        float ans = (percent / 100) * number; //* answer
                        Utils.NotifCheck(
                            notif,
                            new string[] {
                                "Success!",
                                $"{ans} is {percent}% of {number}",
                                "5"
                            }
                        );

                        Utils.CopyCheck(copy, ans.ToString());
                        return ans.ToString();
                    } else if (findPercentageFromNumbers.IsMatch(text)) {
                        MatchCollection matches = findPercentageFromNumbers.Matches(text);
                        float num1 = float.Parse(matches[0].Groups["num1"].Value);
                        float num2 = float.Parse(matches[0].Groups["num2"].Value);

                        float ans = (num1 / num2) * 100; //* answer
                        Utils.NotifCheck(notif,
                            new string[] {
                                "Success!",
                                $"{num1} is {ans}% of {num2}",
                                "5"
                            }
                        );
                        Utils.CopyCheck(copy, ans.ToString());
                        return ans.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Huh.",
                                "It seems you did not input the parameters correctly.",
                                "3"
                            }
                        );
                        return null;
                    }
                },
                aliases: new string[] { "percent", "%" }
            );

            FormattableCommand randchar = new(
                commandName: "randchar",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string[] asciiCharacters = {
                    "a", "b", "c", "d", "e",
                    "f", "g", "h", "i", "j",
                    "k", "l", "m", "n", "o", "p",
                    "q", "r", "s", "t", "u", "v",
                    "w", "x", "y", "z", "A", "B",
                    "C", "D", "E", "F", "G", "H",
                    "I", "J", "K", "L", "M", "N",
                    "O", "P", "Q", "R", "S", "T",
                    "U", "V", "W", "X", "Y", "Z"
                };

                    string text = string.Join(" ", args[1..]);

                    //* testing if text is a number
                    try {
                        int.Parse(text);
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Something went wrong.",
                        "Either the number you entered was not a number, or it was too large.",
                        "5"
                            }
                        );
                    }

                    Random rand = new Random();
                    List<string> randomChar = new();

                    foreach (int i in Enumerable.Range(1, int.Parse(text))) {
                        randomChar.Add(asciiCharacters[rand.Next(0, asciiCharacters.Length - 1)]);
                    }

                    string ans = string.Join("", randomChar);
                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Text copied to clipboard.", "3" });
                    return ans;
                }
            );

            FormattableCommand randint = new(
                commandName: "randint",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);
                    List<int> nums = Utils.RegexFindAllInts(text);

                    if (nums.Count > 1) {
                        //* quick check to see if the first num is greater than second
                        if (nums[0] > nums[1]) {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                "Huh.",
                                "Unfortunately the minimum value of the random number cannot be higher than the max value.",
                                "5"
                                }
                            );
                            return null;
                        }

                        Random rand = new Random();
                        int randint = rand.Next(nums[0], nums[1] + 1);

                        Utils.CopyCheck(copy, randint.ToString());
                        Utils.NotifCheck(notif, new string[] { "Success!", $"The number was: {randint}", "5" });
                        return randint.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Are you sure those were numbers?",
                                "Something went wrong while getting the numbers from the parameters you inputted.",
                                "4"
                            }
                        );
                        return null;
                    }
                },
                aliases: new string[] { "randnum" }
            );

            FormattableCommand reverse = new(
                commandName: "reverse",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    char[] textArray = text.ToCharArray();
                    List<char> textList = new();
                    foreach (char ch in textArray) {
                        textList.Add(ch);
                    }
                    textList.Reverse();
                    string answer = string.Join("", textList);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand sarcasm = new(
                commandName: "sarcasm",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    char currentCase = 'u';
                    foreach (char i in text) {
                        string iStr = i.ToString();
                        if (currentCase == 'u') {
                            converted.Add(iStr.ToUpper());
                            currentCase = 'l';
                        } else if (currentCase == 'l') {
                            converted.Add(iStr.ToLower());
                            currentCase = 'u';
                        }
                    }
                    string sarcasmText = string.Join("", converted);
                    Utils.CopyCheck(copy, sarcasmText);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return sarcasmText;
                }
            );

            FormattableCommand sha1 = new(
                commandName: "sha1",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    System.Text.StringBuilder Sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            Sb.Append(b.ToString("x2"));
                    }

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return Sb.ToString();
                }
            );

            FormattableCommand sha256 = new(
                commandName: "sha256",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    System.Text.StringBuilder Sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            Sb.Append(b.ToString("x2"));
                    }

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return Sb.ToString();
                }
            );

            FormattableCommand sha384 = new(
                commandName: "sha384",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    System.Text.StringBuilder Sb = new System.Text.StringBuilder();

                    using (
                        System.Security.Cryptography.SHA384 hash = System.Security.Cryptography.SHA384.Create()
                    ) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            Sb.Append(b.ToString("x2"));
                    }

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return Sb.ToString();
                }
            );

            FormattableCommand sha512 = new(
                commandName: "sha512",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    System.Text.StringBuilder Sb = new System.Text.StringBuilder();

                    using (System.Security.Cryptography.SHA512 hash = System.Security.Cryptography.SHA512.Create()) {
                        System.Text.Encoding enc = System.Text.Encoding.UTF8;
                        Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                        foreach (Byte b in result)
                            Sb.Append(b.ToString("x2"));
                    }

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return Sb.ToString();
                }
            );

            FormattableCommand spacer = new(
                commandName: "spacer",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    foreach (char i in text) {
                        converted.Add(i.ToString());
                        converted.Add(" ");
                    }
                    string answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand spoilerspam = new(
                commandName: "spoilerspam",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    foreach (char i in text) {
                        converted.Add($"||{i}");
                    }
                    string answer = $"{string.Join("||", converted)}||";
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                }
            );

            FormattableCommand title = new(
                commandName: "titlecase",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]).ToLower();

                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    System.Globalization.TextInfo textInfo =
                        new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    string ans = textInfo.ToTitleCase(string.Join(" ", text));

                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return ans;
                },
                aliases: new string[] { "title" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand uppercase = new(
                commandName: "uppercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    string upperText = text.ToUpper();
                    Utils.CopyCheck(copy, upperText);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return upperText;
                },
                aliases: new string[] { "upper" },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand camelcase = new(
                commandName: "camelcase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    Func<string, string> output = (string result) => {
                        Utils.CopyCheck(copy, result);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return result;
                    };

                    List<string> ans = new();
                    ans.Add(args[1].ToLower());

                    try {
                        var test = args[2];
                        foreach (string i in args[2..]) {
                            ans.Add(Utils.Capitalise(i));
                        }
                    } catch (IndexOutOfRangeException) {
                        return output(ans[0]);
                    }

                    return output(string.Join("", ans));
                }
            );

            FormattableCommand snakecase = new(
                commandName: "snakecase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join("_", args[1..]).ToLower();
                    Utils.CopyCheck(copy, text);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return text;
                }
            );

            FormattableCommand piglatin = new(
                commandName: "piglatin",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    List<string> pigLatin = new();
                    foreach (string word in args[1..]) {
                        if (
                            word.StartsWith("a")
                            | word.StartsWith("e")
                            | word.StartsWith("i")
                            | word.StartsWith("o")
                            | word.StartsWith("u")
                        ) {
                            pigLatin.Add(word + "ay");
                        } else {
                            List<string> lettersX = word[1..].Split().ToList(); //* all letters of word except the first
                            string firstLetter = word[0].ToString(); //* first letter of word

                            //* add first letter and "ay" to end of word
                            lettersX.Add(firstLetter); lettersX.Add("ay");

                            //* join letters together
                            pigLatin.Add(string.Join("", lettersX));
                        }
                    }

                    string result = string.Join(" ", pigLatin);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return result;
                }
            );
        }
    }
}
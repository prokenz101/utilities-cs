using System.Numerics;
using System.Text.RegularExpressions;

namespace utilities_cs {
    /// <summary>
    /// The hierarchy of all command-classes for all commands in utilities-cs.
    /// </summary>
    public class Command {
        /// <summary>
        /// The primary name of the command.
        /// </summary>
        public string? CommandName { get; set; }

        /// <summary>
        /// A command's aliases.
        /// </summary>
        public string[]? Aliases { get; set; }

        /// <summary>
        /// A dictionary of command names to methods (For FormattableCommands).
        /// </summary>
        public static Dictionary<string, Func<string[], bool, bool, string?>> FCommands = new();

        /// <summary>
        /// A dictionary of command names to methods (For RegularCommands).
        /// </summary>
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
                        "4"
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

        /// <summary>
        /// Gets the Method of a Formattable OR Regular Command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>Returns the method of the formattable/regular command.</returns>
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
        /// <summary>
        /// The function that will be executed when the command is called.
        /// </summary>
        public Func<string[], bool, bool, string?>? Function;

        /// <summary>
        /// Denotes whether this specific command will be used in the all command.
        /// </summary>
        public bool UseInAllCommand;

        /// <summary>
        /// If UseInAllCommand is true, then this denotes what all-command-mode the command will be used in.
        /// </summary>
        public string? AllCommandMode;

        /// <summary>
        /// List of all registered FormattableCommands.
        /// </summary>
        public static List<FormattableCommand> FormattableCommands = new();

        /// <summary>
        /// Constructor for FormattableCommands.
        /// </summary>
        /// <param name="commandName">The commandName for the FormattableCommand</param>
        /// <param name="function">The function for the FormattableCommand</param>
        /// <param name="aliases">The aliases for the FormattableCommand</param>
        /// <param name="useInAllCommand">The useInAllCommand for the FormattableCommand</param>
        /// <param name="allCommandMode">The allCommandMode for the FormattableCommand</param>
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
                FormattableCommands.ForEach(i => {
                    if (i.UseInAllCommand && i.AllCommandMode == mode) {
                        methodsSupportedByAll.Add(i);
                    }
                });
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

        /// <summary>
        /// Returns a FormattableCommand using the name of that command.
        /// </summary>
        /// <param name="cmd">The name of the command.</param>
        /// <returns>A FormattableCommand based on the "cmd" that is passed.</returns>
        public static FormattableCommand? GetFormattableCommand(string cmd) {
            foreach (FormattableCommand i in FormattableCommands) {
                if (i.CommandName == cmd) {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a FormattableCommand exists using the name of its name.
        /// </summary>
        /// <param name="cmd">The name of the command.</param>
        /// <returns>True if the command exists, else false.</returns>
        public static bool FCommandExists(string cmd) {
            if (FCommands.ContainsKey(cmd)) {
                return true;
            } else {
                return false;
            }
        }
    }

    /// <summary>
    /// The class that supports regular commands.
    /// </summary>
    public class RegularCommand : Command {
        public Action<string[]>? Function;
        public static List<RegularCommand> RegularCommands = new();
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

            RegularCommands.Add(this);
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

        /// <summary>
        /// Gets a RegularCommand using the name of that command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>An instance of the RegularCommand class, or null.</returns>
        public static RegularCommand? GetRegularCommand(string commandName) {
            foreach (RegularCommand i in RegularCommands!) {
                if (i.CommandName == commandName) {
                    return i;
                }
            }

            return null;
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

            RegularCommand send = new(
                commandName: "send",
                Send.SendMain
            );

            RegularCommand settings = new(
                commandName: "settings",
                function: SettingsModification.SettingsMain
            );

            RegularCommand force = new(
                commandName: "force",
                function: Force.ForceMain
            );

            RegularCommand unforce = new(
                commandName: "unforce",
                function: Force.UnforceMain,
                aliases: new string[] { "un-force" }
            );

            RegularCommand format = new(
                commandName: "format",
                function: Format.Formatter
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
                                true,
                                new string[] { v.ToString(), $"The factorial is: {v.ToString()}", "5" }
                            ); return v.ToString();
                        });
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] { "Huh.", "It seems that the number you inputted was not a number.", "4" }
                        ); return;
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

            RegularCommand help = new(
                commandName: "help",
                function: (string[] args) => {
                    const string wikiLink = "https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki";
                    var process = new System.Diagnostics.ProcessStartInfo("cmd", $"/c start {wikiLink}");
                    process.CreateNoWindow = true;

                    if (args.Length > 0) {
                        string searchQuery = args[1].ToLower();
                        if (Command.Exists(searchQuery)) {
                            string commandName;
                            if (RegularCommand.GetRegularCommand(searchQuery) != null) {
                                commandName =
                                    RegularCommand.GetRegularCommand(searchQuery)!.CommandName!;
                            } else if (FormattableCommand.GetFormattableCommand(searchQuery) != null) {
                                commandName =
                                    FormattableCommand.GetFormattableCommand(searchQuery)!.CommandName!;
                            } else {
                                //* Cannot possibly run.
                                commandName = "Impossible.";
                            }

                            process.Arguments = process.Arguments += $"#{commandName}";
                            Utils.NotifCheck(
                                true,
                                new string[] { "Opening wiki...", $"Opening wiki for \"{commandName}\"", "3" }
                            );

                            System.Diagnostics.Process.Start(process);
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] { "Huh.", "It seems that command does not exist.", "3" }
                            );
                        }
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] { "Opening wiki...", "Opening wiki in your default browser.", "3" }
                        );

                        System.Diagnostics.Process.Start(process);
                    }
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
                commandName: "gs",
                function: (string[] args) => {
                    string searchQuery = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://google.com/search?q={searchQuery}"
                    ) { CreateNoWindow = true });
                }
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
                All.AllCommand
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
                aliases: new string[] { "b32" }
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
                aliases: new string[] { "base85", "b85" }
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

                    foreach (char b in text) {
                        var replaced = Dictionaries.BubbleDict.GetValueOrDefault(b.ToString(), "");
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

                    if (Dictionaries.CopypasteDict.ContainsKey(text)) {
                        Utils.CopyCheck(copy, Dictionaries.CopypasteDict[text]);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return Dictionaries.CopypasteDict[text];
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

                    foreach (char cr in text) {
                        var replaced = Dictionaries.CreepyDict.GetValueOrDefault(cr.ToString(), "");
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

                    foreach (char i in text) {
                        if (Dictionaries.WingdingsDict.ContainsKey(i.ToString())) {
                            converted.Add(Dictionaries.WingdingsDict[i.ToString()]);
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

                    foreach (char c in text) {
                        var replaced = Dictionaries.CursiveDict.GetValueOrDefault(c.ToString(), "");
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

                    foreach (char d in text) {
                        var replaced = Dictionaries.DoublestruckDict.GetValueOrDefault(d.ToString(), "");
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

                    foreach (char i in text) {
                        if (Utils.FormatValid(
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                           i.ToString()
                        )) {
                            converted.Add($":regional_indicator_{i.ToString().ToLower()}:");
                        } else if (Dictionaries.EmojifySpecialCharDict.ContainsKey(i.ToString())) {
                            converted.Add(Dictionaries.EmojifySpecialCharDict[i.ToString()]);
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
                        { "E", "3" }, { "I", "1" }, { "O", "0" }, { "A", "4" }, { "S", "5" }
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
                },
                aliases: new string[] { "loremipsum" }
            );

            FormattableCommand exponent = new(
                commandName: "exponent",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();

                    foreach (char e in text) {
                        var replaced = Dictionaries.ExponentDict.GetValueOrDefault(e.ToString(), "");
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

                    foreach (char f in text) {
                        var replaced = Dictionaries.FlipDict.GetValueOrDefault(f.ToString(), "");
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

                    string[] slashSplit = text.Split("/");
                    string numerator = slashSplit[0];
                    string denominator = slashSplit[1];

                    foreach (char x in numerator) {
                        if (Dictionaries.FractionDict.ContainsKey(x)) {
                            string i = Dictionaries.FractionDict[x][0];
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
                        if (Dictionaries.FractionDict.ContainsKey(x)) {
                            string i = Dictionaries.FractionDict[x][1];
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
                }
            );

            FormattableCommand hcf = new(
                commandName: "hcf",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args);
                    List<System.Numerics.BigInteger> nums = new();
                    Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

                    try {
                        if (nums.Count > 1) {
                            System.Numerics.BigInteger answer =
                            HCF.findGCD(
                                nums.ToArray<System.Numerics.BigInteger>(),
                                nums.ToArray().Length
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

                    List<BigInteger> nums = new();
                    Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

                    try {
                        if (nums.Count > 1) {
                            BigInteger answer =
                            LCMClass.lcmExec(nums.ToArray<BigInteger>());
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

                    foreach (char d in text) {
                        var replaced = Dictionaries.MathItalicDict.GetValueOrDefault(d.ToString(), "");
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
                                    "Unfortunately the minimum value cannot be higher than the max value.",
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
                    List<char> textList = text.ToCharArray().ToList();

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
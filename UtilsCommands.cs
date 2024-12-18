using System.Text.RegularExpressions;

namespace utilities_cs {
    /// <summary>
    /// The base class of all command-classes for all commands in utilities-cs.
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
        public static Dictionary<string, Func<string[], bool, bool, string?>> FCommands = [];

        /// <summary>
        /// A dictionary of command names to methods (For RegularCommands).
        /// </summary>
        public static Dictionary<string, Action<string[]>> RCommands = [];
        /// <summary>
        /// Executes a command in either the RCommands dictionary or the FCommands dictionary.
        /// </summary>
        /// <param name="cmd">The name of the command to be excuted.</param>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        /// <returns>A string of the output of the command. This can also be null.</returns>
        public static string? ExecuteCommand(string[] args, bool copy = true, bool notif = true) {
            string cmd = args[0].ToLower();

            if (FCommands.TryGetValue(cmd, out var fcommand)) {
                string? output = fcommand.Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else if (RCommands.TryGetValue(cmd, out var rcommand)) {
                rcommand.Invoke(args);
                return null;
            } else if (Force.AreAnyForced()) {
                args = Enumerable.Concat(["cmd"], args).ToArray<string>();
                string? output = Force.forced!.Function!.Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else {
                Utils.NotifCheck(
                    true,
                    [
                        "Exception", "Invalid command, try 'help' for more info.", "4"
                    ], "executeCommandError"
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
            if (Exists(commandName)) {
                if (FCommands.TryGetValue(commandName, out var fname)) {
                    return fname;
                } else if (RCommands.TryGetValue(commandName, out var rname)) {
                    return rname;
                } else {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all the aliases for a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>A list of all the aliases, or null if the command does not exist.</returns>
        public static List<string>? GetAliases(string commandName) {
            if (FCommands.TryGetValue(commandName, out var fname)) {
                var aliases = FCommands.Where(kvp => kvp.Value == fname)
                    .Select(kvp => kvp.Key)
                    .ToList();

                return aliases;
            } else if (RCommands.TryGetValue(commandName, out var rname)) {
                var aliases = RCommands.Where(kvp => kvp.Value == rname)
                    .Select(kvp => kvp.Key)
                    .ToList();

                return aliases;
            }

            return null;
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
        public static List<FormattableCommand> FormattableCommands = [];

        /// <summary>
        /// Initializes a new instance of a FormattableCommand.
        /// </summary>
        /// <param name="commandName">The commandName for the FormattableCommand.</param>
        /// <param name="function">The function for the FormattableCommand.</param>
        /// <param name="aliases">The aliases for the FormattableCommand.</param>
        /// <param name="useInAllCommand">Denotes whether the command should be included in the all command.</param>
        /// <param name="allCommandMode">The mode for the all command that the command is to be included in.</param>
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
            } else { FCommands.Add(commandName, function); }

            FormattableCommands.Add(this);
        }

        /// <summary>
        /// A non-static command that allows you to execute a command immediately.
        /// </summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        /// <param name="copy">Controls whether the function is willing to copy text to the clipboard.</param>
        /// <param name="notif">Controls whether the function is willing to send a notification.</param>
        //! Mostly unused method. Only used for testing purposes.
        public string? Execute(string[] args, bool copy, bool notif) {
            if (Function != null) {
                string? output = Function.Invoke(args, copy, notif);
                if (output != null) { Console.WriteLine(output); return output; }
            }

            return null;
        }

        /// <summary>
        /// Lists all currently registered FormattableCommands.
        /// </summary>
        /// <returns>A string with all currently registered Commands, seperated by newlines.</returns>
        public static string ListAllFCommands() {
            List<string> fCommandsList = [];
            foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> i in FCommands) {
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
            if (FCommands.TryGetValue(cmd, out var fcommand)) {
                string? output = fcommand.Invoke(args, copy, notif);
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
            List<FormattableCommand> methodsSupportedByAll = [];

            FormattableCommands?.ForEach(
                i => { if (i.UseInAllCommand && i.AllCommandMode == mode) { methodsSupportedByAll.Add(i); } }
            );

            return methodsSupportedByAll;
        }

        /// <summary>
        /// Returns a FormattableCommand using the name of that command.
        /// </summary>
        /// <param name="cmd">The name of the command that is used to find the method and return it.</param>
        /// <returns>The method of that command name.</returns>
        public static Func<string[], bool, bool, string?>? GetFMethod(string cmd) {
            if (FCommands.TryGetValue(cmd, out var fcommand)) {
                Func<string[], bool, bool, string?> func = fcommand;
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
                } else if (i.Aliases != null) {
                    if (i.Aliases.Any(x => x == cmd)) { return i; }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a FormattableCommand exists using the name of its name.
        /// </summary>
        /// <param name="cmd">The name of the command.</param>
        /// <returns>True if the command exists, else false.</returns>
        public static bool FormattableCommandExists(string cmd) { return FCommands.ContainsKey(cmd); }
    }

    /// <summary>
    /// The class that supports regular commands.
    /// </summary>
    public class RegularCommand : Command {
        public Action<string[]>? Function;
        public static List<RegularCommand> RegularCommands = [];
        /// <summary>
        /// Initializes a new instance of a RegularCommand.
        /// </summary>
        /// <param name="commandName">The name of the regular command.</param>
        /// <param name="function">The function to be run.</param>
        /// <param name="aliases">The aliases for the command.</param>
        public RegularCommand(string commandName, Action<string[]> function, string[]? aliases = null) {
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
            List<string> rCommandsList = [];
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
                } else if (i.Aliases != null) {
                    if (i.Aliases.Any(x => x == commandName)) {
                        return i;
                    }
                }
            }

            return null;
        }

        /// <summary>A non-static method that executes a command immediately.</summary>
        /// <param name="args">The command arguments to be used when executing the command.</param>
        //! Mostly unused method. Only used for testing purposes.
        public void Execute(string[] args) {
            Function?.Invoke(args);
        }
    }

    /// <summary>
    /// The class containing all methods that are used for registering commands to the dictionaries.
    /// </summary>
    public partial class RegisterCommands {

        [GeneratedRegex(@"(?<root>-?\d+\.\d+|-?\d+)(?:st|nd|rd|th) root of (?<num>-?\d+\.\d+|-?\d+)")]
        private static partial Regex RootRegex();

        [GeneratedRegex(@"[""'](?<old>.+)[""'] with [""'](?<new>.+|)[""'] in [""'](?<text>.+)[""']")]
        private static partial Regex ReplaceRegex();

        [GeneratedRegex(@"(?<percent>-?\d+\.\d+|-?\d+)% of (?<number>-?\d+\.\d+|-?\d+)")]
        private static partial Regex FindNumberFromPercentageRegex();

        [GeneratedRegex(@"get (?<num1>-?\d+\.\d+|-?\d+) and (?<num2>-?\d+\.\d+|-?\d+)")]
        private static partial Regex FindPercentageFromNumbersRegex();

        /// <summary>
        /// The method that registers all regular commands.
        /// </summary>
        public static void RegisterAllRCommands() {
            RegularCommand autoclick = new(
                commandName: "autoclick",
                Autoclick.Autoclicker
            );

            RegularCommand send = new(
                commandName: "send",
                Send.SendMain
            );

            RegularCommand spam = new(
                commandName: "spam",
                Spam.SpamMain
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
                aliases: ["un-force"]
            );

            RegularCommand format = new(
                commandName: "format",
                function: Format.FormatMain
            );

            RegularCommand update = new(
                commandName: "update",
                function: (string[] args) => {
                    if (Utils.IndexTest(args)) { return; }

                    if (args[1] == "check") {
                        Update.Check();
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid mode, try 'help' for more info.", "3"],
                            "updateError"
                        );
                    }
                },
                aliases: ["updates"]
            );

            RegularCommand exit = new(
                commandName: "exit",
                function: (string[] args) => {
                    HookManager.UnregisterAllHooks();
                    Application.Exit();
                },
                aliases: ["quit"]
            );

            RegularCommand help = new(
                commandName: "help",
                function: (string[] args) => {
                    const string wikiLink = "https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki";
                    var process = new System.Diagnostics.ProcessStartInfo("cmd", $"/c start {wikiLink}")
                    { CreateNoWindow = true };

                    if (Utils.IndexTest(args, sendNotif: false)) {
                        System.Diagnostics.Process.Start(process);
                        Utils.NotifCheck(
                            true,
                            ["Opening wiki...", "Opening wiki in your default browser.", "3"],
                            "wikiOpen"
                        ); return;
                    } else {
                        string searchQuery = args[1].ToLower();
                        if (Command.Exists(searchQuery)) {
                            string commandName =
                                RegularCommand.GetRegularCommand(searchQuery) != null
                                    ? RegularCommand.GetRegularCommand(searchQuery)!.CommandName!
                                : FormattableCommand.GetFormattableCommand(searchQuery) != null
                                    ? FormattableCommand.GetFormattableCommand(searchQuery)!.CommandName!
                                : "Impossible";

                            process.Arguments = process.Arguments += $"#{commandName}";
                            Utils.NotifCheck(
                                true,
                                ["Opening wiki...", $"Opening wiki for \"{commandName}\"", "3"],
                                "wikiOpen"
                            ); System.Diagnostics.Process.Start(process);
                        } else {
                            Utils.NotifCheck(
                                true,
                                [ "Exception", @"Invalid command.
Opening wiki anyway...", "3" ],
                                "wikiError"
                            );

                            System.Diagnostics.Process.Start(process);
                        }
                    }
                },
                aliases: ["wiki"]
            );

            RegularCommand notification = new(
                commandName: "notification",
                function: (string[] args) => {
                    string text = string.Join(" ", args[1..]);

                    Dictionary<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection>?
                        matchToGroups =
                            Utils.RegexFind(
                                text,
                                @"[""'](?<title>.*?)[""'],? [""'](?<subtitle>.*?)[""'],? (?<duration>\d+)",
                                useIsMatch: true,
                                () => {
                                    Utils.NotifCheck(
                                        true,
                                        ["Exception", "Invalid parameters, try 'help' for more info.", "3"],
                                        "notificationCommandError"
                                    );
                                }
                            );

                    if (matchToGroups != null) {
                        foreach (
                            KeyValuePair<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection>
                                kvp in matchToGroups
                        ) {
                            System.Text.RegularExpressions.GroupCollection groups = kvp.Value;

                            string title = groups["title"].ToString();
                            string subtitle = groups["subtitle"].ToString();
                            int duration = int.Parse(groups["duration"].ToString());

                            Utils.NotifCheck(
                                true,
                                [title, subtitle, duration.ToString()],
                                "notificationCommandSuccess"
                            ); return;
                        }
                    }
                },
                aliases: ["notify", "notif"]
            );

            RegularCommand remind = new(
                commandName: "remind",
                function: (string[] args) => {
                    string text = string.Join(" ", args[1..]);

                    Dictionary<System.Text.RegularExpressions.Match, System.Text.RegularExpressions.GroupCollection>? matchToGroups =
                        Utils.RegexFind(
                            text,
                            @"(?<time>\d+)(?<unit>h|m|s)(?<text> .*)?",
                            useIsMatch: true,
                            () => {
                                Utils.NotifCheck(
                                    true,
                                    ["Exception", "Invalid parameters, try 'help' for more info.", "3"],
                                    "remindCommandError"
                                );
                            }
                        );

                    if (matchToGroups != null) {
                        List<int> timeEnumerable = [];
                        List<char> unitEnumerable = [];
                        List<string> textEnumerable = [];

                        foreach (
                            KeyValuePair<
                                System.Text.RegularExpressions.Match,
                                System.Text.RegularExpressions.GroupCollection
                            > kvp in matchToGroups
                        ) {
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

                        if (timeOptions.ContainsKey(unit)) {
                            int multiplier = int.Parse(timeOptions[unit][0]);
                            string word = timeOptions[unit][1].ToString();
                            int timeSeconds = time * 1000 * multiplier;

                            Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder customReminderToast =
                                new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                    .AddText("Reminder!");

                            string timeEquals1 = $"You set a reminder for 1 {word}.";
                            string timeNotEqualTo1 = $"You set a reminder for {time} {word}s.";
                            string timeWithMessage = $"Your reminder was: {reminderText}";

                            if (time == 1 && reminderText == string.Empty) {
                                customReminderToast.AddText(timeEquals1);
                            } else if (reminderText == string.Empty) {
                                customReminderToast.AddText(timeNotEqualTo1);
                            } else {
                                customReminderToast.AddText(timeWithMessage);
                            }

                            customReminderToast.AddButton(
                                new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                    .SetContent("Dismiss")
                                    .AddArgument("remind", "dismiss")
                                    .SetBackgroundActivation()
                            );

                            if (timeSeconds > 10000) {
                                Utils.NotifCheck(
                                    true,
                                    [
                                        "New reminder added.",
                                        $"A reminder will come in {timeSeconds / 1000} seconds.",
                                        "4"
                                    ], "remindCommandInfo"
                                );
                            }

                            customReminderToast.SetToastScenario(
                                Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Alarm
                            ); Task.Delay(timeSeconds).Wait();

                            Utils.NotifCheck(customReminderToast, "reminder", clearToast: false);
                        }
                    }
                },
                aliases: ["reminder"]
            );

            RegularCommand googleSearch = new(
                commandName: "gs",
                function: (string[] args) => {
                    string url = System.Web.HttpUtility.UrlEncode(string.Join(" ", args[1..]));

                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(
                            "cmd", $"/c start https://google.com/search?q={url}"
                        ) { CreateNoWindow = true }
                    );
                }
            );

            RegularCommand youtubeSearch = new(
                commandName: "youtube",
                function: (string[] args) => {
                    string url = System.Web.HttpUtility.UrlEncode(string.Join(" ", args[1..]));

                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(
                            "cmd", $"/c start https://youtube.com/results?search_query={url}"
                        ) { CreateNoWindow = true }
                    );
                },
                aliases: ["yt"]
            );

            RegularCommand imageSearch = new(
                commandName: "images",
                function: (string[] args) => {
                    string url = System.Web.HttpUtility.UrlEncode(string.Join(" ", args[1..]));

                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(
                            "cmd", $"/c start https://www.google.com/search?tbm=isch&q={url}"
                        ) { CreateNoWindow = true }
                    );
                }
            );

            RegularCommand translate = new(
                commandName: "translate",
                function: Translate.TranslateMain
            );

            RegularCommand getcommandcount = new(
                commandName: "getcommandcount",
                function: (string[] args) => {
                    int regularCommandsCount = RegularCommand.RegularCommands.Count;
                    int formattableCommandsCount = FormattableCommand.FormattableCommands.Count;

                    Utils.NotifCheck(
                        true,
                        [
                            $"Total Commands: {regularCommandsCount + formattableCommandsCount}",
                            $@"RegularCommands Count: {regularCommandsCount}
FormattableCommands Count: {formattableCommandsCount}",
                            "5"
                        ], "getcommandcountSuccess"
                    );
                },
                aliases: ["totalcommandcount", "get-commandcount"]
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

            FormattableCommand getAliases = new(
                commandName: "aliases",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string cmd = args[1];
                    List<string>? aliases = Command.GetAliases(cmd);

                    if (aliases != null) {
                        string aliasesString = string.Join(", ", aliases);

                        Utils.CopyCheck(copy, aliasesString);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "The aliases were copied to your clipboard.", "3"],
                            "getAliasesSuccess"
                        ); return aliasesString;
                    } else {
                        Utils.NotifCheck(
                            false,
                            ["No aliases found for command: " + cmd],
                            "getAliasesError"
                        ); return null;
                    }
                },
                aliases: ["getaliases", "getalias", "get-alias", "get-aliases"]
            );

            FormattableCommand escape = new(
                commandName: "escape",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    string ans = Utils.BulkReplace(
                        text,
                        "! @ # $ % ^ & * ( ) _ + , . / ; ' [ ] < > ? : \" { } ` ~ \\",
                        "\\" + string.Join(" \\", "! @ # $ % ^ & * ( ) _ + , . / ; ' [ ] < > ? : \" { } ` ~ \\".Split(" "))
                    );

                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "escapeSuccess"
                    ); return ans;
                }
            );

            FormattableCommand base32 = new(
                commandName: "base32",
                function: Base32Convert.Base32ConvertMain,
                aliases: ["b32"]
            );

            FormattableCommand base64 = new(
                commandName: "base64",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    Func<string, bool> isBase64 = (string s) => {
                        s = s.Trim();
                        bool isB64 = (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(
                            s, @"^[a-zA-Z0-9\+/]*={0,3}$",
                            System.Text.RegularExpressions.RegexOptions.None
                        ); return isB64;
                    };

                    string text = string.Join(" ", args[1..]);
                    if (isBase64(text)) {
                        try {
                            string ans = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(text));
                            Utils.CopyCheck(copy, ans);
                            Utils.NotifCheck(
                                notif,
                                ["Success!", $"The message was: {ans}", "6"],
                                "base64Success"
                            ); return ans;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                ["Exception", "Something went wrong while converting this text to Base64.", "4"],
                                "base64Error"
                            ); return null;
                        }
                    } else {
                        string ans = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "The message was copied to your clipboard.", "3"],
                            "base64Success"
                        ); return ans;
                    }
                },
                aliases: ["b64"],
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand isBase64 = new(
                commandName: "isbase64",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);

                    Func<string, bool> isBase64 = (string s) => {
                        s = s.Trim();
                        bool isB64 = (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(
                            s, @"^[a-zA-Z0-9\+/]*={0,3}$",
                            System.Text.RegularExpressions.RegexOptions.None
                        ); return isB64;
                    };

                    if (isBase64(text)) {
                        Utils.NotifCheck(notif, ["Yes.", "The string is Base64.", "3"], "isBase64Success");
                        return "Yes";
                    } else {
                        Utils.NotifCheck(notif, ["No.", "The string is not Base64.", "3"], "isBase64Success");
                        return "No";
                    }
                }
            );

            FormattableCommand base85 = new(
                commandName: "base85",
                function: Ascii85.Base85Main,
                aliases: ["ascii85", "b85"]
            );

            FormattableCommand urlencode = new(
                commandName: "urlencode",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    string url = System.Web.HttpUtility.UrlEncode(text);

                    Utils.CopyCheck(copy, url);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "The URL was copied to your clipboard.", "2"],
                        "urlEncodeSuccess"
                    ); return url;
                }
            );

            FormattableCommand urldecode = new(
                commandName: "urldecode",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    string url = System.Web.HttpUtility.UrlDecode(text);

                    Utils.CopyCheck(copy, url);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "The URL was copied to your clipboard.", "2"],
                        "urlDecodeSuccess"
                    ); return url;
                }
            );

            FormattableCommand binary = new(
                commandName: "binary",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    if (!Utils.FormatValid("01 ", text)) {
                        byte[] ConvertToByteArray(string str, System.Text.Encoding encoding) {
                            return encoding.GetBytes(str);
                        }

                        string ToBinary(byte[] data) {
                            return string.Join(
                                " ",
                                data.Select(
                                    byt => Convert.ToString(byt, 2).PadLeft(8, '0')
                                )
                            );
                        }

                        string ans = ToBinary(ConvertToByteArray(text, System.Text.Encoding.ASCII));
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "Message copied to clipboard.", "3"],
                            "binarySuccess"
                        ); return ans;

                    } else {
                        try {
                            string[] textList = text.Split(" ");
                            var chars = from split in textList select ((char)Convert.ToInt32(split, 2)).ToString();

                            Utils.CopyCheck(copy, string.Join("", chars));
                            Utils.NotifCheck(
                                notif,
                                ["Success!", $"The message was: {string.Join("", chars)}", "10"],
                                "binarySuccess"
                            ); return string.Join("", chars);
                        } catch {
                            Utils.NotifCheck(
                                true,
                                ["Exception", @"Something went wrong while converting this text to binary.", "3"],
                                "binaryError"
                            ); return null;
                        }
                    }
                },
                aliases: ["bin"],
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand bubbletext = new(
                commandName: "bubbletext",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.BubbleDict);
                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "Message copied to clipboard.", "3"],
                        "bubbletextSuccess"
                    ); return result;
                },
                aliases: ["bubble"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand commaseperator = new(
                commandName: "commaseperator",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string input = string.Join(" ", args[1..]);
                    System.Text.RegularExpressions.Regex re = new(@"(?<num>-?\d+)(?:\.(?<decimals>\d+))?");

                    if (re.IsMatch(input)) {
                        System.Numerics.BigInteger num =
                            System.Numerics.BigInteger.Parse(re.Match(input).Groups["num"].Value);

                        System.Numerics.BigInteger decimals =
                            re.Match(input).Groups["decimals"].Value != ""
                                ? System.Numerics.BigInteger.Parse(re.Match(input).Groups["decimals"].Value)

                            : 0;

                        string result =
                            decimals == 0 ? string.Format("{0:n0}", num)
                            : string.Format("{0:n0}", num) + "." + decimals.ToString();

                        Utils.CopyCheck(copy, result);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "Message copied to clipboard.", "3"],
                            "commaseperatorSuccess"
                        ); return result;
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "2"],
                            "commaseperatorError"
                        ); return null;
                    }
                },
                aliases: ["cms"]
            );

            FormattableCommand copypaste = new(
                commandName: "copypaste",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);

                    if (Dictionaries.CopypasteDict.TryGetValue(text, out var val)) {
                        Utils.CopyCheck(copy, val);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "Message copied to clipboard.", "3"],
                            "copypasteSuccess"
                        ); return Dictionaries.CopypasteDict[text];
                    } else {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception",
                                "Invalid input, try 'help' for more info.",
                                "3"
                            ], "copypasteError"
                        ); return null;
                    }
                },
                aliases: ["cp"]
            );

            FormattableCommand creepy = new(
                commandName: "creepy",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.CreepyDict);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "Message copied to clipboard.", "3"],
                        "creepySuccess"
                    ); return result;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand wingdings = new(
                commandName: "wingdings",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.WingdingsDict);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "Message copied to clipboard.", "3"],
                        "wingdingsSuccess"
                    ); return result;
                },
                aliases: ["wd"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand exponent = new(
                commandName: "exponent",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.ExponentDict);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, ["Success!", "Message copied to clipboard.", "3"], "epSuccess");
                    return result;
                },
                aliases: ["ep"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand subscript = new(
                commandName: "subscript",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]).ToLower(), Dictionaries.SubscriptDict);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "subscriptSuccess"
                    ); return result;
                },
                aliases: ["sub"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand factorial = new(
                commandName: "factorial",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    try {
                        System.Numerics.BigInteger n = System.Numerics.BigInteger.Parse(args[1]);
                        System.Numerics.BigInteger i = 1;
                        System.Numerics.BigInteger v = 1;
                        while (i <= n) { v *= i; i += 1; }

                        Utils.CopyCheck(copy, v.ToString());
                        Utils.NotifCheck(
                            notif,
                            [v.ToString(), $"The factorial is: {v.ToString()}", "5"],
                            "factorialSuccess"
                        ); return v.ToString();
                    } catch {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "4"],
                            "factorialError"
                        ); return null;
                    }
                }
            );

            FormattableCommand raise = new(
                commandName: "raise",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    bool cancel = false;

                    var matchToGroups =
                        Utils.RegexFind(
                            text,
                            @"(?<base>-?\d+\.\d+|-?\d+) to (?<power>-?\d+\.\d+|-?\d+)",
                            useIsMatch: true,
                            () => {
                                Utils.NotifCheck(
                                    true,
                                    ["Exception", "Invalid syntax, try 'help' for more info.", "4"],
                                    "raiseError"
                                ); cancel = true;
                            }
                        );

                    if (cancel) { return null; }

                    if (matchToGroups != null) {
                        foreach (var kvp in matchToGroups) {
                            try {
                                System.Numerics.BigInteger result = 1;
                                System.Numerics.BigInteger baseNum =
                                    System.Numerics.BigInteger.Parse(kvp.Key.Groups["base"].Value);

                                System.Numerics.BigInteger powerNum =
                                    System.Numerics.BigInteger.Parse(kvp.Key.Groups["power"].Value);

                                for (System.Numerics.BigInteger i = 0; i < powerNum; i += 1) {
                                    result *= baseNum;
                                }

                                Utils.CopyCheck(copy, result.ToString());
                                Utils.NotifCheck(
                                    notif,
                                    [
                                      "Success!", "The result was copied to your clipboard.", "3"
                                    ], "raiseSuccess"
                                ); return result.ToString();
                            } catch (OverflowException) {
                                Utils.NotifCheck(
                                    true, ["Exception", "Inputted values exceed computational limits.", "3"],
                                    "raiseError"
                                ); return null;
                            }
                        }
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid parameters, try 'help' for more info."],
                            "raiseError"
                        ); return null;
                    }

                    return null;
                }
            );

            FormattableCommand root = new(
                commandName: "root",
                function: (string[] args, bool copy, bool notif) => {
                    if (args[1] == "calc" | args[1] == "calculator") {
                        string text = string.Join(" ", args[2..]);
                        var re = RootRegex();

                        if (re.IsMatch(text)) {
                            var match = re.Match(text);
                            try {
                                double root = Convert.ToDouble(match.Groups["root"].Value);
                                double num = Convert.ToDouble(match.Groups["num"].Value);
                                double answer = Utils.RoundIfNumberIsNearEnough(Math.Pow(num, 1 / root));

                                Utils.CopyCheck(copy, answer.ToString());
                                Utils.NotifCheck(
                                    notif,
                                    ["Success!", "Number copied to clipboard.", "3"],
                                    "rootSuccess"
                                ); return answer.ToString();
                            } catch (FormatException) {
                                Utils.NotifCheck(
                                    true,
                                    ["Exception", "Invalid input, try 'help' for more info.", "3"],
                                    "rootError"
                                ); return null;
                            } catch (OverflowException) {
                                Utils.NotifCheck(
                                    true,
                                    ["Exception", "Inputted value exceeds computational limits.", "3"],
                                    "rootError"
                                ); return null;
                            }
                        } else {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception", "Invalid syntax, try 'help' for more info.", "3"
                                ], "rootError"
                            ); return null;
                        }
                    } else if (args[1] == "get") {
                        try {
                            System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(args[2]);
                            void notifAndCopy(string result) {
                                Utils.CopyCheck(copy, result);
                                Utils.NotifCheck(
                                    notif,
                                    ["Success!", "The root was copied to clipboard.", "3"],
                                    "rootSuccess"
                                );
                            }

                            if (num == 2) { notifAndCopy("√"); return "√"; }

                            string? exp =
                                exponent.Execute(["exponent", num.ToString()], false, false);

                            if (exp != null) {
                                string result = $"{exp}√";
                                notifAndCopy(result);
                                return result;
                            } else {
                                Utils.NotifCheck(
                                    true,
                                    [
                                        "Exception", "Invalid input, try 'help' for more info.", "3"
                                    ], "rootError"
                                ); return null;
                            }
                        } catch (FormatException) {
                            Utils.NotifCheck(
                                true,
                                ["Exception", "Invalid input, try 'help' for more info.", "3"],
                                "rootError"
                            ); return null;
                        }
                    } else {
                        Utils.NotifCheck(
                            true, ["Exception", "Invalid mode, try 'help' for more info.", "3"], "rootError"
                        ); return null;
                    }
                }
            );

            FormattableCommand cuberoot = new(
                commandName: "cuberoot",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    try {
                    //* checking if there are commas in the number
                    if (text.Contains(',')) {
                        text = text.Replace(",", string.Empty);
                    }

                    double num = Convert.ToDouble(text);
                    string result = Math.Pow(num, (double)1 / 3).ToString();

                    result = Utils.RoundIfNumberIsNearEnough(Convert.ToDouble(result)).ToString();

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", $"The answer is: {result}", "4"],
                        "cuberootSuccess"
                    ); return result;
                    } catch (FormatException) {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info."],
                            "cuberootError"
                        ); return null;
                    } catch (OverflowException) {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Inputted value exceeds computational limits.", "3"],
                            "rootError"
                        ); return null;
                    }
                },
                aliases: ["cbrt"]
            );

            FormattableCommand cursive = new(
                commandName: "cursive",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.CursiveDict);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "cursiveSuccess"
                    ); return result;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand doublestruck = new(
                commandName: "doublestruck",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.DoublestruckDict);
                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "doublestruckSuccess"
                    ); return result;
                },
                aliases: ["dbs"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand emojify = new(
                commandName: "emojify",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);

                    if (Utils.IndexTest(args)) { return null; }

                    List<string> converted = [];

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
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "emojifySuccess"
                    ); return string.Join(" ", converted);
                }
            );

            FormattableCommand leet = new(
                commandName: "leet",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    Dictionary<string, string> leetChar = new() {
                        { "E", "3" }, { "I", "1" }, { "O", "0" }, { "A", "4" }, { "S", "5" }
                    };

                    string result = Utils.TextFormatter(string.Join(" ", args[1..]).ToUpper(), leetChar);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(notif, ["Success!", "Message copied to clipboard.", "3"], "leetSuccess");
                    return result;
                },
                aliases: ["numberize", "numberise"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand pi = new(
                commandName: "pi",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    try {
                        int digits = int.Parse(args[1]);
                        string result;

                        if (digits <= 0) {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    "Cannot have a negative or zero amount of pi digits.", "3"
                                ], "piError"
                            ); return null;
                        } else if (digits <= 1000) {
                            result = "3." + PiDigits.piDigits[0..digits];
                        } else {
                            string calculatePi(System.Numerics.BigInteger digits) {
                                digits++;

                                uint[] x = new uint[(int)digits * 10 / 3 + 2];
                                uint[] r = new uint[(int)digits * 10 / 3 + 2];

                                uint[] pi = new uint[(int)digits];

                                for (int j = 0; j < x.Length; j++) { x[j] = 20; }

                                for (int i = 0; i < digits; i++) {
                                    uint carry = 0;
                                    for (int j = 0; j < x.Length; j++) {
                                        uint num = (uint)(x.Length - j - 1);
                                        uint dem = num * 2 + 1;

                                        x[j] += carry;

                                        uint q = x[j] / dem;
                                        r[j] = x[j] % dem;

                                        carry = q * num;
                                    }

                                    pi[i] = x[x.Length - 1] / 10;
                                    r[x.Length - 1] = x[x.Length - 1] % 10; ;
                                    for (int j = 0; j < x.Length; j++) { x[j] = r[j] * 10; }
                                }

                                var result = "";
                                uint c = 0;

                                for (int i = pi.Length - 1; i >= 0; i--) {
                                    pi[i] += c;
                                    c = pi[i] / 10;

                                    result = (pi[i] % 10).ToString() + result;
                                }

                                return result;
                            } result = "3." + calculatePi(digits)[1..];
                        }

                        Utils.CopyCheck(copy, result);
                        Utils.NotifCheck(
                            notif, ["Success!", $"The result is: {result}", "4"], "piSuccess"
                        ); return result;

                    } catch (FormatException) {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception", "Invalid input, try 'help' for more info.", "5"
                            ], "piError"
                        ); return null;
                    } catch {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception", "Something went wrong.", "4"
                            ], "piError"
                        ); return null;
                    }
                }
            );

            FormattableCommand lorem = new(
                commandName: "lorem",
                function: LoremIpsum.LoremMain,
                aliases: ["loremipsum"]
            );

            FormattableCommand flip = new(
                commandName: "flip",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = [];

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
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "flipSuccess"
                    ); return answer;
                },
                aliases: ["flipped", "upside-down"],
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
                        (chars[r], chars[i]) = (chars[i], chars[r]);
                    }

                    var answer = new string(chars);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "shuffleSuccess"
                    ); return answer;
                }
            );

            FormattableCommand permutations = new(
                commandName: "permutations",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    char[] textAsCharArray = string.Join(" ", args[1..]).ToCharArray();

                    //* checks if textAsCharArray is than the permutationsCalculationLimit
                    int limit = UtilitiesAppContext.CurrentSettings.PermutationsCalculationLimit;

                    if (textAsCharArray.Length <= limit) {
                        Permutations permutation = new();
                        permutation.GetPer(textAsCharArray);
                        HashSet<string> hashSetAnswer = permutation.Permutation;

                        string answer = string.Join("\n", hashSetAnswer);
                        Utils.CopyCheck(copy, answer);
                        Utils.NotifCheck(
                            notif,
                            [
                                "Success!", "The permutations were copied to your clipboard.", "3"
                            ], "permutationsSuccess"
                        ); return answer;
                    } else {
                        Utils.NotifCheck(
                            true,
                            [
                                "Error!",
                                $"For performance reasons, permutation calculations can only be used on {limit} or less characters.",
                                "3"
                            ], "permutationsError"
                        ); return null;
                    }
                },
                aliases: ["getpermutations", "get-permutations"]
            );

            FormattableCommand fraction = new(
                commandName: "fraction",
                function: Fractions.FractionsMain,
                aliases: ["fc"]
            );

            FormattableCommand gzip = new(
                commandName: "gzip",
                function: GZip.GZipMain
            );

            FormattableCommand hcf = new(
                commandName: "hcf",
                function: HCF.HCFMain,
                aliases: ["gcd"]
            );

            FormattableCommand factors = new(
                commandName: "factors",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    Func<System.Numerics.BigInteger, List<System.Numerics.BigInteger>> findFactors =
                        (System.Numerics.BigInteger num) => {
                            List<System.Numerics.BigInteger> factors = [];

                            for (System.Numerics.BigInteger i = 1; i < num; i++) {
                                if (num % i == 0) {
                                    factors.Add(i);
                                }
                            }

                            return factors;
                        };

                    System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(args[1]);
                    List<System.Numerics.BigInteger> factors = findFactors(num);

                    string ans = string.Join(", ", factors);
                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The factors are: {ans}.", "5"], "factorsSuccess"
                    ); return ans;
                },
                aliases: ["factorise"]
            );

            FormattableCommand primefactors = new(
                commandName: "primefactors",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    try {
                        System.Numerics.BigInteger number = System.Numerics.BigInteger.Parse(args[1]);

                        List<int> factors = [];
                        int divisor = 2;

                        while (number > 1) {
                            if (number % divisor == 0 && factors.Find(x => x % divisor == 0 && x != divisor) == 0) {
                                number /= divisor;
                                factors.Add(divisor);
                            } else {
                                divisor++;
                            }
                        }

                        string ans = string.Join("×", factors);

                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            true, ["Prime Factorization: ", ans, "5"], "primeFactorsSuccess"
                        ); return ans;
                    } catch (FormatException) {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception", "Invalid input, try 'help' for more info.", "3"
                            ],
                            "primeFactorsError"
                        ); return null;
                    }
                },
                aliases: ["primefactorise"]
            );

            FormattableCommand average = new(
                commandName: "average",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<System.Numerics.BigInteger> nums = Utils.RegexFindAllInts(text);

                    if (nums.Count > 1) {
                        //* find sum of all nums in "nums"
                        System.Numerics.BigInteger sum = 0;
                        foreach (System.Numerics.BigInteger num in nums) {
                            sum += num;
                        }

                        //* find average of sum
                        System.Numerics.BigInteger average = sum / nums.Count;

                        Utils.CopyCheck(copy, average.ToString());
                        Utils.NotifCheck(
                            notif, ["Success!", $"The average was {average}", "5"], "averageSuccess"
                        ); return average.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "4"],
                            "averageError"
                        );
                        return null;
                    }
                },
                aliases: ["avg"]
            );

            FormattableCommand hexadecimal = new(
                commandName: "hexadecimal",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    string fromTextToHex(string text) {
                        byte[] ba = System.Text.Encoding.Default.GetBytes(text);
                        var hexString = BitConverter.ToString(ba);
                        hexString = hexString.Replace("-", " ");
                        hexString = hexString.ToLower();

                        return hexString;
                    }

                    byte[] fromHexToText(string hex) {
                        hex = hex.Replace("-", "");
                        byte[] raw = new byte[hex.Length / 2];
                        for (int i = 0; i < raw.Length; i++) {
                            raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }

                        return raw;
                    }

                    bool isHex(IEnumerable<char> chars) {
                        bool isHex;
                        foreach (var c in chars) {
                            isHex = (c >= '0' && c <= '9') ||
                                    (c >= 'a' && c <= 'f') ||
                                    (c >= 'A' && c <= 'F');

                            if (!isHex)
                                return false;
                        }

                        return true;
                    }

                    string[] textList = text.Split(" ");
                    string hexWithDash = string.Join("-", textList);

                    if (isHex(string.Join("", args[1..]))) {
                        try {
                            string textFromHex = System.Text.Encoding.ASCII.GetString(fromHexToText(hexWithDash));
                            Utils.CopyCheck(copy, textFromHex);
                            Utils.NotifCheck(
                                notif, ["Success!", $"The message was: {textFromHex}", "10"], "hexSuccess"
                            ); return textFromHex;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    "Something went wrong while trying to convert your text from hexadecimal.",
                                    "4"
                                ],
                                "hexadecimalError"
                            ); return null;
                        }
                    } else {
                        try {
                            string hexFromText = fromTextToHex(text);
                            Utils.CopyCheck(copy, hexFromText);
                            Utils.NotifCheck(
                                notif, ["Success!", $"Message copied to clipboard.", "3"], "hexSuccess"
                            ); return hexFromText;
                        } catch {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    "Something went wrong while trying to convert your text into hexadecimal.",
                                    "4"
                                ],
                                "hexadecimalError"
                            ); return null;
                        }
                    }
                },
                aliases: ["hex"],
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand ascii = new(
                commandName: "ascii",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string toAscii(string text) {
                        List<string> ascii = [];
                        foreach (char i in text) {
                            ascii.Add(((int)i).ToString());
                        }

                        return string.Join(" ", ascii);
                    }

                    string fromAscii(string ascii, List<int> nums) {
                        List<string> chars = [];
                        foreach (int i in nums) {
                            chars.Add(((char)i).ToString());
                        }

                        return string.Join("", chars);
                    }

                    void notifAndCopy(bool copy, bool notif, string fromAsciiText) {
                        Utils.CopyCheck(copy, fromAsciiText);
                        Utils.NotifCheck(
                            notif, ["Success!", "Message copied to clipboard.", "3"], "asciiSuccess"
                        );
                    }

                    string text = string.Join(" ", args[1..]);
                    if (Utils.FormatValid("0123456789 ", text)) {
                        List<int> values = [];
                        try {
                            Utils.RegexFindAllInts(text).ForEach(x => values.Add((int)x));
                        } catch (OverflowException) {
                            Utils.NotifCheck(
                                true,
                                ["Exception", "Number too large", "3"],
                                "asciiError"
                            ); return null;
                        }

                        List<bool> valuesAreValid = [];
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
                                ["Success!", $"The message was: {fromAsciitext}", "6"],
                                "asciiSuccess"
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
                function: LCMClass.LCMMain
            );

            FormattableCommand length = new(
                commandName: "length",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    string len = $@"Character count: {text.Length}
Word count: {args[1..].Length}";

                    Utils.CopyCheck(copy, len);
                    Utils.NotifCheck(notif, ["Success!", len, "5"], "lengthSuccess");
                    return len;
                },
                aliases: ["len"]
            );

            FormattableCommand characterDistribution = new(
                commandName: "characterdistribution",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    HashSet<char> uniqueChars = [.. text];

                    Dictionary<char, string> charDistrDict = [];
                    uniqueChars.ToList().ForEach(
                        i => charDistrDict.Add(i, $"{i}: {text.Count(f => f == i)}\n")
                    );

                    List<char> firstLetters = [.. charDistrDict.Keys];
                    firstLetters.Sort();
                    List<string> charDistr = [];

                    foreach (var i in firstLetters) { charDistr.Add(charDistrDict[i]); }
                    string result = string.Join("", charDistr);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif,
                        [
                            "Success!", "The character distribution has been copied to your clipboard.", "3"
                        ],
                        "characterDistributionSuccess"
                    ); return result;
                },
                aliases: ["chardistr", "chardistribution", "characterdistr"]
            );

            FormattableCommand replace = new(
                commandName: "replace",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    var re = ReplaceRegex();

                    if (re.IsMatch(text)) {
                        var match = re.Match(text);
                        var old = match.Groups["old"].Value;
                        var new_ = match.Groups["new"].Value;
                        var textToReplace = match.Groups["text"].Value;

                        string result = textToReplace.Replace(old, new_);
                        Utils.CopyCheck(copy, result);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "Message copied to clipboard.", "5"],
                            "replaceSuccess"
                        ); return null;
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid syntax, try 'help' for more info."],
                            "replaceError"
                        ); return null;
                    }
                }
            );

            FormattableCommand characterCount = new(
                commandName: "charactercount",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    bool failed = false;
                    var matchToGroups = Utils.RegexFind(
                        input: text,
                        expression: "(?<char>.) in (?<text>.+)",
                        useIsMatch: true,
                        ifNotMatch: () => {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception", "Invalid syntax, try 'help' for more info.", "3"
                                ],
                                "characterCountError"
                            );
                            failed = true;
                        }
                    );

                    if (failed) { return null; }

                    if (matchToGroups != null) {
                        foreach (var kvp in matchToGroups) {
                            char? character = kvp.Key.Groups["char"].Value.ToCharArray()[0];
                            string? textToSearch = kvp.Key.Groups["text"].Value;

                            if (character != null && textToSearch != null) {
                                int count = textToSearch.Count(f => f == character);
                                string result = $"{character}: {count}";

                                Utils.CopyCheck(copy, result);
                                Utils.NotifCheck(
                                    notif, ["Success!", result, "5"], "characterCountSuccess"
                                ); return result;
                            } else {
                                return null;
                            }
                        }

                        return null;
                    } else {
                        return null;
                    }
                },
                aliases: ["charcount"]
            );

            FormattableCommand lowercase = new(
                commandName: "lowercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string lowerText = string.Join(" ", args[1..]).ToLower();
                    Utils.CopyCheck(copy, lowerText);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "lowercaseSuccess"
                    ); return lowerText;
                },
                aliases: ["lower"]
            );

            FormattableCommand mathitalic = new(
                commandName: "mathitalic",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    var answer = Utils.TextFormatter(string.Join(" ", args[1..]), Dictionaries.MathItalicDict);

                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(
                        notif,
                        ["Success!", "Message copied to clipboard.", "3"],
                        "mathitalicSuccess"
                    ); return answer;
                },
                aliases: ["mai"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand morse = new(
                commandName: "morse",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(" ", args[1..]).ToLower();
                    if (Utils.IndexTest(args)) { return null; }

                    Func<string, bool, bool, string> toMorse = (string text, bool copy, bool notif) => {
                        List<string> morseConverted = [];

                        foreach (char t in text) {
                            if (Dictionaries.MorseToTextDict.ContainsKey(t.ToString())) {
                                morseConverted.Add(Dictionaries.MorseToTextDict[t.ToString()]);
                                morseConverted.Add(" ");
                            } else {
                                morseConverted.Add(t.ToString());
                            }
                        }

                        Utils.CopyCheck(copy, string.Join("", morseConverted));
                        Utils.NotifCheck(
                            notif,
                            ["Success!", "Message copied to clipboard.", "3"],
                            "morseSuccess"
                        ); return string.Join("", morseConverted);
                    };

                    string toText(string morse, bool copy, bool notif) {
                        List<string> convertedText = [];
                        Dictionary<string, string> morseToText = Utils.InvertKeyAndValue(Dictionaries.MorseToTextDict);
                        string[] textArray = morse.Split(" ");

                        foreach (string m in textArray) {
                            if (morseToText.ContainsKey(m.ToString())) {
                                convertedText.Add(morseToText[m.ToString()]);
                            } else {
                                convertedText.Add(m.ToString());
                            }
                        }

                        Utils.CopyCheck(copy, string.Join("", convertedText));
                        Utils.NotifCheck(
                            notif,
                            ["Success!", $"The message was: {string.Join("", convertedText)}", "7"],
                            "morseSuccess"
                        ); return string.Join("", convertedText);
                    }

                    if (Utils.FormatValid("-./ ", text)) {
                        return toText(text, copy, notif);
                    } else {
                        return toMorse(text, copy, notif);
                    }
                },
                aliases: ["morsecode"],
                useInAllCommand: true,
                allCommandMode: "encodings"
            );

            FormattableCommand reciprocal = new(
                commandName: "reciprocal",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    try {
                        double number = double.Parse(args[1]);
                        string reciprocal = Math.ReciprocalEstimate(number).ToString();

                        Utils.CopyCheck(copy, reciprocal);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", $"The reciprocal is: {reciprocal}", "3"],
                            "reciprocalSuccess"
                        ); return reciprocal;
                    } catch (OverflowException) {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Inputted numbers exceed computational limits.", "3"],
                            "reciprocalError"
                        ); return null;
                    } catch (FormatException) {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info."],
                            "reciprocalError"
                        ); return null;
                    }
                }
            );

            FormattableCommand divide = new(
                commandName: "divide",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<System.Numerics.BigInteger> ints = Utils.RegexFindAllInts(text);

                    System.Numerics.BigInteger dividedNum =
                        ints[0] / ints[1]; System.Numerics.BigInteger remainder = ints[0] % ints[1];

                    string returnNum(string ans) {
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif, ["Success!", ans, "5"], "divideSuccess"
                        ); return ans;
                    }

                    if (remainder != 0 && ints.Count > 1) {
                        return returnNum($"Answer: {dividedNum} and Remainder: {remainder}");
                    } else if (remainder == 0) {
                        return returnNum($"Answer: {dividedNum}");
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "3"],
                            "divideError"
                        );
                        return null;
                    }
                }
            );

            FormattableCommand percentage = new(
                commandName: "percentage",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    //* making regex
                    var findNumberFromPercentage = FindNumberFromPercentageRegex();
                    var findPercentageFromNumbers = FindPercentageFromNumbersRegex();

                    if (findNumberFromPercentage.IsMatch(text)) {
                        System.Text.RegularExpressions.MatchCollection matches =
                            findNumberFromPercentage.Matches(text);
                        float percent = float.Parse(matches[0].Groups["percent"].Value);
                        float number = float.Parse(matches[0].Groups["number"].Value);

                        float ans = percent / 100 * number; //* answer
                        Utils.NotifCheck(
                            notif,
                            ["Success!", $"{ans} is {percent}% of {number}", "5"],
                            "percentageSuccess"
                        );

                        Utils.CopyCheck(copy, ans.ToString());
                        return ans.ToString();
                    } else if (findPercentageFromNumbers.IsMatch(text)) {
                        System.Text.RegularExpressions.MatchCollection matches =
                            findPercentageFromNumbers.Matches(text);
                        float num1 = float.Parse(matches[0].Groups["num1"].Value);
                        float num2 = float.Parse(matches[0].Groups["num2"].Value);

                        float ans = num1 / num2 * 100; //* answer
                        Utils.NotifCheck(
                            notif,
                            ["Success!", $"{num1} is {ans}% of {num2}", "5"],
                            "percentageSuccess"
                        );
                        Utils.CopyCheck(copy, ans.ToString());
                        return ans.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid parameters, try 'help' for more info.", "3"],
                            "percentageError"
                        ); return null;
                    }
                },
                aliases: ["percent", "%"]
            );

            FormattableCommand randchar = new(
                commandName: "randchar",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string[] asciiCharacters = [
                        "a", "b", "c", "d", "e",
                        "f", "g", "h", "i", "j",
                        "k", "l", "m", "n", "o", "p",
                        "q", "r", "s", "t", "u", "v",
                        "w", "x", "y", "z", "A", "B",
                        "C", "D", "E", "F", "G", "H",
                        "I", "J", "K", "L", "M", "N",
                        "O", "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y", "Z"
                    ];

                    string text = string.Join(" ", args[1..]);

                    try {
                        Random rand = new();
                        List<string> randomChar = [];

                        foreach (int i in Enumerable.Range(1, int.Parse(text))) {
                            randomChar.Add(asciiCharacters[rand.Next(0, asciiCharacters.Length - 1)]);
                        }

                        string ans = string.Join("", randomChar);
                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(notif, ["Success!", "Text copied to clipboard.", "3"], "randcharSuccess");
                        return ans;
                    } catch (OverflowException) {
                        Utils.NotifCheck(
                            true, ["Exception", "Number was too large", "3"], "randcharError"
                        ); return null;
                    } catch {
                        Utils.NotifCheck(
                            true, ["Exception", "Something went wrong.", "3"], "randcharError"
                        ); return null;
                    }
                }
            );

            FormattableCommand randint = new(
                commandName: "randint",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<int> nums = [];
                    try {
                        Utils.RegexFindAllInts(text).ForEach(num => nums.Add((int)num));
                    } catch (OverflowException) {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Number was too large", "3"],
                            "randintError"
                        ); return null;
                    }

                    if (nums.Count > 1 && nums.Count < 3) {
                        if (nums[0] > nums[1]) {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    "Upper limit of range must be greater than lower limit.",
                                    "5"
                                ], "randintError"
                            ); return null;
                        }

                        Random rand = new();
                        int randint = rand.Next(nums[0], nums[1] + 1);

                        Utils.CopyCheck(copy, randint.ToString());
                        Utils.NotifCheck(
                            notif, ["Success!", $"The number was: {randint}", "5"], "randintSuccess"
                        ); return randint.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "4"],
                            "randintError"
                        ); return null;
                    }
                },
                aliases: ["randnum"]
            );

            FormattableCommand reverse = new(
                commandName: "reverse",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<char> textList = [.. text.ToCharArray()];

                    textList.Reverse();
                    string answer = string.Join("", textList);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "reverseSuccess"
                    ); return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand sarcasm = new(
                commandName: "sarcasm",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = [];
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
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "sarcasmSuccess"
                    ); return sarcasmText;
                }
            );

            FormattableCommand sha1 = new(
                commandName: "sha1",
                function: (string[] args, bool copy, bool notif) =>
                {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    System.Text.StringBuilder Sb = new();
                    System.Text.Encoding enc = System.Text.Encoding.UTF8;
                    byte[] result = System.Security.Cryptography.SHA1.HashData(enc.GetBytes(text));

                    foreach (byte b in result)
                        Sb.Append(b.ToString("x2"));

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "sha1Success"
                    ); return Sb.ToString();
                }
            );

            FormattableCommand sha256 = new(
                commandName: "sha256",
                function: (string[] args, bool copy, bool notif) =>
                {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    System.Text.StringBuilder Sb = new();
                    System.Text.Encoding enc = System.Text.Encoding.UTF8;
                    byte[] result = System.Security.Cryptography.SHA256.HashData(enc.GetBytes(text));

                    foreach (byte b in result)
                        Sb.Append(b.ToString("x2"));

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "sha256Success"
                    ); return Sb.ToString();
                }
            );

            FormattableCommand sha384 = new(
                commandName: "sha384",
                function: (string[] args, bool copy, bool notif) =>
                {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    System.Text.StringBuilder Sb = new();
                    System.Text.Encoding enc = System.Text.Encoding.UTF8;
                    byte[] result = System.Security.Cryptography.SHA384.HashData(enc.GetBytes(text));

                    foreach (byte b in result)
                        Sb.Append(b.ToString("x2"));

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "sha384Success"
                    ); return Sb.ToString();
                }
            );

            FormattableCommand sha512 = new(
                commandName: "sha512",
                function: (string[] args, bool copy, bool notif) =>
                {
                    string text = string.Join(" ", args[1..]);
                    if (Utils.IndexTest(args)) { return null; }

                    System.Text.StringBuilder Sb = new();
                    System.Text.Encoding enc = System.Text.Encoding.UTF8;
                    byte[] result = System.Security.Cryptography.SHA512.HashData(enc.GetBytes(text));

                    foreach (byte b in result)
                        Sb.Append(b.ToString("x2"));

                    Utils.CopyCheck(copy, Sb.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "sha512Success"
                    ); return Sb.ToString();
                }
            );

            FormattableCommand md5 = new(
                commandName: "md5",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string MD5Hasher(string input) {
                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                        byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

                        return Convert.ToHexString(hashBytes);
                    }

                    string text = string.Join(" ", args[1..]);
                    string hash = MD5Hasher(text);

                    Utils.CopyCheck(copy, hash);
                    Utils.NotifCheck(
                        notif, ["Success!", "Hash copied to clipboard.", "3"], "md5Success"
                    ); return hash;
                }
            );

            FormattableCommand spacer = new(
                commandName: "spacer",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = [];
                    foreach (char i in text) {
                        converted.Add(i.ToString());
                        converted.Add(" ");
                    }

                    string answer = string.Join("", converted);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "spacerSuccess"
                    ); return answer;
                },
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand spoilerspam = new(
                commandName: "spoilerspam",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    List<string> converted = [];
                    foreach (char i in text) {
                        converted.Add($"||{i}");
                    }

                    string answer = $"{string.Join("||", converted)}||";
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "spoilerspamSuccess"
                    ); return answer;
                }
            );

            FormattableCommand title = new(
                commandName: "titlecase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]).ToLower();
                    System.Globalization.TextInfo textInfo =
                        new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    string ans = textInfo.ToTitleCase(string.Join(" ", text));

                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "titlecaseSuccess"
                    ); return ans;
                },
                aliases: ["title"],
                useInAllCommand: true,
                allCommandMode: "fancy"
            );

            FormattableCommand uppercase = new(
                commandName: "uppercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join(" ", args[1..]);
                    string upperText = text.ToUpper();
                    Utils.CopyCheck(copy, upperText);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "uppercaseSuccess"
                    ); return upperText;
                },
                aliases: ["upper"]
            );

            FormattableCommand camelcase = new(
                commandName: "camelcase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    Func<string, string> output = (string result) => {
                        Utils.CopyCheck(copy, result);
                        Utils.NotifCheck(
                            notif, ["Success!", "Message copied to clipboard.", "3"], "camelcaseSuccess"
                        ); return result;
                    };

                    List<string> ans = [args[1].ToLower()];

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

            FormattableCommand pascalcase = new(
                commandName: "pascalcase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    List<string> output = [];
                    args[1..].ToList().ForEach(i => output.Add(Utils.Capitalise(i)));
                    string result = string.Join("", output);

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "pascalcaseSuccess"
                    ); return result;
                },
                aliases: ["pascal"]
            );

            FormattableCommand snakecase = new(
                commandName: "snakecase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    string text = string.Join("_", args[1..]).ToLower();
                    Utils.CopyCheck(copy, text);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "snakecaseSuccess"
                    ); return text;
                }
            );

            FormattableCommand piglatin = new(
                commandName: "piglatin",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) { return null; }

                    List<string> pigLatin = [];
                    foreach (string word in args[1..]) {
                        if (
                            word.StartsWith('a')
                            | word.StartsWith('e')
                            | word.StartsWith('i')
                            | word.StartsWith('o')
                            | word.StartsWith('u')
                        ) {
                            pigLatin.Add(word + "ay");
                        } else {
                            List<string> lettersX = [.. word[1..].Split()]; //* all letters of word except the first
                            string firstLetter = word[0].ToString(); //* first letter of word

                            //* add first letter and "ay" to end of word
                            lettersX.Add(firstLetter); lettersX.Add("ay");

                            //* join letters together
                            pigLatin.Add(string.Join("", lettersX));
                        }
                    }

                    Utils.CopyCheck(copy, string.Join(" ", pigLatin));
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "3"], "piglatinSuccess"
                    ); return string.Join(" ", pigLatin); ;
                }
            );
        }
    }
}
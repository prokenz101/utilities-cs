using System.Numerics;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public abstract class Command {
        public string? CommandName { get; set; }
        public string[]? Aliases { get; set; }
        public static Dictionary<string, Func<string[], bool, bool, string?>> fCommands = new();
        public static Dictionary<string, Action<string[]>> rCommands = new();
        public static string? ExecuteCommand(string cmd, string[] args) {
            if (fCommands.ContainsKey(cmd)) {
                string? output = fCommands[cmd].Invoke(args, true, true);
                if (output != null) { return output; } else { return null; }
            } else if (rCommands.ContainsKey(cmd)) {
                rCommands[cmd].Invoke(args);
                return null;
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                    "Welp.",
                    "It seems utilities couldn't understand what command you were trying to use.",
                    "6"
                    }
                );
                return null;
            }
        }
    }
    public class FormattableCommand : Command {
        public Func<string[], bool, bool, string?>? Function;
        public FormattableCommand(
            string commandName,
            Func<string[], bool, bool, string?> function,
            string[]? aliases = null
        ) {
            // setting all attributes for instance
            CommandName = commandName; Function = function; Aliases = aliases;
            if (aliases != null) {
                fCommands.Add(commandName, function);
                foreach (string alias in aliases) { fCommands.Add(alias, function); }
            } else {
                fCommands.Add(commandName, function);
            }
        }
        public void Execute(string[] args, bool copy, bool notif) {
            if (this.Function != null) {
                string? output = this.Function.Invoke(args, copy, notif);
                if (output != null) { Console.WriteLine(output); }
            }
        }

        public static string? FindAndExecute(string cmd, string[] args, bool copy, bool notif) {
            if (fCommands.ContainsKey(cmd)) {
                string? output = fCommands[cmd].Invoke(args, copy, notif);
                if (output != null) { return output; } else { return null; }
            } else {
                return null;
            }
        }

        public static Func<string[], bool, bool, string?>? GetMethod(string cmd) {
            if (fCommands.ContainsKey(cmd)) {
                Func<string[], bool, bool, string?> func = fCommands[cmd];
                return func;
            } else {
                return null;
            }
        }
    }

    public class RegularCommand : Command {
        public Action<string[]>? Function;
        public RegularCommand(
            string commandName,
            Action<string[]> function,
            string[]? aliases = null
        ) {
            // setting all attributes for instance
            CommandName = commandName; Function = function; Aliases = aliases;
            if (aliases != null) {
                rCommands.Add(commandName, function);
                foreach (string alias in aliases) { rCommands.Add(alias, function); }
            } else {
                rCommands.Add(commandName, function);
            }
        }
        public void Execute(string[] args) {
            if (this.Function != null) {
                this.Function.Invoke(args);
            }
        }
    }

    public class RegisterCommands {
        public static void RegisterAllRCommands() {
            RegularCommand autoclick = new(
                commandName: "autoclick",
                function: (string[] args) => {
                    string text = string.Join(" ", args[1..]); // parameters of autoclick, like {interval} {mousebutton} etc
                    MatchCollection matches = Autoclick.re.Matches(text.ToLower());
                    if (matches.Count > 0) {
                        var match = matches[0];
                        GroupCollection groups = match.Groups;
                        AutoclickData data = new(groups);
                        Autoclick.PerformAutoclick(data);
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Huh.",
                        "The parameters for autoclick were not given properly. Try again.",
                        "3"
                            }
                        );
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
                    string text = string.Join(' ', args[1..]);
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
                            System.Text.RegularExpressions.Group main_group = groups["command"];

                            string cmd = main_group.ToString();
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
                            $"/c start https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)"
                        ) { CreateNoWindow = true }
                    );
                }
            );

            RegularCommand notification = new(
                commandName: "notification",
                function: (string[] args) => {
                    string text = string.Join(' ', args[1..]);

                    List<Dictionary<Match, GroupCollection>>? matchToGroups = Utils.RegexFind(
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
                        Dictionary<Match, GroupCollection> match = matchToGroups[0];

                        foreach (KeyValuePair<Match, GroupCollection> kvp in match) {
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
                    string text = string.Join(' ', args[1..]);

                    List<Dictionary<Match, GroupCollection>>? list_of_dicts = Utils.RegexFind(
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

                    if (list_of_dicts != null) {

                        List<int> time_enumerable = new();
                        List<char> unit_enumerable = new();
                        List<string> text_enumerable = new();

                        foreach (Dictionary<Match, GroupCollection> dict in list_of_dicts) {
                            foreach (KeyValuePair<Match, GroupCollection> kvp in dict) {
                                time_enumerable.Add(int.Parse(kvp.Value["time"].ToString())); // float
                                unit_enumerable.Add(kvp.Value["unit"].ToString().ToCharArray()[0]); // char
                                text_enumerable.Add(kvp.Value["text"].ToString()); // string
                            }
                        }

                        int time = time_enumerable[0];
                        char unit = unit_enumerable[0];
                        string reminder_text = text_enumerable[0];

                        Dictionary<char, string[]> time_options = new() {
                            { 's', new string[] { "1", "second" } },
                            { 'm', new string[] { "60", "minute" } },
                            { 'h', new string[] { "3600", "hour" } }
                        };

                        await Task.Run(() => {  // Task for reminder.
                            if (time_options.ContainsKey(unit)) {
                                int multiplier = int.Parse(time_options[unit][0]);
                                string word = time_options[unit][1].ToString();
                                int time_seconds = (time * 1000) * multiplier;

                                Task.Delay(time_seconds).Wait();

                                if (time == 1 && reminder_text == string.Empty) {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! You set a reminder for 1 {word} and it's time!",
                                            "6"
                                        }
                                    );
                                    Console.WriteLine(
                                        $"Reminder! Hey! You set a reminder for 1 {word} and it's time! 6"
                                    );
                                } else if (reminder_text == string.Empty) {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! You set a reminder for {time} {word}s and it's time!",
                                            "6"
                                        }
                                    );
                                    Console.WriteLine(
                                        $"Reminder! Hey! You set a reminder for {time} {word}s and it's time! 6"
                                    );
                                } else {
                                    Utils.NotifCheck(
                                        true,
                                        new string[] {
                                            "Reminder!",
                                            $"Hey! Your reminder was: {reminder_text}",
                                            "6"
                                        }
                                    ); Console.WriteLine("Reminder! Hey! Your reminder was: {reminder_text} 6");
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
                    string search_query = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://google.com/search?q={search_query}"
                    ) { CreateNoWindow = true });
                },
                aliases: new string[] { "gs" }
            );

            RegularCommand youtubeSearch = new(
                commandName: "youtube",
                function: (string[] args) => {
                    string search_query = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://youtube.com/results?search_query={search_query}"
                    ) { CreateNoWindow = true });
                },
                aliases: new string[] { "yt" }
            );

            RegularCommand imageSearch = new(
                commandName: "images",
                function: (string[] args) => {
                    string search_query = string.Join("+", args[1..]);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd",
                        $"/c start https://www.google.com/search?q={search_query}^&safe=strict^&tbm=isch^&sxsrf=ALeKk029ouHDkHfq3RFVc8WpFzOvZZ8s4g%3A1624376552976^&source=hp^&biw=1536^&bih=763^&ei=6ATSYIOrOduJhbIPzda7yAs^&oq=hello^&gs_lcp=CgNpbWcQAzIFCAAQsQMyBQgAELEDMgIIADICCAAyAggAMgIIADICCAAyBQgAELEDMgUIABCxAzICCAA6BwgjEOoCECc6BAgjECc6CAgAELEDEIMBUNIGWKcJYLELaABwAHgAgAGPAogByAqSAQUwLjEuNZgBAKABAaoBC2d3cy13aXotaW1nsAEK^&sclient=img^&ved=0ahUKEwiDv62byqvxAhXbREEAHU3rDrkQ4dUDCAc^&uact=5"
                    ) { CreateNoWindow = true });
                }
            );

            RegularCommand translate = new(
                commandName: "translate",
                function: (string[] args) => {
                    string lang = args[1];
                    string text = string.Join('+', args[2..]);

                    // checking if lang is english

                    foreach (var englishLangAliases in Translate.english_dict.Keys) {
                        if (lang == englishLangAliases) {
                            Translate.toEnglish(text);
                            return;
                        }
                    }

                    // if lang is not english, then use toOtherLang()

                    foreach (var langAliases in Translate.languages.Keys) {
                        if (langAliases.Contains(lang)) {
                            Translate.toOtherLang(Translate.languages[langAliases], text);
                            break;
                        }
                    }
                }
            );
        }

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

                    string text = string.Join(' ', args[2..]);
                    string mode = args[1];

                    if (text.Contains("=")) {
                        text = text.Replace("=", string.Empty);
                    }

                    if (mode == "to") {
                        // convert text to base32
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

                    string text = string.Join(' ', args[1..]);
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
                aliases: new string[] { "b64" }
            );

            FormattableCommand binary = new(
                commandName: "binary",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(' ', args[1..]);
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
                            string[] text_list = text.Split(" ");

                            var chars = from split in text_list
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
                aliases: new string[] { "bin" }
            );

            FormattableCommand bubbletext = new(
                commandName: "bubbletext",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var bubble_char = new Dictionary<string, string>() {
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
                        var replaced = bubble_char.GetValueOrDefault(b.ToString(), "");
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
                aliases: new string[] { "bubble" }
            );

            FormattableCommand commaseperator = new(
                commandName: "commaseperator",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string str_num = string.Join(' ', args[1..]);

                    try {
                        // Checking if number is an actual number
                        System.Numerics.BigInteger.Parse(str_num);
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

                    System.Numerics.BigInteger num = System.Numerics.BigInteger.Parse(str_num);
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
                    Dictionary<string, string> cp_dict = new() {
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
                        { "->>", "↠" }
                    };

                    if (cp_dict.ContainsKey(text)) {
                        Utils.CopyCheck(copy, cp_dict[text]);
                        Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                        return cp_dict[text];
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
                    var creepy_char = new Dictionary<string, string>() {
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
                        var replaced = creepy_char.GetValueOrDefault(cr.ToString(), "");
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
                }
            );

            FormattableCommand cuberoot = new(
                commandName: "cuberoot",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(' ', args[1..]);
                    // testing if string is a double
                    try {
                        Convert.ToDouble(text);
                    } catch (FormatException) {
                        Utils.NotifCheck(true, new string[] { "Huh.", "It seems you did not input a number." });
                        return null;
                    }

                    // checking if there are commas in the number
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
                    var cursive_char = new Dictionary<string, string>() {
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
                        var replaced = cursive_char.GetValueOrDefault(c.ToString(), "");
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
                }
            );

            FormattableCommand doublestruck = new(
                commandName: "doublestruck",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var dbs_char = new Dictionary<string, string>() {
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
                        var replaced = dbs_char.GetValueOrDefault(d.ToString(), "");
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
                aliases: new string[] { "dbs" }
            );

            FormattableCommand emojify = new(
                commandName: "emojify",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(' ', args[1..]);

                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    List<string> converted = new();
                    Dictionary<string, string> special_char = new Dictionary<string, string> {
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
                        } else if (special_char.ContainsKey(i.ToString())) {
                            converted.Add(special_char[i.ToString()]);
                        } else {
                            converted.Add(i.ToString());
                        }
                    }

                    Utils.CopyCheck(copy, string.Join(" ", converted));
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return string.Join(" ", converted);
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
                    var exponents = new Dictionary<string, string>() {
                    {"-", "⁻"}, {"=", "⁼"}, {"+", "⁺"},
                    {"1", "¹"}, {"2", "²"}, {"3", "³"},
                    {"4", "⁴"}, {"5", "⁵"}, {"6", "⁶"},
                    {"7", "⁷"}, {"8", "⁸"}, {"9", "⁹"}, {"0", "⁰"},
                    {"a", "ᵃ"}, {"b", "ᵇ"}, {"c", "ᶜ"}, {"d", "ᵈ"},{ "e", "ᵉ"},
                    {"f", "ᶠ"}, {"g", "ᵍ"}, {"h", "ʰ"}, {"i", "ᶦ"},{ "j", "ʲ"},
                    {"k", "ᵏ"}, {"l", "ˡ"}, {"m", "ᵐ"}, {"n", "ⁿ"},{ "o", "ᵒ"},
                    {"p", "ᵖ"}, {"r", "ʳ"}, {"s", "ˢ"}, {"t", "ᵗ"}, {"u", "ᵘ"},
                    {"v", "ᵛ"}, {"w", "ʷ"}, {"x", "ˣ"}, {"y", "ʸ"}, {"z", "ᶻ"},
                    {"(", "⁽"}, {")", "⁾"}, {" ", " "}
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
                aliases: new string[] { "ep" }
            );

            FormattableCommand factorial = new(
                commandName: "factorial",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    try {
                        int n = int.Parse(args[1]);
                        int i = 1;
                        System.Numerics.BigInteger v = 1;

                        while (i <= n) {
                            v *= i;
                            i += 1;
                        }

                        Utils.CopyCheck(copy, v.ToString());
                        Utils.NotifCheck(
                            notif, new string[] { v.ToString(), $"The factorial is: {v.ToString()}", "5" }
                        );
                        return v.ToString();

                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                        "Huh.",
                        "It seems that the number you inputted was not a number.",
                        "4"
                            }
                        );
                        return null;
                    }
                }
            );

            FormattableCommand flip = new(
                commandName: "flip",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    var flipped_char = new Dictionary<string, string>() {
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
                        var replaced = flipped_char.GetValueOrDefault(f.ToString(), "");
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
                aliases: new string[] { "flipped", "upside-down" }
            );

            FormattableCommand fraction = new(
                commandName: "fraction",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    Dictionary<char, string[]> fraction_dict = new Dictionary<char, string[]> {
                        { '0', new string[]{"⁰", "₀"} },
                        { '1', new string[]{"¹", "₁"} },
                        { '2', new string[]{"²", "₂"} },
                        { '3', new string[]{"³", "₃"} },
                        { '4', new string[]{"⁴", "₄"} },
                        { '5', new string[]{"⁵", "₅"} },
                        { '6', new string[]{"⁶", "₆"} },
                        { '7', new string[]{"⁷", "₇"} },
                        { '8', new string[]{"⁸", "₈"} },
                        { '9', new string[]{"⁹", "₉"} },
                        { '=', new string[]{"⁼", "₌"} },
                        { '+', new string[]{"⁺", "₊"} },
                        { '-', new string[]{"⁻", "₋"} },
                        { '(', new string[]{"⁽", "₍"} },
                        { ')', new string[]{"⁾", "₎"} },
                        { 'a', new string[]{"ᵃ", "ₐ"} },
                        { 'b', new string[]{"ᵇ", "failed"} },
                        { 'c', new string[]{"ᶜ", "failed"} },
                        { 'd', new string[]{"ᵈ", "failed"} },
                        { 'e', new string[]{"ᵉ", "ₑ"} },
                        { 'f', new string[]{"ᶠ", "failed"} },
                        { 'g', new string[]{"ᵍ", "failed"} },
                        { 'h', new string[]{"ʰ", "ₕ"} },
                        { 'i', new string[]{"ⁱ", "ᵢ"} },
                        { 'j', new string[]{"ʲ", "ⱼ"} },
                        { 'k', new string[]{"ᵏ", "ₖ"} },
                        { 'l', new string[]{"ˡ", "ₗ"} },
                        { 'm', new string[]{"ᵐ", "ₘ"} },
                        { 'n', new string[]{"ⁿ", "ₙ"} },
                        { 'o', new string[]{"ᵒ", "ₒ"} },
                        { 'p', new string[]{"ᵖ", "ₚ"} },
                        { 'r', new string[]{"ʳ", "ᵣ"} },
                        { 's', new string[]{"ˢ", "ₛ"} },
                        { 't', new string[]{"ᵗ", "ₜ"} },
                        { 'u', new string[]{"ᵘ", "ᵤ"} },
                        { 'v', new string[]{"ᵛ", "ᵥ"} },
                        { 'w', new string[]{"ʷ", "failed"} },
                        { 'x', new string[]{"ˣ", "ₓ"} },
                        { 'y', new string[]{"ʸ", "failed"} },
                        { 'z', new string[]{"ᶻ", "failed"} },
                    };

                    string[] slash_split = text.Split("/");
                    string numerator = slash_split[0];
                    string denominator = slash_split[1];

                    foreach (char x in numerator) {
                        if (fraction_dict.ContainsKey(x)) {
                            string i = fraction_dict[x][0];
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
                        if (fraction_dict.ContainsKey(x)) {
                            string i = fraction_dict[x][1];
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
                    string text = string.Join(' ', args[2..]);
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

                    List<int> nums_int = Utils.RegexFindAllInts(text);
                    List<System.Numerics.BigInteger> nums_BigIntegers = new();

                    foreach (int num in nums_int) {
                        nums_BigIntegers.Add(num);
                    }

                    try {
                        System.Numerics.BigInteger answer =
                            HCF.findGCD(
                                nums_BigIntegers.ToArray<System.Numerics.BigInteger>(),
                                nums_BigIntegers.ToArray().Length
                            );
                        Utils.CopyCheck(copy, answer.ToString());
                        Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                        return answer.ToString();
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

            FormattableCommand hexadecimal = new(
                commandName: "hexadecimal",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(' ', args[1..]);
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string[] text_list = text.Split(" ");
                    string hex_with_dash = string.Join("-", text_list);

                    if (Hex.IsHex(string.Join("", args[1..]))) {
                        string text_from_hex = System.Text.Encoding.ASCII.GetString(Hex.toText(hex_with_dash));
                        Utils.CopyCheck(copy, text_from_hex);
                        Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {text_from_hex}", "10" });
                        return text_from_hex;
                    } else {
                        string hex_from_text = Hex.toHex(text);
                        Utils.CopyCheck(copy, hex_from_text);
                        Utils.NotifCheck(notif, new string[] { "Success!", $"Message copied to clipboard.", "3" });
                        return hex_from_text;
                    }
                },
                aliases: new string[] { "hex" }
            );

            FormattableCommand lcm = new(
                commandName: "lcm",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(" ", args[1..]);

                    List<int> nums_int = Utils.RegexFindAllInts(text);
                    List<BigInteger> nums_BigInteger = new();

                    foreach (int num in nums_int) {
                        nums_BigInteger.Add(num);
                    }

                    try {
                        BigInteger answer =
                            lcm_class.lcm_exec(nums_BigInteger.ToArray<BigInteger>());
                        Utils.CopyCheck(copy, answer.ToString());
                        Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                        return answer.ToString();
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

                    string text = string.Join(' ', args[1..]);
                    string len = $@"Character count: {text.Length.ToString()}
Word count: {args[1..].Length}";
                    Utils.NotifCheck(notif, new string[] { "Success!", len, "5" });
                    return len;
                },
                aliases: new string[] { "len" }
            );

            FormattableCommand lowercase = new(
                commandName: "lowercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    string lower_text = text.ToLower();
                    Utils.CopyCheck(copy, lower_text);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return lower_text;
                },
                aliases: new string[] { "lower" }
            );

            FormattableCommand mathitalic = new(
                commandName: "mathitalic",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(' ', args[1..]);
                    List<string> converted = new();
                    Dictionary<string, string> mathitalic_char = new() {
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
                        var replaced = mathitalic_char.GetValueOrDefault(d.ToString(), "");
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
                aliases: new string[] { "mai" }
            );

            FormattableCommand morse = new(
                commandName: "morse",
                function: (string[] args, bool copy, bool notif) => {
                    string text = string.Join(' ', args[1..]).ToLower();
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    if (Utils.FormatValid("-./ ", text)) {
                        return Morse.toText(text, copy, notif);
                    } else {
                        return Morse.toMorse(text, copy, notif);
                    }
                },
                aliases: new string[] { "morsecode" }
            );

            FormattableCommand percentage = new(
                commandName: "percentage",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);

                    // making regex
                    List<Dictionary<Match, GroupCollection>>? matchToGroups = Utils.RegexFind(
                text,
                @"(?<percent>\d+(\.\d+)?)% of (?<number>\d+(\.\d+)?)",
                useIsMatch: true,
                () => {
                    Utils.NotifCheck(
                    true,
                    new string[] {
                            "Huh.",
                            "It seems you did not input the parameters correctly. Try '% 50% of 300'.",
                            "3"
                                    }
                );
                }
            );

                    if (matchToGroups != null) {
                        List<float> nums = new();

                        foreach (Dictionary<Match, GroupCollection> dict in matchToGroups) {
                            foreach (KeyValuePair<Match, GroupCollection> kvp in dict) {
                                nums.Add(float.Parse(kvp.Value["percent"].ToString()) / 100); // percentage in decimal
                                nums.Add(float.Parse(kvp.Value["number"].ToString())); // number
                            }
                        }

                        float y = nums[0] * nums[1]; // answer

                        Utils.NotifCheck(notif, new string[] { "Success!", $"The Answer is {y}.", "5" });
                        Utils.CopyCheck(copy, y.ToString());
                        return y.ToString();
                    } else {
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

                    string[] ascii_characters = {
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

                    string text = string.Join(' ', args[1..]);

                    // testing if text is a number
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
                        randomChar.Add(ascii_characters[rand.Next(0, ascii_characters.Length - 1)]);
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

                    Random rand = new Random();
                    int randint = rand.Next(nums[0], nums[1]);

                    Utils.CopyCheck(copy, randint.ToString());
                    Utils.NotifCheck(notif, new string[] { "Success!", $"The number was: {randint}", "5" });
                    return randint.ToString();
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
                    char[] text_array = text.ToCharArray();
                    List<char> text_list = new();
                    foreach (char ch in text_array) {
                        text_list.Add(ch);
                    }
                    text_list.Reverse();
                    string answer = string.Join("", text_list);
                    Utils.CopyCheck(copy, answer);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return answer;
                }
            );

            FormattableCommand sarcasm = new(
                commandName: "sarcasm",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    List<string> converted = new();
                    char current_case = 'u';
                    foreach (char i in text) {
                        string i_str = i.ToString();
                        if (current_case == 'u') {
                            converted.Add(i_str.ToUpper());
                            current_case = 'l';
                        } else if (current_case == 'l') {
                            converted.Add(i_str.ToLower());
                            current_case = 'u';
                        }
                    }
                    string sarcasm_text = string.Join("", converted);
                    Utils.CopyCheck(copy, sarcasm_text);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return sarcasm_text;
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

            FormattableCommand shuffle = new(
                commandName: "shuffle",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }

                    string text = string.Join(' ', args[1..]);
                    Shuffle.getPer(text.ToCharArray());

                    string ans = string.Join("\n", Shuffle.permutations);
                    Utils.CopyCheck(copy, ans);
                    System.Console.WriteLine($"Number of permutations: {Shuffle.permutations.Count}");
                    Utils.NotifCheck(notif, new string[] { "Success!", $@"Permutations copied to clipboard.
Number of permutations: {Shuffle.permutations.Count}", "4" });
                    return ans;
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
                }
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
                    string text = string.Join(' ', args[1..]).ToLower();

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
                aliases: new string[] { "title" }
            );

            FormattableCommand uppercase = new(
                commandName: "uppercase",
                function: (string[] args, bool copy, bool notif) => {
                    if (Utils.IndexTest(args)) {
                        return null;
                    }
                    string text = string.Join(" ", args[1..]);
                    string upper_text = text.ToUpper();
                    Utils.CopyCheck(copy, upper_text);
                    Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                    return upper_text;
                },
                aliases: new string[] { "upper" }
            );
        }
    }
}
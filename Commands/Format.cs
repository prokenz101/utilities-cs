using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class Formatter {
        public static void formatter(string[] args) {
            string text = string.Join(' ', args[1..]);
            if (Utils.IndexTest(args)) {
                return;
            }

            Dictionary<string, string> formatdict = new();

            Regex re = new Regex(@"{(?<command>[^}]+)}");
            MatchCollection matches = re.Matches(text);



            foreach (Match? i in matches) {
                if (i != null) {
                    GroupCollection groups = i.Groups;
                    Group main_group = groups["command"];

                    string cmd = main_group.ToString();
                    string[] splitcommand = cmd.Split(" ");

                    if (UtilitiesAppContext.formattable_commands.ContainsKey(splitcommand[0])) {
                        Func<string[], bool, bool, string?> f = UtilitiesAppContext.formattable_commands[splitcommand[0]];
                        string? output = f.Invoke(splitcommand, false, false);

                        if (output == null) {
                            output = "errored";
                        }

                        formatdict[cmd] = output;
                    }
                }
            }

            string result = formatdict.ReplaceKeyInString(text);
            WindowsClipboard.SetText(result);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }

    public static class DictionaryExtensions {
        public static string ReplaceKeyInString(this Dictionary<string, string> dictionary, string inputString) {

            var regex = new Regex("{(.*?)}");
            var matches = regex.Matches(inputString);
            foreach (Match? match in matches) {
                if (match != null) {
                    var valueWithoutBrackets = match.Groups[1].Value;
                    var valueWithBrackets = match.Value;

                    if (dictionary.ContainsKey(valueWithoutBrackets))
                        inputString = inputString.Replace(valueWithBrackets, dictionary[valueWithoutBrackets]);
                }
            }

            return inputString;
        }
    }
}
using System.Text.RegularExpressions;

namespace utilities_cs {
    public static partial class Format {

        [GeneratedRegex(@"{(?<command>[^}]+)}")]
        private static partial Regex CommandRegex();

        [GeneratedRegex("{(.*?)}")]
        private static partial Regex ReplaceRegex();

        public static void FormatMain(string[] args) {
            if (Utils.IndexTest(args)) { return; }
            string text = string.Join(" ", args[1..]);

            Dictionary<string, string> formatDict = new();
            var re = CommandRegex();

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
                            new string[] { "Huh.", "Perhaps that was not a real command.", "4" },
                            "formatError"
                        ); formatDict[cmd] = output;
                    } else { formatDict[cmd] = output; }
                }
            }

            Utils.CopyCheck(true, ReplaceKeyInString(formatDict, text));
            Utils.NotifCheck(true, ["Success!", "Message copied to clipboard.", "3"], "formatSuccess");
        }

        static string ReplaceKeyInString(Dictionary<string, string> dictionary, string inputString) {
            Regex re = ReplaceRegex();
            var matches = re.Matches(inputString);
            foreach (System.Text.RegularExpressions.Match? match in matches) {
                if (match != null) {
                    var valueWithoutBrackets = match.Groups[1].Value;
                    var valueWithBrackets = match.Value;

                    if (dictionary.TryGetValue(valueWithoutBrackets, out var replacementValue))
                        inputString = inputString.Replace(valueWithBrackets, replacementValue);
                }
            }

            return inputString;
        }
    }
}
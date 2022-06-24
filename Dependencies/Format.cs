namespace utilities_cs {
    public static class Format {
        public static void FormatMain(string[] args) {
            string text = string.Join(" ", args[1..]);
            if (Utils.IndexTest(args)) { return; }

            Dictionary<string, string> formatDict = new();
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
                            new string[] { "Huh.", "Perhaps that was not a real command.", "4" },
                            "formatError"
                        ); formatDict[cmd] = output;
                    } else { formatDict[cmd] = output; }
                }
            }

            Utils.CopyCheck(true, replaceKeyInString(formatDict, text));
            Utils.NotifCheck(true, new string[] { "Success!", "Message copied to clipboard.", "3" }, "formatSuccess");
        }

        static string replaceKeyInString(Dictionary<string, string> dictionary, string inputString) {
            var regex = new System.Text.RegularExpressions.Regex("{(.*?)}");
            var matches = regex.Matches(inputString);
            foreach (System.Text.RegularExpressions.Match? match in matches) {
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
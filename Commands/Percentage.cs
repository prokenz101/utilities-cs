using System.Text.RegularExpressions;

namespace utilities_cs {
    public class Percentage {
        public static string? Percent(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything at all.", 3)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);

            // making regex
            string regex_exp = @"(?<percent>\d+.?\d+)% of (?<number>\d+.?\d+)";
            Regex re = new Regex(regex_exp, RegexOptions.Compiled);

            if (re.IsMatch(text)) {
                // getting all values using regex
                MatchCollection matches = re.Matches(text);
                Match match = matches[0];
                GroupCollection groups = match.Groups;

                float p = float.Parse(groups["percent"].Value) / 100; // percentage in decimal
                float x = float.Parse(groups["number"].Value); // number
                float y = p * x; // answer

                Utils.NotifCheck(notif, new string[] { "Success!", $"The Answer is {y}.", "5" });
                Utils.CopyCheck(copy, y.ToString());
                return y.ToString();

            } else {
                Utils.Notification(
                    "Huh.",
                    "It seems you did not input the parameters for percentage properly. Try '% 50% of 300'.",
                    4
                );

                return null;
            }
        }
    }
}
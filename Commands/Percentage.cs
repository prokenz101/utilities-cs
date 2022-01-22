using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class Percentage {
        public static string? Percent(string[] args, bool copy, bool notif) {
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
                    Utils.Notification(
                        "Huh.",
                        "It seems you did not input the parameters correctly. Try '% 50% of 300'."
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
        }
    }
}
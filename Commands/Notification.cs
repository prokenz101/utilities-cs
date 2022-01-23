using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class Notification {
        public static void Notify(string[] args) {
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
        }
    }
}
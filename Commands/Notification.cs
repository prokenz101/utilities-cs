using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable

namespace utilities_cs {
    public class Notification {
        public static void Notify(string[] args) {
            string text = string.Join(' ', args[1..]);

            List<Dictionary<Match, GroupCollection>> matchToGroups = Utils.RegexFind(
                text,
                @"[""'](?<title>.*?)[""'],? [""'](?<subtitle>.*?)[""'],? (?<duration>\d+)",
                useIsMatch: true,
                () => {
                    Utils.Notification("Huh.", "The parameters were not inputted properly.", 3);
                }
            );

            Dictionary<Match, GroupCollection> match = matchToGroups[0];

            foreach (KeyValuePair<Match, GroupCollection> kvp in match) {
                GroupCollection groups = kvp.Value;

                string title = groups["title"].ToString();
                string subtitle = groups["subtitle"].ToString();
                int duration = int.Parse(groups["duration"].ToString());

                Utils.Notification(title, subtitle, duration);
                return;
            }
        }
    }
}
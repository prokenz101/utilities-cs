using System.Collections.Generic;

namespace utilities_cs {
    public class Spacer {
        public static string? spacer(string[] args, bool copy, bool notif) {
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
    }
}
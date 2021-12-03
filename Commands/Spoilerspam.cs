using System.Collections.Generic;

namespace utilities_cs {
    public class Spoilerspam {
        public static void spoilerspam(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems that you did not input anything for spoilerspam.", 3)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            foreach (char i in text) {
                converted.Add($"||{i}");
            }
            string answer = $"{string.Join("||", converted)}||";
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
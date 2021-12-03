using System.Collections.Generic;

namespace utilities_cs {
    public class Spacer {
        public static void spacer(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for spacer to work.", 3)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            foreach (char i in text) {
                converted.Add(i.ToString());
                converted.Add(" ");
            }
            string answer = string.Join("", converted);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
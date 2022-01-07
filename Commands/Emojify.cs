using System.Collections.Generic;

namespace utilities_cs {
    public class Emojify {
        public static void emojify(string[] args) {
            string text = string.Join(' ', args[1..]);

            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for utilities-cs to emojify.", 4)) {
                return;
            }

            List<string> converted = new();
            Dictionary<string, string> special_char = new Dictionary<string, string> {
                {" ", ":black_large_square:"},
                {"?", ":question:"},
                {"!", ":exclamation:"},
                {"1", ":one:"},
                {"2", ":two:"},
                {"3",":three:"},
                {"4", ":four:"},
                {"5", ":five:"},
                {"6", ":six:"},
                {"7", ":seven:"},
                {"8", ":eight:"},
                {"9", ":nine:"},
                {"0", ":zero:"},
            };

            foreach (char i in text) {
                if (Utils.FormatValid(
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                   i.ToString()
                )) {
                    converted.Add($":regional_indicator_{i.ToString().ToLower()}:");
                } else if (special_char.ContainsKey(i.ToString())) {
                    converted.Add(special_char[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            WindowsClipboard.SetText(string.Join(" ", converted));
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
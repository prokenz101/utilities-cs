using System.Collections.Generic;

namespace utilities_cs {
    public class Doublestruck {
        public static void dbs(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for doublestruck to work.", 4)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            var dbs_char = new Dictionary<string, string>() {
                    {"a", "𝕒"}, {"b", "𝕓"}, {"c", "𝕔"}, {"d", "𝕕"}, {"e", "𝕖"},
                    {"f", "𝕗"}, {"g", "𝕘"}, {"h", "𝕙"}, {"i", "𝕚"}, {"j", "𝕛"},
                    {"k", "𝕜"}, {"l", "𝕝"}, {"m", "𝕞"}, {"n", "𝕟"}, {"o", "𝕠"},
                    {"p", "𝕡"}, {"q", "𝕢"}, {"r", "𝕣"}, {"s", "𝕤"}, {"t", "𝕥"},
                    {"u", "𝕦"}, {"v", "𝕧"}, {"w", "𝕨"}, {"x", "𝕩"}, {"y", "𝕪"},
                    {"z", "𝕫"}, {"A", "𝔸"}, {"B", "𝔹"}, {"C", "ℂ"}, {"D", "𝔻"},
                    {"E", "𝔼"}, {"F", "𝔽"}, {"H", "ℍ"}, {"I", "𝕀"}, {"J", "𝕁"},
                    {"K", "𝕂"}, {"L", "𝕃"}, {"M", "𝕄"}, {"N", "ℕ"}, {"O", "𝕆"},
                    {"P", "ℙ"}, {"Q", "ℚ"}, {"R", "ℝ"}, {"S", "𝕊"}, {"T", "𝕋"},
                    {"U", "𝕌"}, {"V", "𝕍"}, {"W", "𝕎"}, {"X", "𝕏"}, {"Y", "𝕐"},
                    {"Z", "ℤ"}, {"1", "𝟙"}, {"2", "𝟚"}, {"3", "𝟛"}, {"4", "𝟜"},
                    {"5", "𝟝"}, {"6", "𝟞"}, {"7", "𝟟"}, {"8", "𝟠"}, {"9", "𝟡"},
                    {"0", "𝟘"}, {" ", " "}
                };
            foreach (char d in text) {
                var replaced = dbs_char.GetValueOrDefault(d.ToString(), "");
                if (replaced != "") {
                    converted.Add(replaced);
                } else {
                    converted.Add(d.ToString());
                }
            }
            var answer = string.Join("", converted);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
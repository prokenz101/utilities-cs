using System.Collections.Generic;

namespace utilities_cs {
    public class Flip {
        public static string? flip(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for flip to work.", 4)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            var flipped_char = new Dictionary<string, string>() {
                    {"a", "ɐ"}, {"b", "q"}, {"c", "ɔ"}, {"d", "p"}, {"e", "ǝ"},
                    {"f", "ɟ"}, {"g", "ƃ"}, {"h", "ɥ"}, {"i", "ᴉ"}, {"j", "ɾ"},
                    {"k", "ʞ"}, {"l", "l"}, {"m", "ɯ"}, {"n", "u"}, {"o", "o"},
                    {"p", "d"}, {"r", "ɹ"}, {"s", "s"}, {"t", "ʇ"}, {"u", "n"},
                    {"v", "ʌ"}, {"w", "ʍ"}, {"x", "x"}, {"y", "ʎ"}, {"z", "z"},
                    {"A", "∀"}, {"B", "q"}, {"C", "Ɔ"}, {"D", "p"}, {"E", "Ǝ"},
                    {"F", "Ⅎ"}, {"G", "פ"}, {"H", "H"}, {"I", "I"}, {"J", "ſ"},
                    {"K", "ʞ"}, {"L", "˥"}, {"M", "W"}, {"N", "N"}, {"O", "O"},
                    {"P", "Ԁ"}, {"Q", "Q"}, {"R", "ɹ"}, {"S", "S"}, {"T", "┴"},
                    {"U", "∩"}, {"V", "Λ"}, {"W", "M"}, {"X", "X"}, {"Y", "⅄"}, {"Z", "Z"}
                };

# nullable disable

            foreach (char f in text) {
                var replaced = flipped_char.GetValueOrDefault(f.ToString(), "");
                if (replaced != "") {
                    converted.Add(replaced);
                } else {
                    converted.Add(f.ToString());
                }
            }

# nullable enable

            converted.Reverse();
            var answer = string.Join("", converted);
            Utils.CopyCheck(copy, answer);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return answer;
        }
    }
}
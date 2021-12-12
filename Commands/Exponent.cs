using System.Collections.Generic;

#nullable disable

namespace utilities_cs {
    public class Exponent {
        public static void exponent(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems that you did not input anything at all.", 3)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            var exponents = new Dictionary<string, string>() {
                    {"-", "⁻"}, {"=", "⁼"}, {"+", "⁺"},
                    {"1", "¹"}, {"2", "²"}, {"3", "³"},
                    {"4", "⁴"}, {"5", "⁵"}, {"6", "⁶"},
                    {"7", "⁷"}, {"8", "⁸"}, {"9", "⁹"}, {"0", "⁰"},
                    {"a", "ᵃ"}, {"b", "ᵇ"}, {"c", "ᶜ"}, {"d", "ᵈ"},{ "e", "ᵉ"},
                    {"f", "ᶠ"}, {"g", "ᵍ"}, {"h", "ʰ"}, {"i", "ᶦ"},{ "j", "ʲ"},
                    {"k", "ᵏ"}, {"l", "ˡ"}, {"m", "ᵐ"}, {"n", "ⁿ"},{ "o", "ᵒ"},
                    {"p", "ᵖ"}, {"r", "ʳ"}, {"s", "ˢ"}, {"t", "ᵗ"}, {"u", "ᵘ"},
                    {"v", "ᵛ"}, {"w", "ʷ"}, {"x", "ˣ"}, {"y", "ʸ"}, {"z", "ᶻ"},
                    {"(", "⁽"}, {")", "⁾"}, {" ", " "}
                };
            foreach (char e in text) {
                var replaced = exponents.GetValueOrDefault(e.ToString(), "");
                if (replaced != "") {
                    converted.Add(replaced);
                } else {
                    converted.Add(e.ToString());
                }
            }
            var answer = string.Join("", converted);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
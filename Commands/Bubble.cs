using System.Collections.Generic;

namespace utilities_cs
{
    public class Bubble
    {
        public static void bubbletext(string[] args)
        {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for bubble to work.", 4)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            var bubble_char = new Dictionary<string, string>() {
                {"a", "ⓐ"}, {"b", "ⓑ"}, {"c", "ⓒ"}, {"d", "ⓓ"}, {"e", "ⓔ"},
                {"f", "ⓕ"}, {"g", "ⓖ"}, {"h", "ⓗ"}, {"i", "ⓘ"}, {"j", "ⓙ"},
                {"k", "ⓚ"}, {"l", "ⓛ"}, {"m", "ⓜ"}, {"n", "ⓝ"}, {"o", "ⓞ"},
                {"p", "ⓟ"}, {"q", "ⓠ"}, {"r", "ⓡ"}, {"s", "ⓢ"}, {"t", "ⓣ"},
                {"u", "ⓤ"}, {"v", "ⓥ"}, {"w", "ⓦ"}, {"x", "ⓧ"}, {"y", "ⓨ"},
                {"z", "ⓩ"}, {"A", "Ⓐ"}, {"B", "Ⓑ"}, {"C", "Ⓒ"}, {"D", "Ⓓ"},
                {"E", "Ⓔ"}, {"F", "Ⓕ"}, {"G", "Ⓖ"}, {"H", "Ⓗ"}, {"I", "Ⓘ"},
                {"J", "Ⓙ"}, {"K", "Ⓚ"}, {"L", "Ⓛ"}, {"M", "Ⓜ"}, {"O", "Ⓞ"},
                {"N", "Ⓝ"}, {"P", "Ⓟ"}, {"Q", "Ⓠ"}, {"R", "Ⓡ"}, {"S", "Ⓢ"},
                {"T", "Ⓣ"}, {"U", "Ⓤ"}, {"V", "Ⓥ"}, {"W", "Ⓦ"}, {"X", "Ⓧ"},
                {"Y", "Ⓨ"}, {"Z", "Ⓩ"}, {"1", "①"}, {"2", "②"}, {"3", "③"},
                {"4", "④"}, {"5", "⑤"}, {"6", "⑥"}, {"7", "⑦"}, {"8", "⑧"},
                {"9", "⑨"}, {"0", "⓪"}
                };
            foreach (char b in text)
            {
                var replaced = bubble_char.GetValueOrDefault(b.ToString(), "");
                if (replaced != "")
                {
                    converted.Add(replaced);
                }
                else
                {
                    converted.Add(b.ToString());
                }
            }
            var answer = string.Join("", converted);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
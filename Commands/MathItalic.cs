using System.Collections.Generic;

namespace utilities_cs {
    public class MathItalics {
        public static string? MathItalic(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(' ', args[1..]);
            List<string> converted = new();
            Dictionary<string, string> mathitalic_char = new() {
                { "a", "𝑎" },
                { "b", "𝑏" },
                { "c", "𝑐" },
                { "d", "𝑑" },
                { "e", "𝑒" },
                { "f", "𝑓" },
                { "g", "𝑔" },
                { "h", "ℎ" },
                { "i", "𝑖" },
                { "j", "𝑗" },
                { "k", "𝑘" },
                { "l", "𝑙" },
                { "m", "𝑚" },
                { "n", "𝑛" },
                { "o", "𝑜" },
                { "p", "𝑝" },
                { "q", "𝑞" },
                { "r", "𝑟" },
                { "s", "𝑠" },
                { "t", "𝑡" },
                { "u", "𝑢" },
                { "v", "𝑣" },
                { "w", "𝑤" },
                { "x", "𝑥" },
                { "y", "𝑦" },
                { "z", "𝑧" },
                { "A", "𝐴" },
                { "B", "𝐵" },
                { "C", "𝐶" },
                { "D", "𝐷" },
                { "E", "𝐸" },
                { "F", "𝐹" },
                { "G", "𝐺" },
                { "H", "𝐻" },
                { "I", "𝐼" },
                { "J", "𝐽" },
                { "K", "𝐾" },
                { "L", "𝐿" },
                { "M", "𝑀" },
                { "N", "𝑁" },
                { "O", "𝑂" },
                { "P", "𝑃" },
                { "Q", "𝑄" },
                { "R", "𝑅" },
                { "S", "𝑆" },
                { "T", "𝑇" },
                { "U", "𝑈" },
                { "V", "𝑉" },
                { "W", "𝑊" },
                { "X", "𝑋" },
                { "Y", "𝑌" },
                { "Z", "𝑍" },
            };

            foreach (char d in text) {
                var replaced = mathitalic_char.GetValueOrDefault(d.ToString(), "");
                if (replaced != "") {
                    converted.Add(replaced!);
                } else {
                    converted.Add(d.ToString());
                }
            }

            var answer = string.Join("", converted);
            Utils.CopyCheck(copy, answer);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return answer;
        }
    }
}
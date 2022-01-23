using System;
using System.Linq;
using System.Collections.Generic;

namespace utilities_cs {
    public class Randchar {
        public static string? RandomChar(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string[] ascii_characters = {
                "a", "b", "c", "d", "e",
                "f", "g", "h", "i", "j",
                "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v",
                "w", "x", "y", "z", "A", "B",
                "C", "D", "E", "F", "G", "H",
                "I", "J", "K", "L", "M", "N",
                "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z"
            };

            string text = string.Join(' ', args[1..]);

            // testing if text is a number
            try {
                int.Parse(text);
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Something went wrong.",
                        "Either the number you entered was not a number, or it was too large.",
                        "5"
                    }
                );
            }

            Random rand = new Random();
            List<string> randomChar = new();

            foreach (int i in Enumerable.Range(1, int.Parse(text))) {
                randomChar.Add(ascii_characters[rand.Next(0, ascii_characters.Length - 1)]);
            }

            string ans = string.Join("", randomChar);
            Utils.CopyCheck(copy, ans);
            Utils.NotifCheck(notif, new string[] { "Success!", "Text copied to clipboard.", "3" });
            return ans;
        }
    }
}
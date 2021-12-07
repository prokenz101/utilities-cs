using System.Collections.Generic;

namespace utilities_cs {
    public class Creepy {
        public static void creepy(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for creepy to work.", 4)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            var creepy_char = new Dictionary<string, string>() {
                    {"a", "á̷͍̖̐̐͘"}, {"b", "ḃ̶̢̹̖"}, {"c", "c̸̢̧̰̙͔̲̿̈́͌̉̀͘"}, {"d", "d̸͉͛̈́̊̍͘"}, {"e", "ḗ̸̫̽"},
                    {"f", "f̸̡̹̱̹̺͋͒͋"}, {"g", "g̴̼̙̜͒̄̈́̚͝"}, {"h", "h̴̜̕"}, {"i", "í̸͓̬͚̘̆"}, {"j", "j̶̯͋̋͋"},
                    {"k", "k̴̛̰̻͈͘͘͜"}, {"l", "l̸͔̠̝̪̯͇͐̓͆"}, {"m", "m̴̲̗͗̽̂͌"}, {"n", "n̸͈͇̳̈̾̿̄ͅ"}, {"o", "o̵̧̜̖͈̲͔͂͋́͝"},
                    {"p", "p̶̡̯̳͓̣͂̈́́͘"}, {"q", "q̴̡͓̭̠̂͋̈́̔"}, {"r", "r̶͍̎"}, {"s", "s̴͈͎̙̘̱̋ͅ"}, {"t", "ţ̶̠̜̙͚̎͗"},
                    {"u", "ų̸̙̭͋ͅ"}, {"v", "v̶̗͂̑̕̚"}, {"w", "w̸͉͂̈́̅̌̊"}, {"x", "x̴͕̞̙̮͐͐͒"}, {"y", "ÿ̵̠͍̪̠̩́"},
                    {"z", "z̶̞͖̓̚"}, {"A", "A̷̡͍̩͉̱̹͑̒̀̑͝"}, {"B", "B̵̯̭̄̀̾̑"}, {"C", "C̷̗̽͛"}, {"D", "D̴͖͈̯̜̭̊̓̏͆̆͘"},
                    {"E", "Ḙ̷̦̠̍"}, {"F", "F̶̛̮̤̈́̿̈́͂̂"}, {"G", "Ĝ̶̨̢̺̻̹̦̅͆̈́͗"}, {"H", "H̸̼͖̦̗͛͗͐̿̀̀ͅ"}, {"I", "Į̶̛̩͙̭͕́̌̏̚"},
                    {"J", "J̷̜̀͆̄͛̆"}, {"K", "Ḵ̴̨̧̨͔̾"}, {"L", "Ḻ̶̰̱̹͎͈̔"}, {"M", "M̵̠̲̞̿̋̐̕̕͝"}, {"N", "Ṅ̷̻"},
                    {"O", "O̸̞̍̐"}, {"P", "P̵͈͊͋͂͗͝"}, {"Q", "Q̸̡͉̥̱͕̩̄̈́"}, {"R", "R̵̻̺̯͗̇͜"}, {"S", "S̴͖̬̀̇̃͋̈"},
                    {"T", "T̵͓̫̠̈́̂̀̓́̍ͅ"}, {"U", "Ụ̷̡͚̻͇͆͑̉͋͝"}, {"V", "V̴̟̪͓͓̩̳̄̀͌̾̕"}, {"W", "W̵̞̯͛̿"}, {"X", "X̷͈͍̬́"},
                    {"Y", "Ỳ̶̖̣͌͜"}, {"Z", "Z̴̗͈̬̱̩̆̊͗"}, {" ", " "}
                };
            foreach (char cr in text) {
                var replaced = creepy_char.GetValueOrDefault(cr.ToString(), "");
                if (replaced != "") {
                    converted.Add(replaced);
                } else {
                    converted.Add(cr.ToString());
                }
            }
            var answer = string.Join("", converted);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
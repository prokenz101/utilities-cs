using System;
using System.Collections.Generic;

namespace utilities_cs
{
    public class Copypaste
    {
        public static void cp(string[] args)
        {
            string text = string.Join(" ", args[1..]);
            Dictionary<string, string> cp_dict = new() {

            {"aigu e", "é"}, {"aigu E", "É"}, {"grave a", "à"},
            {"grave e", "è"}, {"grave u", "ù"}, {"grave A", "À"},
            {"grave E", "È"}, {"grave U", "Ù"}, {"chapeau a", "â"},
            {"chapeau e", "ê"}, {"chapeau i", "î"}, {"chapeau o", "ô"},
            {"chapeau u", "û"}, {"chapeau A", "Â"}, {"chapeau E", "Ê"},
            {"chapeau I", "Î"}, {"chapeau O", "Ô"}, {"chapeau U", "Û"},
            {"trema e", "ë"}, {"trema i", "ï"}, {"trema u", "ü"}, {"bullet", "•"},
            {"trema E", "Ë"}, {"trema I", "Ï"}, {"trema U", "Ü"},
            {"cedille c", "ç"}, {"cedille C", "Ç"}, {"3164", "ㅤ"},
            {"hangul filler", "ㅤ"}, {"divison", "÷"}, {"divide", "÷"}, {"multi", "×"},
            {"!=", "≠"}, {"congruence", "≅"}, {"greater than or equal to", "≥"},
            {">=", "≥"}, {"lesser than or equal to", "≤"}, {"<=", "≤"},
            {"shrug", @"¯\_(ツ)_/¯"}, {"trademark", "™️"}, {"copyright", "©️"},
            {"music", "♩♪♫♬"}, {"therefore", "∴"}, {"x", "𝑥"}, {"y", "𝑦"}
            
        };
            
            if (cp_dict.ContainsKey(text)) {
                WindowsClipboard.SetText(cp_dict[text]);
                Utils.Notification("Success!", "Message copied to clipboard.");
            }
            else {
                Utils.Notification(
                    "Welp.",
                    "It seems that utilities could not understand what word you were trying to copypaste.",
                    3
                );
                Environment.Exit(0);
            }
            
        }
    }
}
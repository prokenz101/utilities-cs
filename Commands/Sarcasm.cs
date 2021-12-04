using System.Collections.Generic;

namespace utilities_cs {
    public class Sarcasm {
        public static void Sarcasm_(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for sarcasm to 'sarcasm-ize'.", 4)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            char current_case = 'u';
            foreach (char i in text) {
                string i_str = i.ToString();
                if (current_case == 'u') {
                    converted.Add(i_str.ToUpper());
                    current_case = 'l';
                } else if (current_case == 'l') {
                    converted.Add(i_str.ToLower());
                    current_case = 'u';
                }
            }
            string sarcasm_text = string.Join("", converted);
            WindowsClipboard.SetText(sarcasm_text);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
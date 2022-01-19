using System.Collections.Generic;

namespace utilities_cs {
    public class Sarcasm {
        public static string? Sarcasm_(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
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
            Utils.CopyCheck(copy, sarcasm_text);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return sarcasm_text;
        }
    }
}
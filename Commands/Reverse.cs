using System.Collections.Generic;

namespace utilities_cs {
    public class Reverse {
        public static string? reverse(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);
            char[] text_array = text.ToCharArray();
            List<char> text_list = new();
            foreach (char ch in text_array) {
                text_list.Add(ch);
            }
            text_list.Reverse();
            string answer = string.Join("", text_list);
            Utils.CopyCheck(copy, answer);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return answer;
        }
    }
}
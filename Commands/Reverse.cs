using System.Collections.Generic;

namespace utilities_cs {
    public class Reverse {
        public static void reverse(string[] args) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything to reverse.", 3)) {
                return;
            }
            string text = string.Join(" ", args[1..]);
            char[] text_array = text.ToCharArray();
            List<char> text_list = new();
            foreach (char ch in text_array) {
                text_list.Add(ch);
            }
            text_list.Reverse();
            string answer = string.Join("", text_list);
            WindowsClipboard.SetText(answer);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
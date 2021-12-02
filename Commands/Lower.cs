namespace utilities_cs {
    public class Lower {
        public static void Lowercase(string[] args) {
            string text = string.Join(" ", args[1..]);
            string lower_text = text.ToLower();
            WindowsClipboard.SetText(lower_text);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
namespace utilities_cs {
    public class Upper {
        public static void Uppercase(string[] args) {
            string text = string.Join(" ", args[1..]);
            string upper_text = text.ToUpper();
            WindowsClipboard.SetText(upper_text);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
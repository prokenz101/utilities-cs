namespace utilities_cs {
    public class Upper {
        public static string? Uppercase(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input any text for uppercase.", 4)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);
            string upper_text = text.ToUpper();
            Utils.CopyCheck(copy, upper_text);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return upper_text;
        }
    }
}
namespace utilities_cs {
    public class Lower {
        public static string? Lowercase(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input any text for lowercase.", 4)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);
            string lower_text = text.ToLower();
            Utils.CopyCheck(copy, lower_text);
            Utils.NotifCheck(notif, new string[]{"Success!", "Message copied to clipboard.", "3"});
            return lower_text;
        }
    }
}
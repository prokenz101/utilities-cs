using System.Globalization;

namespace utilities_cs {
    public class Title {
        public static string? title(string[] args, bool copy, bool notif) {
            string text = string.Join(' ', args[1..]).ToLower();

            if (Utils.IndexTest(args)) {
                return null;
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            string ans = textInfo.ToTitleCase(string.Join(" ", text));
            Utils.CopyCheck(copy, ans);
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return ans;
        }
    }
}
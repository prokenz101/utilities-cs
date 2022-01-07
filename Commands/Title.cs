using System.Globalization;

namespace utilities_cs {
    public class Title {
        public static void title(string[] args) {
            string text = string.Join(' ', args[1..]).ToLower();

            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything.", 4)) {
                return;
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            string ans = textInfo.ToTitleCase(string.Join(" ", text));
            WindowsClipboard.SetText(ans);
            Utils.Notification("Success!", "Message copied to clipboard.", 3);
        }
    }
}
namespace utilities_cs {
    public class Base64Convert {
        public static string? Base64ConvertMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args[1..]);
            if (IsBase64.IsBase64String(text)) {
                try {
                    string ans = Base64Convert.Base64Decode(text);
                    Utils.CopyCheck(copy, ans);
                    Utils.NotifCheck(
                        notif,
                        new string[] { "Success!", $"The message was: {ans}", "6" },
                        "base64Success"
                    ); return ans;
                } catch {
                    Utils.NotifCheck(
                        true,
                        new string[] { "Huh.", "An exception occured when converting this text to Base64.", "4" },
                        "base64Error"
                    ); return null;
                }
            } else {
                string ans = Base64Convert.Base64Encode(text);
                Utils.CopyCheck(copy, ans);
                Utils.NotifCheck(
                    notif,
                    new string[] { "Success!", "The message was copied to your clipboard.", "3" },
                    "base64Success"
                ); return ans;
            }
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public static class IsBase64 {
        public static string? IsBase64Main(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args[1..]);

            if (IsBase64.IsBase64String(text)) {
                Utils.NotifCheck(notif, new string[] { "Yes.", "The string is Base64.", "3" }, "isBase64Success");
                return "Yes";
            } else {
                Utils.NotifCheck(notif, new string[] { "No.", "The string is not Base64.", "3" }, "isBase64Success");
                return "No";
            }
        }

        public static bool IsBase64String(this string s) {
            s = s.Trim();
            return (s.Length % 4 == 0) &&
                System.Text.RegularExpressions.Regex.IsMatch(
                    s, @"^[a-zA-Z0-9\+/]*={0,3}$",
                    System.Text.RegularExpressions.RegexOptions.None
                );
        }
    }
}
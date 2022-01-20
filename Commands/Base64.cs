using System.Text.RegularExpressions;

namespace utilities_cs {
    public static class Base64Conversion {
        public static string? Base64Convert(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(' ', args[1..]);
            if (IsBase64String(text)) {
                string ans = Base64Decode(text);
                Utils.CopyCheck(copy, ans);
                Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {ans}", "6" });
                return ans;
            } else {
                string ans = Base64Encode(text);
                Utils.CopyCheck(copy, ans);
                Utils.NotifCheck(notif, new string[] { "Success!", "The message was copied to your clipboard.", "3" });
                return ans;
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

        public static bool IsBase64String(this string s) {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }
    }
}
namespace utilities_cs {
    public class Base64Convert {
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    static class IsBase64 {
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
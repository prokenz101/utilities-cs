using System;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class CubeRoot {
        public static string? Cbrt(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(' ', args[1..]);
            // testing if string is a double
            try {
                Convert.ToDouble(text);
            } catch (FormatException) {
                Utils.NotifCheck(true, new string[] { "Huh.", "It seems you did not input a number." });
                return null;
            }

            // checking if there are commas in the number
            if (text.Contains(",")) {
                text = text.Replace(",", string.Empty);
            }

            double num = Convert.ToDouble(text);
            string result = Math.Pow(num, ((double)1 / 3)).ToString();

            Regex re = new Regex(@"\d+\.99999+(?:\d+)");

            if (re.IsMatch(result)) {
                result = Math.Ceiling(Convert.ToDouble(result)).ToString();
            }

            Utils.CopyCheck(copy, result);
            Utils.NotifCheck(notif, new string[] { "Success!", $"The answer is: {result}", "4" });
            return result;
        }
    }
}
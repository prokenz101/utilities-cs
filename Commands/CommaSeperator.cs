using System;
using System.Numerics;

namespace utilities_cs {
    public class CommaSeperator {
        public static string? Cms(string[] args, bool copy, bool notif) {
            string str_num = string.Join(' ', args[1..]);
            if (Utils.IndexTest(args)) {
                return null;
            }

            try {
                // Checking if number is an actual number
                BigInteger.Parse(str_num);
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems you did not input anything to seperate with commas.",
                        "2"
                    }
                );
                return null;
            }

            BigInteger num = BigInteger.Parse(str_num);
            string ans = String.Format("{0:n0}", num);

            Utils.CopyCheck(copy, ans);
            Utils.NotifCheck(notif, new string[] { "Success!", "Number copied to clipboard.", "3" });
            return ans;
        }
    }
}
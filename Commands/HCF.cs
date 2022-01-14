using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class HCF {
        static BigInteger hcf(BigInteger a, BigInteger b) {
            if (a == 0)
                return b;
            return hcf(b % a, a);
        }

        // Function to find gcd of 
        // array of numbers
        static BigInteger findGCD(BigInteger[] arr, BigInteger n) {
            BigInteger result = arr[0];
            for (int i = 1; i < n; i++) {
                result = hcf(arr[i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
        public static string? hcf_main(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(
                    args,
                    "Huh.",
                    "It seems you did not input any number for the HCF calculator to work.",
                    4
                )
            ) {
                return null;
            }

            string text = string.Join(" ", args);

            string regex_exp = @"(\d+)+";
            Regex re = new Regex(regex_exp);
            MatchCollection matches = re.Matches(text);
            List<BigInteger> nums = new();

# nullable disable

            if (matches.Count >= 2) {
                foreach (Match match in matches) {
                    nums.Add(BigInteger.Parse(match.ToString()));
                }
            }

            BigInteger[] nums_array = nums.ToArray();

# nullable enable

            try {
                BigInteger answer = findGCD(nums_array, nums_array.Length);
                Utils.CopyCheck(copy, answer.ToString());
                Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                return answer.ToString();
            } catch {
                Utils.Notification(
                    "Huh.",
                    "It seems you did not input the numbers properly. Try 'hcf 15 70' as an example.",
                    8
                );

                return null;
            }
        }
    }
}
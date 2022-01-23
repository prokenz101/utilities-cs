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
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args);

            List<int> nums_int = Utils.RegexFindAllInts(text);
            List<BigInteger> nums_BigIntegers = new();

            foreach (int num in nums_int) {
                nums_BigIntegers.Add(num);
            }

            try {
                BigInteger answer = findGCD(nums_BigIntegers.ToArray(), nums_BigIntegers.ToArray().Length);
                Utils.CopyCheck(copy, answer.ToString());
                Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                return answer.ToString();
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems you did not input the numbers properly. Try 'hcf 15 70' as an example.",
                        "8"
                    }
                );

                return null;
            }
        }
    }
}
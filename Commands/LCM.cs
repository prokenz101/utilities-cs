using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class lcm_class {
        static BigInteger lcm(BigInteger[] element_array) {
            BigInteger lcm_of_array_elements = 1;
            int divisor = 2;

            while (true) {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < element_array.Length; i++) {

                    if (element_array[i] == 0) {
                        return 0;
                    } else if (element_array[i] < 0) {
                        element_array[i] = element_array[i] * (-1);
                    }
                    if (element_array[i] == 1) {
                        counter++;
                    }

                    if (element_array[i] % divisor == 0) {
                        divisible = true;
                        element_array[i] = element_array[i] / divisor;
                    }
                }

                if (divisible) {
                    lcm_of_array_elements = lcm_of_array_elements * divisor;
                } else {
                    divisor++;
                }

                if (counter == element_array.Length) {
                    return lcm_of_array_elements;
                }
            }
        }
        public static string? lcm_main(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(
                    args,
                    "Huh.",
                    "It seems you did not input any number for the LCM Calculator to work.",
                    4
                )
            ) {
                return null;
            }

            string text = string.Join(" ", args[1..]);

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
                BigInteger answer = lcm(nums_array);
                Utils.CopyCheck(copy, answer.ToString());
                Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
                return answer.ToString();
            } catch {
                Utils.Notification(
                    "Huh.",
                    "It seems you did not input a number. Try 'lcm 15 70' as an example.",
                    8
                );
                return null;
            }
        }
    }
}
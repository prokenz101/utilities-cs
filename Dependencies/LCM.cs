using System.Numerics;

namespace utilities_cs {
    public class LCMClass {
        public static string? LCMMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args[1..]);

            List<BigInteger> nums = new();
            Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

            try {
                if (nums.Count > 1) {
                    BigInteger answer =
                    LCMClass.lcmExec(nums.ToArray<BigInteger>());
                    Utils.CopyCheck(copy, answer.ToString());
                    Utils.NotifCheck(
                        notif, new string[] { "Success!", $"The answer was {answer}.", "5" }, "lcmSuccess"
                    ); return answer.ToString();
                } else {
                    Utils.NotifCheck(
                        true,
                        new string[] { "Something went wrong.", "You need to input at least two numbers.", "4" },
                        "lcmError"
                    );
                    return null;
                }
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems you did not input a number. Try 'lcm 15 70' as an example.",
                        "8"
                    },
                    "lcmError"
                );
                return null;
            }
        }

        public static BigInteger lcmExec(BigInteger[] element_array) {
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
    }
}
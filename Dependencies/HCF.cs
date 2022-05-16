using System.Numerics;

namespace utilities_cs {
    public class HCF {
        public static string? HCFMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string text = string.Join(" ", args);
            List<BigInteger> nums = new();
            Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

            try {
                if (nums.Count > 1) {
                    BigInteger answer =
                    HCF.FindGCD(
                        nums.ToArray<BigInteger>(),
                        nums.ToArray().Length
                    );
                    Utils.CopyCheck(copy, answer.ToString());
                    Utils.NotifCheck(
                        notif, new string[] { "Success!", $"The answer was {answer}.", "5" }, "hcfSuccess"
                    ); return answer.ToString();
                } else {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.", "You need to input at least two numbers.", "4"
                        },
                        "hcfError"
                    );
                    return null;
                }
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems you did not input the numbers properly. Try 'hcf 15 70' as an example.",
                        "8"
                    },
                    "hcfError"
                );

                return null;
            }
        }

        public static System.Numerics.BigInteger FindHCF(
            System.Numerics.BigInteger a, System.Numerics.BigInteger b
        ) {
            if (a == 0)
                return b;
            return FindHCF(b % a, a);
        }

        //* Function to find gcd of 
        //* array of numbers
        public static System.Numerics.BigInteger FindGCD(
            System.Numerics.BigInteger[] arr,
            System.Numerics.BigInteger n
        ) {
            System.Numerics.BigInteger result = arr[0];
            for (int i = 1; i < n; i++) {
                result = FindHCF(arr[i], result);

                if (result == 1) {
                    return 1;
                }
            }

            return result;
        }
    }
}
using System.Numerics;

namespace utilities_cs {
    public class HCF {
        public static string? HCFMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) { return null; }

            string text = string.Join(" ", args);
            List<BigInteger> nums = [];
            Utils.RegexFindAllInts(text).ForEach(x => nums.Add(x));

            try {
                if (nums.Count > 1) {
                    BigInteger answer =
                    HCF.FindGCD(
                        [.. nums],
                        nums.ToArray().Length
                    );

                    Utils.CopyCheck(copy, answer.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer was {answer}.", "5"], "hcfSuccess"
                    ); return answer.ToString();
                } else {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Invalid input, try 'help' for more info.", "4"],
                        "hcfError"
                    );
                    return null;
                }
            } catch {
                Utils.NotifCheck(
                    true, ["Exception", "Invalid input, try 'help' for more info.", "4"], "hcfError"
                );

                return null;
            }
        }

        public static BigInteger FindHCF(
            BigInteger a, BigInteger b
        ) {
            if (a == 0) { return b; }
            return FindHCF(b % a, a);
        }

        //* Function to find gcd of array of numbers
        public static BigInteger FindGCD(
            BigInteger[] arr,
            BigInteger n
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
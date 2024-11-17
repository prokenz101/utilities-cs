using System.Numerics;

namespace utilities_cs {
    public class LCMClass {
        public static string? LCMMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) { return null; }

            string text = string.Join(" ", args[1..]);

            List<BigInteger> nums = [];
            Utils.RegexFindAllInts(text).ForEach(nums.Add);

            try {
                if (nums.Count > 1) {
                    BigInteger answer =
                    FindLCM([.. nums]);
                    Utils.CopyCheck(copy, answer.ToString());
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer was {answer}.", "5"], "lcmSuccess"
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

        public static BigInteger FindLCM(BigInteger[] elementArray) {
            BigInteger lcmOfArrayElements = 1;
            int divisor = 2;

            while (true) {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < elementArray.Length; i++) {

                    if (elementArray[i] == 0) {
                        return 0;
                    } else if (elementArray[i] < 0) { elementArray[i] = elementArray[i] * (-1); }
                    if (elementArray[i] == 1) { counter++; }

                    if (elementArray[i] % divisor == 0) {
                        divisible = true;
                        elementArray[i] = elementArray[i] / divisor;
                    }
                }

                if (divisible) {
                    lcmOfArrayElements *= divisor;
                } else { divisor++; }

                if (counter == elementArray.Length) return lcmOfArrayElements;
            }
        }
    }
}
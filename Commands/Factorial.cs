using System.Numerics;

namespace utilities_cs {
    public class Factorial {
        public static string? factorial(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            try {
                int n = int.Parse(args[1]);
                int i = 1;
                BigInteger v = 1;

                while (i <= n) {
                    v *= i;
                    i += 1;
                }

                Utils.CopyCheck(copy, v.ToString());
                Utils.NotifCheck(notif, new string[] { v.ToString(), $"The factorial is: {v.ToString()}", "5" });
                return v.ToString();

            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems that the number you inputted was not a number.",
                        "4"
                    }
                );
                return null;
            }
        }
    }
}
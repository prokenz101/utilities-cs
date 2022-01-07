using System.Numerics;

namespace utilities_cs {
    public class Factorial {
        public static void factorial(string[] args) {
            try {
                int n = int.Parse(args[1]);
                int i = 1;
                BigInteger v = 1;

                while (i <= n) {
                    v *= i;
                    i += 1;
                }
                WindowsClipboard.SetText(v.ToString());
                Utils.Notification(v.ToString(), $"The factorial is: {v.ToString()}");

            } catch {
                Utils.Notification("Huh.", "It seems that the number you inputted was not a number.", 4);
            }
        }
    }
}
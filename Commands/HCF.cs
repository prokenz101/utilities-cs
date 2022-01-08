namespace utilities_cs {
    public class HCF {
        public static int hcf_main(int a, int b) {
            while (b != 0) {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        public static string? GCD(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(
                    args,
                    "Huh.",
                    "It seems you did not input any number for the HCF calculator to work.",
                    4
                )
            ) {
                return null;
            }
            int num1 = int.Parse(args[1]);
            int num2 = int.Parse(args[2]);
            int answer = hcf_main(num1, num2);
            Utils.CopyCheck(copy, answer.ToString());
            Utils.NotifCheck(notif, new string[] { "Success!", $"The answer was {answer}.", "5" });
            return answer.ToString();
        }
    }
}
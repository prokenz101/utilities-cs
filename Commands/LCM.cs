namespace utilities_cs {
    public class lcm_class {
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
            int lcm(int a, int b) {
                return (a / HCF.hcf_main(a, b)) * b;
            }

            int num1 = int.Parse(args[1]);
            int num2 = int.Parse(args[2]);
            int answer = lcm(num1, num2);
            Utils.CopyCheck(copy, answer.ToString());
            Utils.NotifCheck(notif, new string[]{"Success!", $"The answer was {answer}.", "5"});
            return answer.ToString();
        }
    }
}
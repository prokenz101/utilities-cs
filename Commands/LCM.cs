namespace utilities_cs {
    public class lcm_class {
        public static void lcm_main(string[] args) {
            int lcm(int a, int b) {
                return (a / HCF.hcf_main(a, b)) * b;
            }

            int num1 = int.Parse(args[1]);
            int num2 = int.Parse(args[2]);
            int answer = lcm(num1, num2);
            WindowsClipboard.SetText(answer.ToString());
            Utils.Notification("Success!", $"The answer was {answer}.", 5);
        }
    }
}
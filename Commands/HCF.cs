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
        public static void GCD(string[] args) {
            int num1 = int.Parse(args[1]);
            int num2 = int.Parse(args[2]);
            int answer = hcf_main(num1, num2);
            WindowsClipboard.SetText(answer.ToString());
            Utils.Notification("Success!", $"The answer was {answer}.", 5);
        }
    }
}
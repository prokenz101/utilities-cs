using System.Diagnostics;

namespace utilities_cs {
    public class Help {
        public static void Wiki(string[] args) {
            Process.Start(
                new ProcessStartInfo(
                    "cmd",
                    $"/c start https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)"
                ) { CreateNoWindow = true }
            );
        }
    }
}
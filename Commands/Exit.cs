using System.Windows.Forms;

namespace utilities_cs {
    public class UtilitiesExit {
        public static void UtilsExit(string[] args) {
            HookManager.UnregisterAllHooks();
            Application.Exit();
        }
    }
}
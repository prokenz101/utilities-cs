namespace utilities_cs {
    public class Force {
        public static FormattableCommand? forced;
        public static void ForceCommand(string commandName) {
            forced = FormattableCommand.GetFormattableCommand(commandName);
        }

        public static bool AreAnyForced() {
            if (forced != null) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsSpecificCmdForced(string commandName) {
            if (forced!.CommandName == commandName) {
                return true;
            } else {
                return false;
            }
        }

        public static void UnForceCommand() {
            forced = null;
        }
    }
}
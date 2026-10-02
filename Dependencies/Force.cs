namespace utilities_cs {
    public class Force {
        public static FormattableCommand? forced;

        public static void ForceMain(string[] args) {
            string commandName = args[1];
            bool notif = Convert.ToBoolean(args.ElementAtOrDefault(2) ?? "true");

            if (FormattableCommand.FormattableCommandExists(commandName)) {
                ForceCommand(commandName);
                Utils.NotifCheck(notif, ["Success!", "That command has been forced.", "3"], "forceSuccess");
            } else {
                Utils.NotifCheck(
                    notif, ["Exception", "Invalid command, try 'help' for more info.", "3"], "forceError"
                );
            }
        }

        public static void UnforceMain(string[] args) {
            //* check if command is enabled
            if (AreAnyForced()) {
                //* disable command
                Utils.NotifCheck(
                    true,
                    ["Success!", $"The {forced!.CommandName} command has been un-forced.", "3"],
                    "unforceSuccess"
                ); UnForceCommand();
            } else {
                Utils.NotifCheck(
                    true, ["Exception", "Cannot un-force command that is not forced.", "3"], "unforceError"
                );
            }
        }

        public static void ForceCommand(string cmdName) { forced = FormattableCommand.GetFormattableCommand(cmdName); }

        public static bool AreAnyForced() { return forced != null; }

        public static bool IsSpecificCommandForced(string cmdName) { return forced!.CommandName == cmdName; }

        public static void UnForceCommand() { forced = null; }
    }
}
namespace utilities_cs {
    public class Force {
        public static FormattableCommand? forced;

        public static void ForceMain(string[] args) {
            string commandName = args[1];

            //* check if command exists
            if (FormattableCommand.FCommandExists(commandName)) {
                //* check if command is already forced
                if (Force.AreAnyForced()) {
                    if (Force.IsSpecificCmdForced(commandName)) {
                        Utils.NotifCheck(
                            true,
                            new string[] { "Huh.", "It seems that command has already been forced.", "4" },
                            "forceError"
                        ); return;
                    } else {
                        Utils.NotifCheck(
                            true,
                            new string[] { "Huh.", "A command has already been forced.", "3" },
                            "forceError"
                        ); return;
                    }
                } else {
                    //* enable command
                    Force.ForceCommand(commandName);
                    Utils.NotifCheck(true, new string[] { "Success!", "That command has been forced.", "3" }, "forceSuccess");
                }
            } else {
                Utils.NotifCheck(true, new string[] { "Huh.", "That command does not exist.", "3" }, "forceError");
            }
        }

        public static void UnforceMain(string[] args) {
            //* check if command is enabled
            if (Force.AreAnyForced()) {
                //* disable command
                Utils.NotifCheck(
                    true,
                    new string[] { "Success!", $"The {Force.forced!.CommandName} command has been un-forced.", "3" },
                    "unforceSuccess"
                ); Force.UnForceCommand();
            } else {
                Utils.NotifCheck(true, new string[] { "Huh.", "That command was never forced.", "3" }, "unforceError");
            }
        }

        public static void ForceCommand(string cmdName) { forced = FormattableCommand.GetFormattableCommand(cmdName); }

        public static bool AreAnyForced() { return forced != null; }

        public static bool IsSpecificCmdForced(string cmdName) { return forced!.CommandName == cmdName; }

        public static void UnForceCommand() { forced = null; }
    }
}
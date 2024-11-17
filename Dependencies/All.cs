namespace utilities_cs {
    public class All {
        public static string? AllCommand(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string category = args[1];
            string? all = returnCategory(args[1..], category, copy, notif);
            if (all != null) {
                Utils.CopyCheck(copy, all);
                Utils.NotifCheck(notif, ["Success!", "Text copied to clipboard.", "2"], "allSuccess");
                return all;
            } else {
                Utils.NotifCheck(
                    true,
                    ["Exception", "Invalid category inputted, try 'help' for more info.", "4"],
                    "allError"
                );
                return null;
            }
        }
        static string? returnCategory(string[] args, string category, bool copy, bool notif) {
            bool shouldShowNames = UtilitiesAppContext.CurrentSettings.AllCommandHideNames;

            var buildCommandDictionary = (List<FormattableCommand> commands) => (
                    from command in commands
                    select new Tuple<string, Func<string[], bool, bool, string>>(
                        command.CommandName!,
                        command.Function!
                )
            ).ToDictionary(t => t.Item1, t => t.Item2);

            Dictionary<string, Func<string[], bool, bool, string?>> fancy =
                buildCommandDictionary(FormattableCommand.GetMethodsSupportedByAll("fancy"));

            Dictionary<string, Func<string[], bool, bool, string?>> encodings =
                buildCommandDictionary(FormattableCommand.GetMethodsSupportedByAll("encodings"));

            List<string> converted = new();
            Action<Dictionary<string, Func<string[], bool, bool, string?>>> allCommandRun =
                (Dictionary<string, Func<string[], bool, bool, string?>> dict) => {
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in dict) {
                        try {
                            string? output = kvp.Value.Invoke(args, false, false);
                            if (output != null) {
                                if (!shouldShowNames) {
                                    converted.Add($"{kvp.Key}: {output}");
                                } else { converted.Add(output!); }
                            }
                        } catch { }
                    }
                };

            switch (category) {
                case "everything":
                    allCommandRun(
                        fancy.Concat(encodings).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    );
                    break;

                case "encodings":
                    allCommandRun(encodings);
                    break;

                case "fancy":
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in fancy) {
                        try {
                            string? output = kvp.Value.Invoke(args, false, false);
                            if (output != null) {
                                if (!shouldShowNames) {
                                    converted.Add($@"{kvp.Key}: {output}");
                                } else { converted.Add(output); }
                            }
                        } catch { }
                    }
                    break;

                default:
                    return null;

            }

            return string.Join("\n", converted);
        }
    }
}
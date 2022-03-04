namespace utilities_cs {
    public class All {
        public static string? returnCategory(string[] args, string category, bool copy, bool notif) {
            SettingsJSON currentSettings = SettingsModification.GetSettings();
            bool shouldShowNames = currentSettings.allCommandHideNames;

            var buildCommandDictionary = (string[] commands) => (
                    from command in commands
                    select new Tuple<string, Func<string[], bool, bool, string>>(
                        command,
                        FormattableCommand.GetMethod(command.ToLower())!
                )
            ).ToDictionary(t => t.Item1, t => t.Item2);

            Dictionary<string, Func<string[], bool, bool, string?>> fancy = buildCommandDictionary(
                new[] {
                    "Spacer", "UpperCase", "LowerCase", "Cursive", "BubbleText",
                    "DoubleStruck", "Creepy", "Reversed", "Exponent", "Flipped", "TitleCase", "Morse", "MathItalic"
                }
            );

            Dictionary<string, Func<string[], bool, bool, string?>> encodings = buildCommandDictionary(
                new[] {
                    "Binary", "Hexadecimal", "Base64", "Base32", "GZip", "SHA1", "SHA256", "SHA384", "SHA512",
                }
            );

            List<string> b32AndGZipArgs = args[1..].ToList();
            b32AndGZipArgs.Insert(0, "x");
            b32AndGZipArgs.Insert(1, "to");

            List<string> converted = new();
            Action<Dictionary<string, Func<string[], bool, bool, string?>>> base32AndGZIPCheck =
                (Dictionary<string, Func<string[], bool, bool, string?>> dict) => {
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in dict) {
                        if (kvp.Key == "GZip" | kvp.Key == "Base32") {
                            try {
                                string? output = kvp.Value.Invoke(b32AndGZipArgs.ToArray(), false, false);
                                if (output != null) {
                                    if (!shouldShowNames) {
                                        converted.Add($"{kvp.Key}: {output}");
                                    } else { converted.Add(output); }
                                }
                            } catch { }
                        } else {
                            try {
                                string? output = kvp.Value.Invoke(args, false, false);
                                if (output != null) {
                                    if (!shouldShowNames) {
                                        converted.Add($"{kvp.Key}: {output}");
                                    } else { converted.Add(output!); }
                                }
                            } catch { }
                        }
                    }
                };

            switch (category) {
                case "everything":
                    base32AndGZIPCheck(
                        fancy.Concat(encodings).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    ); break;
                case "encodings": base32AndGZIPCheck(encodings); break;
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
namespace utilities_cs {
    public class SettingsModification {
        public static SettingsJSON defaultSettings = new SettingsJSON {
            DisableNotifications = false,
            DisableClipboardManipulation = false,
            PermutationsCalculationLimit = 6,
            EscapeBase85OutputText = true,
            CopyingHotkeyDelay = 25,
            AutoPaste = false,
            PressEscape = true,
            AllCommandHideNames = false
        };

        static string settingsJsonPath = Path.Combine(Program.UtilitiesCsFolder, "settings.json");

        public static void SettingsMain(string[] args) {
                        Utils.NotifCheck(true,
                            new string[] {
                                "Huh.",
                                "It seems you did not input a setting/value.",
                                "3"
                            },
                            "settingsError"
                        );
            try {
                string mode = args[1];
                switch (mode) {
                    case "modify":
                        try {
                            string setting = args[2];
                            string value = args[3];
                            SettingsJSON currentSettings = GetSettings();
                            ModifySetting(currentSettings, setting, value);
                        } catch (IndexOutOfRangeException) {
                        }
                        break;

                    case "reset":
                        CreateDirectoryAndJson();
                        Utils.NotifCheck(
                            true,
                            ["Reset.", "All settings have been reset to default.", "4"],
                            "settingsReset"
                        ); break;

                    case "list":
                        string settings = ListAllSettings();
                        Utils.CopyCheck(true, settings);
                        Utils.NotifCheck(
                            true,
                            ["Success!", "The settings have been copied to your clipboard.", "3"],
                            "settingsCopy"
                        ); break;

                    case "open":
                        Utils.NotifCheck(
                            true,
                            ["Opening settings file.", "Opening in your default editor.", "3"],
                            "settingsOpen"
                        ); OpenSettingsJSON(); break;

                    case "refresh":
                        if (
                            GetSettings().DisableClipboardManipulation
                            && GetSettings().AutoPaste
                        ) {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    @"disableClipboardManipulation and autoPaste are mutually exclusive.
They cannot both be true at the same time."
                                ], "settingsError"
                            ); break;
                        } else {
                            UtilitiesAppContext.CurrentSettings = GetSettings();
                            Utils.NotifCheck(
                                true,
                                ["Refreshed.", "Settings have been refreshed.", "3"],
                                "settingsRefresh"
                            ); break;
                        }

                    default:
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid mode given, try 'help' for more info.", "3"],
                            "settingsError"
                        ); break;
                    }

                default:
                    Utils.NotifCheck(
                        true,
                        new string[] { "Huh.", "It seems that was not a valid mode.", "3" },
                        "settingsError"
                    ); break;
            }
        }

        public static SettingsJSON GetSettings() {
            try {
                string jsonString = File.ReadAllText(settingsJsonPath);
                SettingsJSON settings = System.Text.Json.JsonSerializer.Deserialize<SettingsJSON>(jsonString)!;
                return settings;
            } catch {
                CreateDirectoryAndJson();
                Thread.Sleep(500);
                return GetSettings();
            }
        }

        public static void ModifySetting(SettingsJSON currentSettings, string setting, string value) {
            Action mutuallyExclusive = () => {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "You can't do that unfortunately.",
                        @"'disableClipboardManipulation' and 'autoPaste' are mutually exclusive.",
                        "7"
                    }, "settingsError"
                );
            };

            switch (setting.ToLower()) {
                case "disablenotifications":
                    currentSettings.DisableNotifications = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;

                case "disableclipboardmanipulation":
                    if (!currentSettings.AutoPaste) {
                        currentSettings.DisableClipboardManipulation = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    } break;

                case "permutationscalculationlimit":
                    currentSettings.PermutationsCalculationLimit = int.Parse(ConvertToBoolOrInt("int", value)!.ToString()!);
                    break;

                case "escapebase85outputtext":
                    currentSettings.EscapeBase85OutputText = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;

                case "copyinghotkeydelay":
                    currentSettings.CopyingHotkeyDelay = int.Parse(ConvertToBoolOrInt("int", value)!.ToString()!);
                    break;

                case "autopaste":
                    if (!currentSettings.DisableClipboardManipulation) {
                        currentSettings.AutoPaste = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    } break;

                case "pressescape":
                    currentSettings.PressEscape = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;

                case "allcommandhidenames":
                    currentSettings.AllCommandHideNames = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;

            }

            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(currentSettings);
            try {
                File.WriteAllText(settingsJsonPath, jsonString);
            } catch (DirectoryNotFoundException) {
                CreateDirectoryAndJson();
            }

            UtilitiesAppContext.CurrentSettings = SettingsModification.GetSettings();
            Utils.NotifCheck(
                true,
                ["Modified.", $"'{setting}' has been changed to {value}.", "4"],
                "settingsModifiedSuccess"
            );
        }

        public static void OpenSettingsJSON() {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(
                    "cmd",
                    $"/c start {settingsJsonPath}"
                ) { CreateNoWindow = true }
            );
        }

        public static string ListAllSettings() {
            List<string> settings = [];
            foreach (var i in defaultSettings.GetType().GetProperties()) {
                settings.Add($"{i.Name}: {i.GetValue(UtilitiesAppContext.CurrentSettings)}");
            }

            string allSettings = string.Join("\n", settings);
            return allSettings;
        }

        public static void CreateDirectoryAndJson() {
            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(defaultSettings);
            Directory.CreateDirectory(Program.UtilitiesCsFolder);
            File.WriteAllText(settingsJsonPath, jsonString);

            UtilitiesAppContext.CurrentSettings = SettingsModification.GetSettings();
        }

        static object? ConvertToBoolOrInt(string boolOrInt, string value) {
            try {
                if (boolOrInt == "bool") {
                    return Convert.ToBoolean(value);
                } else if (boolOrInt == "int") {
                    return int.Parse(value);
                } else {
                    return null;
                }
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] { "Huh", "It seems you did not input the parameters correctly.", "3" },
                    "settingsError"
                ); return null;
            }
        }
    }

    public class SettingsJSON {
        public bool DisableNotifications { get; set; }
        public bool DisableClipboardManipulation { get; set; }
        public int PermutationsCalculationLimit { get; set; }
        public bool EscapeBase85OutputText { get; set; }
        public int CopyingHotkeyDelay { get; set; }
        public bool AutoPaste { get; set; }
        public bool PressEscape { get; set; }
        public bool AllCommandHideNames { get; set; }
    }
}
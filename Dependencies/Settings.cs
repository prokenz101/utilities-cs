namespace utilities_cs {
    public class SettingsModification {
        public static SettingsJSON defaultSettings = new SettingsJSON {
            disableNotifications = false,
            disableClipboardManipulation = false,
            sendTypingDelay = 0,
            copyingHotkeyDelay = 25,
            autoPaste = false,
            pressEscape = true,
            allCommandHideNames = false
        };

        public static string utilitiesCsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "utilities-cs"
        );

        public static string settingsJsonPath = Path.Combine(utilitiesCsFolder, "settings.json");

        public static void SettingsMain(string[] args) {
            string mode = args[1];
            switch (mode) {
                case "modify":
                    try {
                        string setting = args[2];
                        string value = args[3];
                        SettingsJSON currentSettings = SettingsModification.GetSettings();
                        SettingsModification.ModifySetting(currentSettings, setting, value);
                    } catch (IndexOutOfRangeException) {
                        Utils.NotifCheck(true,
                            new string[] {
                                        "Huh.",
                                        "It seems you did not input a setting/value.",
                                        "3"
                            }
                        );
                    }
                    break;

                case "reset":
                    SettingsModification.CreateDirAndJson();
                    Utils.NotifCheck(
                        true,
                        new string[] { "Reset.", "All settings have been reset to default.", "4" }
                    ); break;

                case "list":
                    string settings = SettingsModification.ListAllSettings();
                    Utils.CopyCheck(true, settings);
                    Utils.NotifCheck(
                        true,
                        new string[] { "Success!", "The settings have been copied to your clipboard.", "3" }
                    );
                    break;

                case "open":
                    Utils.NotifCheck(
                        true,
                        new string[] {
                                    "Opening settings file.",
                                    "Opening settings.json in your default editor.",
                                    "3"
                        }
                    );
                    SettingsModification.OpenSettingsJSON();
                    break;

                case "refresh":
                    if (
                        SettingsModification.GetSettings().disableClipboardManipulation
                        && SettingsModification.GetSettings().autoPaste
                    ) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                        "Hey!",
                                        @"disableClipboardManipulation and autoPaste are mutually exclusive.
They cannot both be true at the same time."
                            }
                        ); break;
                    } else {
                        UtilitiesAppContext.currentSettings = SettingsModification.GetSettings();
                        Utils.NotifCheck(true, new string[] { "Refreshed.", "Settings have been refreshed.", "3" });
                        break;
                    }

                default:
                    Utils.NotifCheck(true, new string[] { "Huh.", "It seems that was not a valid mode.", "3" });
                    break;
            }
        }

        public static SettingsJSON GetSettings() {
            try {
                string jsonString = File.ReadAllText(settingsJsonPath);
                SettingsJSON settings = System.Text.Json.JsonSerializer.Deserialize<SettingsJSON>(jsonString)!;
                return settings;
            } catch {
                CreateDirAndJson();
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
                    }
                );
            };

            switch (setting.ToLower()) {
                case "disablenotifications":
                    currentSettings.disableNotifications = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
                case "disableclipboardmanipulation":
                    if (!currentSettings.autoPaste) {
                        currentSettings.disableClipboardManipulation = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    }
                    break;
                case "sendtypingdelay":
                    currentSettings.sendTypingDelay = int.Parse(ConvertToBoolOrInt("int", value)!.ToString()!);
                    break;
                case "copyinghotkeydelay":
                    currentSettings.copyingHotkeyDelay = int.Parse(ConvertToBoolOrInt("int", value)!.ToString()!);
                    break;
                case "autopaste":
                    if (!currentSettings.disableClipboardManipulation) {
                        currentSettings.autoPaste = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    }
                    break;
                case "pressescape":
                    currentSettings.pressEscape = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
                case "allcommandhidenames":
                    currentSettings.allCommandHideNames = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
            }

            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(currentSettings);
            try {
                File.WriteAllText(settingsJsonPath, jsonString);
            } catch (DirectoryNotFoundException) {
                CreateDirAndJson();
            }

            UtilitiesAppContext.currentSettings = SettingsModification.GetSettings();
            Utils.NotifCheck(true, new string[] { "Modified.", $"'{setting}' has been changed to {value}.", "4" });
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
            List<string> settings = new();
            foreach (var i in defaultSettings.GetType().GetProperties()) {
                settings.Add(i.Name);
            }

            string allSettings = string.Join("\n", settings);
            return allSettings;
        }

        public static void CreateDirAndJson() {
            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(defaultSettings);
            Directory.CreateDirectory(SettingsModification.utilitiesCsFolder);
            File.WriteAllText(settingsJsonPath, jsonString);

            UtilitiesAppContext.currentSettings = SettingsModification.GetSettings();
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
                Utils.NotifCheck(true, new string[] { "Huh", "It seems you did not input the parameters correctly.", "3" });
                return null;
            }
        }
    }

    public class SettingsJSON {
        public bool disableNotifications { get; set; }
        public bool disableClipboardManipulation { get; set; }
        public int sendTypingDelay { get; set; }
        public int copyingHotkeyDelay { get; set; }
        public bool autoPaste { get; set; }
        public bool pressEscape { get; set; }
        public bool allCommandHideNames { get; set; }
    }
}
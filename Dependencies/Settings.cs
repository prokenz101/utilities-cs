namespace utilities_cs {
    public class SettingsModifification {
        public static string utilitiesCsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "utilities-cs"
        );
        public static string settingsJsonPath = Path.Combine(utilitiesCsFolder, "settings.json");
        public static SettingsJSON getSettings() {
            try {
                string jsonString = File.ReadAllText(settingsJsonPath);
                SettingsJSON settings = System.Text.Json.JsonSerializer.Deserialize<SettingsJSON>(jsonString)!;
                return settings;
            } catch {
                createDirAndJson();
                Thread.Sleep(500);
                return getSettings();
            }
        }
        public static void modifySetting(SettingsJSON currentSettings, string setting, string value) {
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

            switch (setting) {
                case "disableNotifications":
                    currentSettings.disableNotifications = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
                case "disableClipboardManipulation":
                    if (!currentSettings.autoPaste) {
                        currentSettings.disableClipboardManipulation = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    }
                    break;
                case "copyingHotkeyDelay":
                    currentSettings.copyingHotkeyDelay = int.Parse(ConvertToBoolOrInt("int", value)!.ToString()!);
                    break;
                case "autoPaste":
                    if (!currentSettings.disableClipboardManipulation) {
                        currentSettings.autoPaste = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    } else {
                        mutuallyExclusive.Invoke();
                        return;
                    }
                    break;
                case "pressEscape":
                    currentSettings.pressEscape = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
                case "allCommandHideNames":
                    currentSettings.allCommandHideNames = Convert.ToBoolean(ConvertToBoolOrInt("bool", value));
                    break;
            }

            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(currentSettings);
            try {
                File.WriteAllText(settingsJsonPath, jsonString);
            } catch (DirectoryNotFoundException) {
                createDirAndJson();
            }

            Utils.NotifCheck(true, new string[] { "Modified.", $"'{setting}' has been changed to {value}.", "4" });
        }
        public static void createDirAndJson() {
            SettingsJSON defaultSettings = new SettingsJSON {
                disableNotifications = false,
                disableClipboardManipulation = false,
                copyingHotkeyDelay = 25,
                autoPaste = false,
                pressEscape = true,
                allCommandHideNames = false
            };

            string jsonString = System.Text.Json.JsonSerializer.Serialize<SettingsJSON>(defaultSettings);
            Directory.CreateDirectory(SettingsModifification.utilitiesCsFolder);
            File.WriteAllText(settingsJsonPath, jsonString);
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
        public int copyingHotkeyDelay { get; set; }
        public bool autoPaste { get; set; }
        public bool pressEscape { get; set; }
        public bool allCommandHideNames { get; set; }
    }
}
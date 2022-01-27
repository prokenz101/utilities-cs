using System;
using System.IO;
using System.Threading;
using System.Text.Json;

namespace utilities_cs {
    public class Settings {
        public static void SettingsManager(string[] args) {
            string mode = args[1];

            if (mode == "modify") {
                try {
                    string setting = args[2];
                    string value = args[3];
                    SettingsJSON currentSettings = SettingsModifification.getSettings();
                    SettingsModifification.modifySettings(currentSettings, setting, value);
                } catch (IndexOutOfRangeException) {
                    Utils.NotifCheck(true, new string[] { "Huh.", "It seems you did not input a setting/value.", "3" });
                }

            } else if (mode == "reset") {
                SettingsModifification.createDirAndJson();
                Utils.NotifCheck(true, new string[] { "Reset.", "All settings have been reset to default.", "4" });
            } else {
                Utils.NotifCheck(true, new string[] { "Huh.", "It seems that was not a valid mode.", "3" });
            }
        }
    }

    public class SettingsModifification {
        public static string utilitiesCsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "utilities-cs"
        );
        public static string settingsJsonPath = Path.Combine(utilitiesCsFolder, "settings.json");
        public static SettingsJSON getSettings() {
            try {
                string jsonString = File.ReadAllText(settingsJsonPath);
                SettingsJSON settings = JsonSerializer.Deserialize<SettingsJSON>(jsonString);
                return settings;
            } catch (DirectoryNotFoundException) {
                createDirAndJson();
                Thread.Sleep(500);
                return getSettings();
            }
        }
        public static void modifySettings(SettingsJSON currentSettings, string setting, string value) {
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
            }

            string jsonString = JsonSerializer.Serialize<SettingsJSON>(currentSettings);
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
                pressEscape = true
            };

            string jsonString = JsonSerializer.Serialize<SettingsJSON>(defaultSettings);
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
    }
}
namespace utilities_cs {
    class Program {
        public const string Version = "v1.12";
        public const BuildMode buildMode = BuildMode.FrameworkDependent;
        public static string UtilitiesCsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "utilities-cs"
        );

        [STAThread]
        /// <summary>
        /// The main starting point of the program.
        /// </summary>
        static void Main(string[] args) {
            RegisterCommands.RegisterAllRCommands();
            RegisterCommands.RegisterAllFCommands();

#if UTILITIES_DEBUG
            //* debug mode, only used for specific times
            string? copied_text = UtilitiesAppContext.Utilities(args);
            if (copied_text != null) { Console.WriteLine(copied_text); }
#else
            Utils.NotifCheck(
                true,
                new string[] {
                    "Opened utilities-cs.",
                    "I am now in your system tray, right click me and press Exit to exit.",
                    "3"
                }, "utilities-csOpen"
            );

            CheckForUpdates();

            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            var app = new UtilitiesAppContext();
            Application.ApplicationExit += delegate {
                app.Exit();
            };

            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.OnActivated += toastArgs => {
                string key = toastArgs.Argument.Split("=")[0];
                string value = toastArgs.Argument.Split("=")[1];
                List<KeyValuePair<string, object>>? userInput =
                    toastArgs.UserInput.Count > 0 ? toastArgs.UserInput.ToList()
                    : null;

                switch (key) {
                    case "update": Update.HandleOnActivatedToast(value); break;

                    case "spam": Spam.HandleOnActivatedToast(value, userInput); break;

                    case "remind":
                        if (value == "dismiss") {
                            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.History.Remove("reminder");
                        } break;
                }
            };

            Application.Run(app);
#endif
        }

        public enum BuildMode {
            FrameworkDependent,
            SelfContained
        }

        async static void CheckForUpdates() {
            await Task.Run(
                () => {
                    Thread.Sleep(4000);
                    Update.Check(alertEvenIfUpdateIsNotRequired: false);
                }
            );
        }
    }

    public class UtilitiesAppContext : ApplicationContext {
        /// <summary>
        /// The main method that is called when a command is executed.
        /// </summary>
        /// <param name="args">The arguments for the command being run.</param>
        /// <returns>Returns null or a string of the output of the command.</returns>
        public static string? Utilities(string[] args) {
            string? output = Command.ExecuteCommand(args);
            if (output != null) { return output; } else { return null; }
        }

        private NotifyIcon trayIcon;

        /// <summary>
        /// Current settings of the program.
        /// </summary>
        public static SettingsJSON CurrentSettings = SettingsModification.GetSettings();

        /// <summary>
        /// Constructor for the application context.
        /// </summary>
        public UtilitiesAppContext() {
            //* making keyboard hook for ctrl + f8
            HookManager.AddHook(
                "utilities-cs-ctrlF8",
                new ModifierKeys[] { ModifierKeys.Control },
                Keys.F8,
                async () => {
                    int hotkeyDelay = CurrentSettings.CopyingHotkeyDelay;
                    bool pressEscape = CurrentSettings.PressEscape;

                    SendKeys.Send("^a");
                    Thread.Sleep(hotkeyDelay);
                    SendKeys.Send("^c");
                    Thread.Sleep(hotkeyDelay);
                    if (pressEscape) { SendKeys.Send("{ESC}"); }
                    string[] args = Clipboard.GetText().Split(" ");
                    await Task.Run(() => Utilities(args));
                },
                onFail: () => {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.",
                            @"Are you opening multiple instances of utilities-cs?",
                            "6"
                        }, "utilitiesHotkeyError"
                    ); Exit();
                }
            );

            //* creating tray icon
            trayIcon = new NotifyIcon() { Text = "utilities-cs" };
            var menu = new ContextMenuStrip();

            menu.Items.Add(
                "Wiki...", null, delegate {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        "cmd", $"/c start https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki"
                    ) { CreateNoWindow = true });
                }
            );

            menu.Items.Add(
                "Settings...", null, delegate {
                    Utils.NotifCheck(
                        true,
                        new string[] { "Opening settings.json...", "Opening settings.json on your default editor.", "3" },
                        "utiliitesOpeningSettings"
                    ); SettingsModification.OpenSettingsJSON();
                }
            );

            menu.Items.Add("Exit", null, delegate { Exit(); });

            trayIcon.Icon = Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!);
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void Exit() {
            trayIcon.Visible = false;
            HookManager.UnregisterAllHooks();
            Application.Exit();
        }
    }
}
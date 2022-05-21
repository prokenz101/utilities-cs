namespace utilities_cs {
    class Program {
        [STAThread]
        /// <summary>
        /// The main starting point of the program.
        /// </summary>
        static void Main(string[] args) {
            Utils.NotifCheck(
                true,
                new string[] {
                    "Opened utilities-cs.",
                    "I am now in your system tray, right click me and press Exit to exit.",
                    "3"
                }, "utilities-csOpen"
            );

            RegisterCommands.RegisterAllRCommands();
            RegisterCommands.RegisterAllFCommands();

#if UTILITIES_DEBUG
            //* debug mode, only used for specific times
            string? copied_text = UtilitiesAppContext.Utilities(args);
            if (copied_text != null) { Console.WriteLine(copied_text); }
#else
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            var app = new UtilitiesAppContext();
            Application.ApplicationExit += delegate {
                app.Exit();
            };

            Application.Run(app);
#endif
        }
    }

    public class UtilitiesAppContext : ApplicationContext {
        /// <summary>
        /// The main method that is called when a command is executed.
        /// </summary>
        /// <param name="args">The arguments for the command being run.</param>
        /// <returns>Returns null or a string of the output of the command.</returns>
        public static string? Utilities(string[] args) {
            var cmd = args[0].ToLower();
            string? output = Command.ExecuteCommand(cmd, args);
            if (output != null) { return output; } else { return null; }
        }
        private NotifyIcon trayIcon;

        /// <summary>
        /// Current settings of the program.
        /// </summary>
        public static SettingsJSON currentSettings = SettingsModification.GetSettings();

        /// <summary>
        /// Constructor for the application context.
        /// </summary>
        public UtilitiesAppContext() {
            //* making keyboard hook for ctrl + f8
            HookManager.AddHook(
                "utilities",
                new ModifierKeys[] { ModifierKeys.Control },
                Keys.F8,
                () => {
                    int hotkeyDelay = currentSettings.copyingHotkeyDelay;
                    bool pressEscape = currentSettings.pressEscape;

                    SendKeys.Send("^a");
                    Thread.Sleep(hotkeyDelay);
                    SendKeys.Send("^c");
                    Thread.Sleep(hotkeyDelay);
                    if (pressEscape) { SendKeys.Send("{ESC}"); }
                    string[] args = Clipboard.GetText().Split(" ");
                    Utilities(args);
                },
                () => {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.",
                            @"Are you opening multiple instances of utilities-cs?",
                            "6"
                        },
                        "utilitiesHotkeyError"
                    );
                    Exit();
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
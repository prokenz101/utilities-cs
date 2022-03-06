namespace utilities_cs {
    class Program {
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
        public static string? Utilities(string[] args) {
            var cmd = args[0].ToLower();
            string? output = Command.ExecuteCommand(cmd, args);
            if (output != null) { return output; } else { return null; }
        }
        private NotifyIcon trayIcon;

        public static SettingsJSON currentSettings = SettingsModification.GetSettings();

        public UtilitiesAppContext() {

            Action registerHotkeyFailed = () => {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Something went wrong.",
                        @"Are you opening multiple instances of utilities-cs?",
                        "6"
                    }
                );
                Exit();
            };

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
                registerHotkeyFailed
            );

            //* creating tray icon
            trayIcon = new NotifyIcon() {
                Text = "utilities-cs"
            };
            var menu = new ContextMenuStrip();

            menu.Items.Add("Wiki...", null, delegate {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                "cmd", $"/c start https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)"
            ) { CreateNoWindow = true });
            }
            );
            menu.Items.Add("Exit", null, delegate { Exit(); });

            trayIcon.Icon = Icon.ExtractAssociatedIcon(
                (System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!)
            );
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
        }

        public void Exit() {
            trayIcon.Visible = false;
            HookManager.UnregisterAllHooks();
            Application.Exit();
        }
    }
}
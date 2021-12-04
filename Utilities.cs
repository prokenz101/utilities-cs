using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace utilities_cs {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            var app = new UtilitiesAppContext();
            Application.ApplicationExit += delegate {
                app.Exit();
            };

            Application.Run(app);
        }

    }
    public class UtilitiesAppContext : ApplicationContext {
        public static Dictionary<string, Action<string[]>> commands = new()
        {
            { "sarcasm", Sarcasm.Sarcasm_ },
            { "copypaste", Copypaste.cp },
            { "cp", Copypaste.cp },
            { "upper", Upper.Uppercase },
            { "uppercase", Upper.Uppercase },
            { "lower", Lower.Lowercase },
            { "lowercase", Lower.Lowercase },
            { "cursive", Cursive.cursive },
            { "bubbletext", Bubble.bubbletext },
            { "bubble", Bubble.bubbletext },
            { "doublestruck", Doublestruck.dbs },
            { "dbs", Doublestruck.dbs },
            { "creepy", Creepy.creepy },
            { "-", BrowserSearch.GoogleSearch },
            { "youtube", BrowserSearch.YouTubeSearch },
            { "yt", BrowserSearch.YouTubeSearch },
            { "images", BrowserSearch.ImageSearch },
            { "spacer", Spacer.spacer },
            { "spoilerspam", Spoilerspam.spoilerspam },
            { "reverse", Reverse.reverse },
            { "exponent", Exponent.exponent },
            { "ep", Exponent.exponent },
            { "flip", Flip.flip },
            { "autoclick", Autoclick.autoclick }
        };
        public static void Utilities(string[] args) {
            var cmd = args[0].ToLower();
            var f = commands.GetValueOrDefault(cmd, (args) => Utils.Notification(
                    "Welp.",
                    "It seems utilities couldn't understand what command you were trying to use.",
                    6
                )
            );
            f.Invoke(args);
        }
        private NotifyIcon trayIcon;

        public UtilitiesAppContext() {
            // making keyboard hook
            
            HookManager.AddHook(
                "utilities",
                ModifierKeys.Control,
                Keys.F8,
                () => {
                SendKeys.SendWait("^a");
                SendKeys.Send("^c");
                SendKeys.Send("{ESC}");
                string[] args = Clipboard.GetText().Split(" ");
                Utilities(args);
            },
                () => {
                Utils.Notification(
                    "Something went wrong.",
                    @"utilities-cs was unable to register a hotkey.
This could be because you have multiple verions of the application running.",
                    6
                );
                Exit();
            }
            );

            // creating tray icon

            trayIcon = new NotifyIcon() {
                Text = "utilities-cs"
            };
            var menu = new ContextMenuStrip();

            menu.Items.Add("Help Center", null, delegate {
                Utils.Notification("lmfao", "you thought there was a helpcenter?", 3);
            }
            );
            menu.Items.Add("Exit", null, delegate { Exit(); });

            trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
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

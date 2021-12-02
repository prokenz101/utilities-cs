using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace utilities_cs
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            var app = new UtilitiesAppContext();
            Application.ApplicationExit += delegate
            {
                app.Exit();
            };

            Application.Run(app);
        }

    }
    public class UtilitiesAppContext : ApplicationContext
    {
        public static Dictionary<string, Action<string[]>> commands = new()
        {
            { "cursive", Cursive.cursive },
            { "copypaste", Copypaste.cp },
            { "cp", Copypaste.cp },
            { "upper", Upper.Uppercase },
            { "lower", Lower.Lowercase },
            { "sarcasm", Sarcasm.Sarcasm_ }
        };
        public static void Utilities(string[] args)
        {
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
        private KeyboardHook hook;

        public UtilitiesAppContext()
        {
            hook = new KeyboardHook();
            hook.KeyPressed += delegate
            {
                SendKeys.SendWait("^a");
                SendKeys.Send("^c");
                SendKeys.Send("{ESC}");
                string[] args = Clipboard.GetText().Split(" ");
                Utilities(args);
            };
            hook.RegisterHotKey(ModifierKeys.Control, Keys.F9);

            trayIcon = new NotifyIcon()
            {
                Text = "utilities-cs"
            };
            var menu = new ContextMenuStrip();

            menu.Items.Add("Help Center", null, delegate
            {
                Utils.Notification("lmfao", "you thought there was a helpcenter?", 3);
            }
            );
            menu.Items.Add("Exit", null, delegate { Exit(); });

            trayIcon.Icon = new System.Drawing.Icon(@"C:\Items\Code\utilities-cs\csharp.ico");
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
        }

        public void Exit()
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }

}

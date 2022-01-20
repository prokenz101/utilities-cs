using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace utilities_cs {
    class Program {
        [STAThread]
        static void Main(string[] args) {
#if UTILITIES_DEBUG
            // debug mode, only used for specific times
            UtilitiesAppContext.Utilities(args);
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
        public static Dictionary<string, Action<string[]>> commands = new() {
            { "help", Help.help },
            { "-", BrowserSearch.GoogleSearch },
            { "youtube", BrowserSearch.YouTubeSearch },
            { "yt", BrowserSearch.YouTubeSearch },
            { "images", BrowserSearch.ImageSearch },
            { "autoclick", Autoclick.autoclick },
            { "translate", Translate.Translator },
            { "format", Formatter.formatter },
            { "notification", Notification.Notify },
            { "notify", Notification.Notify },
            { "notif", Notification.Notify },
            { "exit", UtilitiesExit.UtilsExit },
            { "quit", UtilitiesExit.UtilsExit }

        };
        public static Dictionary<string, Func<string[], bool, bool, string?>> formattable_commands = new() {
            { "sarcasm", Sarcasm.Sarcasm_ },
            { "spacer", Spacer.spacer },
            { "copypaste", Copypaste.cp },
            { "cp", Copypaste.cp },
            { "upper", Upper.Uppercase },
            { "uppercase", Upper.Uppercase },
            { "lower", Lower.Lowercase },
            { "lowercase", Lower.Lowercase },
            { "cursive", Cursive.cursive },
            { "bubbletext", Bubble.BubbleText },
            { "bubble", Bubble.BubbleText },
            { "doublestruck", Doublestruck.dbs },
            { "dbs", Doublestruck.dbs },
            { "creepy", Creepy.creepy },
            { "spoilerspam", Spoilerspam.spoilerspam },
            { "reverse", Reverse.reverse },
            { "exponent", Exponent.exponent },
            { "ep", Exponent.exponent },
            { "flip", Flip.flip },
            { "flipped", Flip.flip },
            { "upside-down", Flip.flip },
            { "fc", Fraction.fraction },
            { "lcm", lcm_class.lcm_main },
            { "hcf", HCF.hcf_main },
            { "fraction", Fraction.fraction },
            { "binary", Binary.Bin },
            { "bin", Binary.Bin },
            { "hexadecimal", Hex.Hexadecimal },
            { "hex", Hex.Hexadecimal },
            { "base64", Base64Conversion.Base64Convert },
            { "base32", Base32Conversion.Base32Convert },
            { "emojify", Emojify.emojify },
            { "title", Title.title },
            { "titlecase", Title.title },
            { "factorial", Factorial.factorial },
            { "morse", Morse.MorseCodeTranslate },
            { "commaseperator", CommaSeperator.Cms },
            { "cms", CommaSeperator.Cms },
            { "percent", Percentage.Percent },
            { "%", Percentage.Percent },
            { "percentage", Percentage.Percent },
            { "mathitalic", MathItalics.MathItalic },
            { "mai", MathItalics.MathItalic },
            { "randint", RandInt.Randint },
            { "randnum", RandInt.Randint },
            { "randchar", Randchar.RandomChar }
        };
        public static void Utilities(string[] args) {
            var cmd = args[0].ToLower();
            if (commands.ContainsKey(cmd)) {
                Action<string[]> f = commands[cmd];
                f.Invoke(args);
            } else if (formattable_commands.ContainsKey(cmd)) {
                Func<string[], bool, bool, string?> f = formattable_commands[cmd];
                f.Invoke(args, true, true);
            } else {
                Utils.Notification(
                    "Welp.",
                    "It seems utilities couldn't understand what command you were trying to use.",
                    6
                );
            }

        }
        private NotifyIcon trayIcon;

        public UtilitiesAppContext() {
            // making keyboard hook for ctrl + f8

            HookManager.AddHook(
                "utilities",
                ModifierKeys.Control,
                Keys.F8,
                () => {
                    SendKeys.Send("^a");
                    Thread.Sleep(25);
                    SendKeys.Send("^c");
                    Thread.Sleep(25);
                    SendKeys.Send("{ESC}");
                    string[] args = Clipboard.GetText().Split(" ");
                    Utilities(args);
                },
                () => {
                    Utils.Notification(
                        "Something went wrong.",
                        @"Are you opening multiple instances of utilities-cs?",
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

            menu.Items.Add("Wiki...", null, delegate {
                Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)"
            ) { CreateNoWindow = true });
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

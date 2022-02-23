using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace utilities_cs {
    class Program {
        [STAThread]
        static void Main(string[] args) {
#if UTILITIES_DEBUG
            // debug mode, only used for specific times
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
        public static Dictionary<string, Action<string[]>> commands = new() {
            { "help", Help.Wiki },
            { "settings", Settings.SettingsManager },
            { "-", BrowserSearch.GoogleSearch },
            { "youtube", BrowserSearch.YouTubeSearch },
            { "yt", BrowserSearch.YouTubeSearch },
            { "images", BrowserSearch.ImageSearch },
            { "autoclick", Autoclick.autoclick },
            { "translate", Translate.Translator },
            { "format", Formatter.Format },
            { "remind", Reminder.Remind },
            { "reminder", Reminder.Remind },
            { "notification", Notification.Notify },
            { "notify", Notification.Notify },
            { "notif", Notification.Notify },
            { "exit", UtilitiesExit.UtilsExit },
            { "quit", UtilitiesExit.UtilsExit }
        };
        public static Dictionary<string, Func<string[], bool, bool, string?>> formattable_commands = new() {
            { "all", All.ReturnAll },
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
            { "sha1", SHAHashing.SHA1Hash },
            { "sha256", SHAHashing.SHA256Hash },
            { "sha384", SHAHashing.SHA384Hash },
            { "sha512", SHAHashing.SHA512Hash },
            { "base64", Base64Conversion.Base64Convert },
            { "base32", Base32Conversion.Base32Convert },
            { "gzip", GZip.GZipConversion },
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
            { "cuberoot", CubeRoot.Cbrt },
            { "cbrt", CubeRoot.Cbrt },
            { "mathitalic", MathItalics.MathItalic },
            { "mai", MathItalics.MathItalic },
            { "randint", RandInt.Randint },
            { "randnum", RandInt.Randint },
            { "randchar", Randchar.RandomChar }
        };
        public static string? Utilities(string[] args) {
            var cmd = args[0].ToLower();
            if (commands.ContainsKey(cmd)) {
                Action<string[]> f = commands[cmd];
                f.Invoke(args);
                return null;
            } else if (formattable_commands.ContainsKey(cmd)) {
                Func<string[], bool, bool, string?> f = formattable_commands[cmd];
                string? final_string = f.Invoke(args, true, true);
                if (final_string != null) {
                    return final_string;
                } else return null;
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Welp.",
                        "It seems utilities couldn't understand what command you were trying to use.",
                        "6"
                    }
                );
                return null;
            }
        }
        private NotifyIcon trayIcon;
        public UtilitiesAppContext() {
            SettingsJSON currentSettings = SettingsModifification.getSettings();
            int hotkeyDelay = currentSettings.copyingHotkeyDelay;
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

            // making keyboard hook for ctrl + f8
            HookManager.AddHook(
                "utilities",
                new ModifierKeys[] { ModifierKeys.Control },
                Keys.F8,
                () => {
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
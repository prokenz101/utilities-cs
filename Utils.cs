using System.Text.RegularExpressions;

namespace utilities_cs {
    public partial class Utils {

        [GeneratedRegex(@"(?<num>-?\d+)+")]
        private static partial Regex FindAllIntsRegex();

        [GeneratedRegex(@"-?\d+\.(?:9){6,}")]
        private static partial Regex RoundIfNumberIsNearEnoughRegex();

        /// <summary>
        /// The primary notification method, used to send a Windows Toast Notification.
        /// </summary>
        /// <param name="title">The title of the notificaton.</param>
        /// <param name="subtitle">The subtitle of the notification.</param>
        /// <param name="tag">The tag of the notification.</param>
        /// <param name="toastDuration">The amount of time that the toast will stay on screen.</param>
        /// <param name="bypassLengthLimit">Whether or not to bypass the length limit of the notification.</param>
        public async static void Notification(
            string title,
            string subtitle,
            string tag,
            int toastDuration = 1,
            bool bypassLengthLimit = false
        ) {

            string[] notifTooLong = [
                "This notification was too long.",
                "What you are looking for has probably been copied to your clipboard.",
            ];

            //* Check if title and subtitle are too long
            if (title.Length > 54 && !bypassLengthLimit) {
                title = notifTooLong[0]; subtitle = notifTooLong[1]; toastDuration = 5;
            } else if (subtitle.Length > 108 && !bypassLengthLimit) {
                title = notifTooLong[0]; subtitle = notifTooLong[1]; toastDuration = 5;
            }

            await Task.Run(
                () => {
                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                        .AddText(title)
                        .AddText(subtitle)
                        .Show(toast => { toast.Tag = tag; });

                    Task.Delay((toastDuration + 1) * 1000).Wait();
                    Microsoft.Toolkit.Uwp.Notifications.
                        ToastNotificationManagerCompat.History.Remove(tag);
                }
            );
        }

        /// <summary>
        /// Returns true if IndexTest failed, i.e there were no arguments other than the command.
        /// Returns false if the program ran successfully with all arguments.
        /// </summary>
        /// <param name="args">All arguments passed when pressing main Ctrl+F8 function.</param>
        /// <param name="argscount">The index that indextest will check to see if it exists.</param>
        /// <param name="ifOutOfRange">Method that is called incase indextest is false.</param>
        /// <returns>A bool that will be false if IndexTest failed, and true if it didn't.</returns>
        public static bool IndexTest(string[] args, int argscount = 1, bool sendNotif = true) {
            try {
                string test = args[argscount];
                return false;
            } catch (IndexOutOfRangeException) {
                NotifCheck(
                    sendNotif,
                    ["Exception", "Invalid parameters inputted, try 'help' for more info.", "3"],
                    "indexTestError"
                );

                return true;
            }
        }

        /// <summary>
        /// Checks if a string has only a certain set of characters.
        /// </summary>
        /// <param name="allowableChar">Set of characters that are allowed in the string.</param>
        /// <param name="text">The text that is being checked.</param>
        /// <returns>A bool that will be true if the text matches the format and false if it doesn't.</returns>
        public static bool FormatValid(string allowableChar, string text) {
            foreach (char c in text) {
                if (!allowableChar.Contains(c.ToString()))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Interchanges the position of key and value in a dictionary.
        /// </summary>
        /// <param name="dict">The dictionary that is inverted.</param>
        /// <returns>A dictionary with all keys and values inverted.</returns>
        public static Dictionary<string, string> InvertKeyAndValue(Dictionary<string, string> dict) {
            Dictionary<string, string> finalDict = new();
            foreach (var key in dict.Keys) { finalDict[dict[key]] = key; }

            return finalDict;
        }

        /// <summary>
        /// Checks if the function being called is willing to copy something to the clipboard.
        /// </summary>
        /// <param name="copy">Boolean that is usually true and checks if the function wants to copy something.</param>
        /// <param name="toCopy">The string that is to be copied to the clipboard if copy is true.</param>
        public static void CopyCheck(bool copy, string toCopy) {
            if (copy && !UtilitiesAppContext.CurrentSettings.DisableClipboardManipulation) {
                WindowsClipboard.SetText(toCopy);
            }

            if (
                UtilitiesAppContext.CurrentSettings.AutoPaste
                && !UtilitiesAppContext.CurrentSettings.DisableClipboardManipulation
            ) { Thread.Sleep(100); SendKeys.SendWait("^v"); }
        }

        /// <summary>
        /// Checks if the function being called is willing to send a notification.
        /// </summary>
        /// <param name="notif">Boolean that is usually true and checks if notification is gonna be sent.</param>
        /// <param name="notifContent">The content for the notification, if it notif is true.</param>
        public static void NotifCheck(
            bool notif,
            string[] notifContent,
            string tag,
            bool bypassLengthLimit = false
        ) {
            bool settingsDisallowed = UtilitiesAppContext.CurrentSettings.DisableNotifications;

            if (notif && !settingsDisallowed) {
                Notification(
                    notifContent[0],
                    notifContent[1],
                    tag,
                    int.Parse(notifContent[2]),
                    bypassLengthLimit: bypassLengthLimit
                );
            }
        }

        /// <summary>
        /// Overload of original NotifCheck method that allows you to send a custom ToastContentBuilder.
        /// </summary>
        /// <param name="customToast">The ToastContentBuilder that is to be shown to the user.</param>
        /// <param name="toastTag">The tag of the Toast Notification.</param>
        /// <param name="clearToast">Denotes whether the toast should be cleared.</param>
        /// <param name="toastDuration">The duration of the toast.</param>
        public async static void NotifCheck(
            Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder customToast,
            string toastTag,
            bool clearToast = true,
            int toastDuration = 1
        ) {
            bool settingsDisallowed = UtilitiesAppContext.CurrentSettings.DisableNotifications;

            if (customToast != null && !settingsDisallowed) {
                await Task.Run(() => customToast.Show());

                if (clearToast) {
                    Task.Delay((toastDuration + 1) * 1000).Wait();
                    Microsoft.Toolkit.Uwp.Notifications.
                        ToastNotificationManagerCompat.History.Remove(toastTag);
                }
            }
        }

        /// <summary>
        /// Returns all integers from a string using Regex.
        /// </summary>
        /// <param name="input">The string which is to be searched for integers.</param>
        /// <returns>A list of all BigIntegers that were captured by the Regex.</returns>
        public static List<System.Numerics.BigInteger> RegexFindAllInts(string input) {
            input = input.Replace(",", "");

            List<System.Numerics.BigInteger> BigInts = [];
            Regex re = FindAllIntsRegex();

            if (re.Matches(input).Count >= 1) {
                MatchCollection matches = re.Matches(input);
                foreach (Match? match in matches) {
                    if (match != null) {
                        GroupCollection groups = match.Groups;
                        BigInts.Add(System.Numerics.BigInteger.Parse(groups["num"].Value));
                    }
                }
            }

            return BigInts;
        }

        /// <summary>
        /// Formats every character of a string to a value in a dictionary.
        /// </summary>
        /// <param name="input">The input string to be formatted.</param>
        /// <param name="dict">The dictionary to use for the keys and values.</param>
        /// <returns>A formatted string.</returns>
        public static string TextFormatter(string input, Dictionary<string, string> dict) {
            List<string> converted = new();

            foreach (char i in input) {
                if (dict.ContainsKey(i.ToString())) {
                    converted.Add(dict[i.ToString()]);
                } else {
                    converted.Add(i.ToString());
                }
            }

            return string.Join("", converted);
        }

        /// <summary>
        /// Uses Regex to search through a string using an expression.
        /// Returns a List of DIctionaries where the Match is the key and a GroupCollection of that match is the value.
        /// </summary>
        /// <param name="input">The string that is to be searched.</param>
        /// <param name="expression">The regex expression used to search through the input.</param>
        /// <param name="useIsMatch">
        /// If true, the function will use re.IsMatch() instead of re.Matches().Count,
        /// this should only be used for expressions which are designed to have only one match.
        /// </param>
        /// <returns>A dictionary of all the matches which point to their groups.</returns>
        public static Dictionary<Match, GroupCollection>? RegexFind(
            string input,
            string expression,
            bool useIsMatch = false,
            Action? ifNotMatch = null
        ) {

            List<Dictionary<Match, GroupCollection>> matchesAndGroups = new();
            Regex re = new(expression);

            void matched() {
                foreach (Match? match in re.Matches(input)) {
                    if (match != null) {
                        Dictionary<Match, GroupCollection> matchToGroups =
                            new() { { match, match.Groups } };
                        matchesAndGroups.Add(matchToGroups);
                    }
                }
            }

            if (!useIsMatch) {
                if (re.Matches(input).Count >= 1) { matched(); } else { ifNotMatch?.Invoke(); return null; }
            } else if (useIsMatch) {
                if (re.IsMatch(input)) { matched(); } else { ifNotMatch?.Invoke(); return null; }
            }

            return matchesAndGroups[0];
        }

        /// <summary>
        /// Returns the input string with the first character converted to uppercase.
        /// </summary>
        public static string Capitalise(string s) {
            if (string.IsNullOrEmpty(s)) { return string.Empty; }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// A method that returns a rounded-version of a number if it is close enough to a whole number.
        /// 5.999999998 -> 6, 5.5 = 5.5.
        /// </summary>
        /// <param name="num">The number to be rounded off to, or not.</param>
        /// <returns>
        /// A double based on if the number was rounded or not.
        /// If the number was not rounded off, it returns the same number.s
        /// </returns>
        public static double RoundIfNumberIsNearEnough(double num) {
            Regex re = RoundIfNumberIsNearEnoughRegex();

            if (re.Matches(num.ToString()).Count == 1) {
                return Math.Round(num);
            } else {
                return num;
            }
        }

        /// <summary>
        /// Replaces all instances of each character in the string "chars" with the character at the same index in the string "replacementChars".
        /// </summary>
        /// <param name="text">The text to be modified.</param>
        /// <param name="chars">The characters to be replaced. Each character should be separated by a space.</param>
        /// <param name="replacementChars">The characters to replace with. Each character should be separated by a space.</param>
        /// <returns>The modified text.</returns>
        public static string BulkReplace(string text, string chars, string replacementChars) {
            List<string> charsList = [.. chars.Split(" ")];
            List<string> replacementCharsList = [.. replacementChars.Split(" ")];
            string result = text;
            
            foreach (string i in charsList) {
                result = result.Replace(i, replacementCharsList[charsList.IndexOf(i)]);
            }

            return result;
        }
    }
    /// <summary>
    /// Primary class for modifying and manipulating the windows clipboard.
    /// </summary>
    public class WindowsClipboard {
        /// <summary>
        /// The method that sets the windows clipboard to the specified text.
        /// </summary>
        /// <param name="text">The text that is to be set to the clipboard.</param>
        public static void SetText(string text) {
            OpenClipboard();

            EmptyClipboard();
            IntPtr hGlobal = default;
            try {
                var bytes = (text.Length + 1) * 2;
                hGlobal = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes);

                if (hGlobal == default) { ThrowWin32(); }
                var target = GlobalLock(hGlobal);

                if (target == default) { ThrowWin32(); }

                try {
                    System.Runtime.InteropServices.Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                } finally { GlobalUnlock(target); }

                if (SetClipboardData(cfUnicodeText, hGlobal) == default) { ThrowWin32(); }

                hGlobal = default;
            } finally {
                if (hGlobal != default) { System.Runtime.InteropServices.Marshal.FreeHGlobal(hGlobal); }

                CloseClipboard();
            }
        }

        public static void OpenClipboard() {
            var num = 10;

            while (true) { if (OpenClipboard(default)) { break; } if (--num == 0) { ThrowWin32(); } Thread.Sleep(100); }
        }

        const uint cfUnicodeText = 13;

        static void ThrowWin32() {
            throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool CloseClipboard();

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool EmptyClipboard();
    }
}
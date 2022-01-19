using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Uwp.Notifications;

namespace utilities_cs {
    public class Utils {

        private static Task? notificationTask;

        /// <summary>
        /// The primary notification method, used to send a Windows Toast Notification.
        /// </summary>
        /// <param name="title">The title of the notificaton.</param>
        /// <param name="subtitle">The subtitle of the notification.</param>
        /// <param name="toastexpirationtime">The amount of time that the toast will stay on screen.</param>
        public async static void Notification(string title, string subtitle, int toastexpirationtime = 1) {
            if (notificationTask != null) {
                await notificationTask;
            }

            string[] notifTooLong = new string[] {
                "This notification was too long.",
                "What you are looking for has probably been copied to your clipboard.",
            };

            // Check if title and subtitle are too long
            if (title.Length > 54) {
                title = notifTooLong[0];
                subtitle = notifTooLong[1];
                toastexpirationtime = 5;
            } else if (subtitle.Length > 108) {
                title = notifTooLong[0];
                subtitle = notifTooLong[1];
                toastexpirationtime = 5;
            }

            notificationTask = Task.Run(() => {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(subtitle).Show();
                Task.Delay((toastexpirationtime + 1) * 1000).Wait();
                ToastNotificationManagerCompat.History.Clear();
            });
        }

        /// <summary>
        /// Returns true if IndexTest failed, i.e there were no arguments other than the command.
        /// Returns false if the program ran successfully with all arguments.
        /// </summary>
        /// <param name="args">All arguments passed when pressing main Ctrl+F8 function.</param>
        /// <param name="title">The title of the notification incase the indextest was false.</param>
        /// <param name="subtitle">The subtitle of the notification incase the indextest was false.</param>
        /// <param name="duration">The amount of time the notification stays on screen incase the indextest was false.</param>
        /// <param name="argscount">The index that indextest will check to see if it exists.</param>
        /// <param name="ifOutOfRange">Method that is called incase indextest is false.</param>
        public static bool IndexTest(
                string[] args,
                int argscount = 1,
                Action? ifOutOfRange = null
            ) {

            try {
                string test = args[argscount];
                return false;
            } catch (IndexOutOfRangeException) {
                Notification("Huh.", "It seems you did not specify the parametes correctly.", 3);
                ifOutOfRange?.Invoke();
                return true;
            }
        }

        /// <summary>
        /// Checks if a string has only a certain set of characters.
        /// </summary>
        /// <param name="allowable_char">Set of characters that are allowed in the string.</param>
        /// <param name="text">The text that is being checked.</param>
        public static bool FormatValid(string allowable_char, string text) {
            foreach (char c in text) {
                if (!allowable_char.Contains(c.ToString()))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Interchanges the position of key and value in a dictionary.
        /// </summary>
        /// <param name="dict">The dictionary that is inverted.</param>
        public static Dictionary<string, string> invertKeyAndValue(Dictionary<string, string> dict) {
            Dictionary<string, string> final_dict = new();
            foreach (var key in dict.Keys) {
                final_dict[dict[key]] = key;
            }

            return final_dict;
        }

        /// <summary>
        /// Checks if the function being called is willing to copy something to the clipboard.
        /// Primarily used for Format.
        /// </summary>
        /// <param name="copy">Boolean that is usually true and checks if the function wants to copy something.</param>
        /// <param name="toCopy">The string that is to be copied to the clipboard if copy is true.</param>
        public static void CopyCheck(bool copy, string toCopy) {
            if (copy) {
                WindowsClipboard.SetText(toCopy);
            }
        }

        /// <summary>
        /// Checks if the function being called is willing to send a notification.
        /// Primarily used for Format.
        /// </summary>
        /// <param name="notif">Boolean that is usually true and checks if notification is gonna be sent.</param>
        /// <param name="notifContent">The content for the notification, if it notif is true.</param>
        public static void NotifCheck(bool notif, string[] notifContent) {

# nullable disable

            if (notif) {
                Notification(
                    notifContent[0],
                    notifContent[1],
                    int.Parse(notifContent[2])
                );
            }
        }
        /// <summary>
        /// Returns all integers from a string using Regex.
        /// </summary>
        /// <param name="input">The string which is to be searched for integers.</param>
        public static List<int> RegexFindAllInts(string input) {
            List<int> ints = new();
            List<Dictionary<Match, GroupCollection>> matchesAndGroups = RegexFind(input, @"(\d+)+");

            foreach (Dictionary<Match, GroupCollection> dictMatchesAndGroups in matchesAndGroups) {
                foreach (KeyValuePair<Match, GroupCollection> keyValuePair in dictMatchesAndGroups) {
                    foreach (Group group in keyValuePair.Value) {
                        ints.Add(int.Parse(group.ToString()));
                    }
                }
            }

            return ints;
        }

# nullable enable

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
        public static List<Dictionary<Match, GroupCollection>>? RegexFind(
                string input,
                string expression,
                bool useIsMatch = false,
                Action? ifNotMatch = null
            ) {

            List<Dictionary<Match, GroupCollection>> matchesAndGroups = new();

            Regex re = new Regex(expression);

# nullable disable

            Action matched = () => {
                foreach (Match match in re.Matches(input)) {
                    Dictionary<Match, GroupCollection> matchToGroups = new() { { match, match.Groups } };
                    matchesAndGroups.Add(matchToGroups);
                }
            };

            if (!useIsMatch) {
                if (re.Matches(input).Count >= 1) {
                    matched.Invoke();
                } else {
                    ifNotMatch?.Invoke();
                    return null;
                }
            } else if (useIsMatch) {
                if (re.IsMatch(input)) {
                    matched.Invoke();
                } else {
                    ifNotMatch?.Invoke();
                    return null;
                }
            }

            return matchesAndGroups;
        }
    }
    public class WindowsClipboard {
        public static void SetText(string text) {
            OpenClipboard();

            EmptyClipboard();
            IntPtr hGlobal = default;
            try {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);

                if (hGlobal == default) {
                    ThrowWin32();
                }

                var target = GlobalLock(hGlobal);

                if (target == default) {
                    ThrowWin32();
                }

                try {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                } finally {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(cfUnicodeText, hGlobal) == default) {
                    ThrowWin32();
                }

                hGlobal = default;
            } finally {
                if (hGlobal != default) {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }
        }

        public static void OpenClipboard() {
            var num = 10;
            while (true) {
                if (OpenClipboard(default)) {
                    break;
                }
                if (--num == 0) {
                    ThrowWin32();
                }

                Thread.Sleep(100);
            }
        }

        const uint cfUnicodeText = 13;

        static void ThrowWin32() {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
    }
}

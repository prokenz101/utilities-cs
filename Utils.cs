using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;

namespace utilities_cs {
    public class Utils {

        private static Task? notificationTask;
        public async static void Notification(string title, string subtitle, int toastexpirationtime = 1) {
            if (notificationTask != null) {
                await notificationTask;
            }
            notificationTask = Task.Run(() => {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(subtitle).Show();
                Task.Delay((toastexpirationtime + 1) * 1000).Wait();
                ToastNotificationManagerCompat.History.Clear();
            });
        }

        public static bool IndexTest(
                string[] args,
                string title,
                string subtitle,
                int duration = 1,
                int argscount = 1,
                Action? ifOutOfRange = null
            ) {
            // Returns true if IndexTest failed, i.e there were no arguments other than the command.
            // Returns false if the program ran successfully with all arguments.
            try {
                string test = args[argscount];
                return false;
            } catch (IndexOutOfRangeException) {
                Notification(title, subtitle, duration);
                ifOutOfRange?.Invoke();
                return true;
            }
        }

        public static bool FormatValid(string allowable_char, string format) {
            foreach (char c in format) {
                if (!allowable_char.Contains(c.ToString()))
                    return false;
            }
            return true;
        }

        public static Dictionary<string, string> invertKeyAndValue(Dictionary<string, string> dict) {
            Dictionary<string, string> final_dict = new();
            foreach (var key in dict.Keys) {
                final_dict[dict[key]] = key;
            }

            return final_dict;
        }
    }
}
static class WindowsClipboard {
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

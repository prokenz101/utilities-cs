using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

# nullable disable

namespace utilities_cs {
    public struct AutoclickData {
        public int interval;
        public MouseButton mousebutton;
        public int count;

        public AutoclickData(GroupCollection groups) {
            if (!int.TryParse(groups["interval"].Value, out interval)) interval = 1;
            interval = Math.Clamp(interval, 1, int.MaxValue);


            if (!int.TryParse(groups["count"].Value, out count)) count = int.MaxValue;
            var button = StringToMouseButton(groups["mb"].Value);
            if (button.HasValue) {
                mousebutton = (MouseButton)button;
            } else {
                throw new ArgumentException("bruh really?"); // never going to run
            }
        }


        public static MouseButton? StringToMouseButton(string stringvalue) {

            switch (stringvalue) {
                case "left":
                    return MouseButton.Left;

                case "right":
                    return MouseButton.Right;

                case "middle":
                    return MouseButton.Middle;

                default:
                    return null;
            }
        }
    }

    public class Autoclick {
        static MouseOperations.MousePoint topLeft = new() {
            X = 0,
            Y = 0
        };
        static string reg_exp = @"(?<interval>\d+ )?(?<mb>left|right|middle)(?<count> \d+)?";
        static Regex re = new Regex(reg_exp, RegexOptions.Compiled);
        private static Task autoclickTask;
        private static CancellationTokenSource cancelTkn;

        public static void autoclick(string[] args) {
            string text = string.Join(" ", args[1..]); // parameters of autoclick, like {interval} {mousebutton} etc
            MatchCollection matches = re.Matches(text);
            if (matches.Count > 0) {
                var match = matches[0];
                GroupCollection groups = match.Groups;
                AutoclickData data = new(groups);
                PerformAutoclick(data);
            } else {
                Utils.Notification(
                    "Huh.",
                    "The parameters for autoclick were not given properly. Try again.",
                    3
                );
            }
        }

        public static void PerformAutoclick(AutoclickData data) {
            cancelTkn = new CancellationTokenSource();
            HookManager.AddHook(
                "autoclick_stop",
                ModifierKeys.Control,
                Keys.F7,
                stopAutoclick,
                autoclickFailedToRegisterHotkey
            );

            Action click = () => {
                MouseOperations.MouseClick(data.mousebutton);
                Task.Delay(data.interval).Wait();
            };

            // creating task for autoclicker
            autoclickTask = Task.Factory.StartNew(
                () => {
                    var token = cancelTkn.Token;
                    Func<bool> shouldStop = () => token.IsCancellationRequested
                        || MouseOperations.GetCursorPosition().toPoint() == topLeft.toPoint();

                    Utils.Notification("Starting autoclicker...", "Press Ctrl + F7 to stop.");
                    Task.Delay(1500).Wait();
                    if (data.count == int.MaxValue) {
                        while (true) {
                            if (shouldStop()) break;
                            click();
                        }
                    } else {
                        for (int i = 0; i < data.count; i++) {
                            if (shouldStop()) break;
                            click();
                        }
                    }
                    stopAutoclick();

                }
            );

            void stopAutoclick() {
                cancelTkn.Cancel();
                HookManager.UnregisterHook("autoclick_stop");
                Utils.Notification("Stopped Autoclicker.", "The autoclicker was stopped.", 3);
            }
            void autoclickFailedToRegisterHotkey() {
                Utils.Notification(
                    "Something went wrong.",
                    @"This might be because you might have attempted to start a second autoclicker.",
                    5
                );
                stopAutoclick();
            }
        }
    }
    public class MouseOperations {
        [Flags]
        public enum MouseEventFlags {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y) {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point) {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition() {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }


        public static void MouseEvent(MouseEventFlags value) {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        public static void MouseClick(MouseButton button) {
            var flags = mouseButtonToFlags(button);
            MouseEvent(flags);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint {
            public int X;
            public int Y;

            public MousePoint(int x, int y) {
                X = x;
                Y = y;
            }

            public Point toPoint() => new Point(X, Y);
        }

        public static MouseEventFlags mouseButtonToFlags(MouseButton button) {
            switch (button) {
                case MouseButton.Left:
                    return MouseEventFlags.LeftDown | MouseEventFlags.LeftUp;

                case MouseButton.Right:
                    return MouseEventFlags.RightDown | MouseEventFlags.RightUp;

                case MouseButton.Middle:
                    return MouseEventFlags.MiddleDown | MouseEventFlags.MiddleUp;
            }
            return MouseEventFlags.Move;
        }

    }
    public enum MouseButton {
        Left,
        Middle,
        Right,
    }
}
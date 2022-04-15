using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace utilities_cs {
    public class Autoclick {
        public async static void Autoclicker(string[] args) {
            Thread.Sleep(100);
            Regex re = new Regex(@"(?<interval>\d+)? ?(?<mb>left|right|middle)(?<count> \d+)?");
            string text = string.Join(" ", args[1..]).ToLower();

            if (re.IsMatch(text)) {
                MatchCollection matches = re.Matches(text);

                try {
                    List<object> data = new();

                    try {
                        int interval = int.Parse(matches[0].Groups["interval"].Value.ToString());
                        data.Add(interval);
                    } catch (FormatException) {
                        data.Add("null");
                    }

                    data.Add(stringToMouseEventFlags(matches[0].Groups["mb"].Value)!);

                    try {
                        int count = int.Parse(matches[0].Groups["count"].Value.ToString());
                        data.Add(count);
                    } catch (FormatException) {
                        data.Add("null");
                    }

                    //* DICTIONARY OF DATA LIST
                    //* data[0] => interval (int)
                    //* data[1] => mouse button (MouseEventFlags)
                    //* data[2] => count (int)

                    if (data[0].ToString() == "null") { data[0] = 0; } //* default interval
                    if (data[2].ToString() == "null") { data[2] = int.MaxValue; } //* default count

                    //* tokens
                    var autoclickTokenSource = new CancellationTokenSource();
                    var autoclickToken = autoclickTokenSource.Token;

                    Action stopAutoclicker = () => {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Stopped the autoclicker.",
                                "The autoclicker has been stopped successfully.",
                                "4"
                            }
                        );

                        HookManager.UnregisterHook("autoclickStop");
                        autoclickTokenSource.Cancel();
                        autoclickTokenSource.Dispose();
                    };

                    Task autoclickTask = new Task(
                        () => {
                            Action performAutoclick = () => {
                                MouseOperations.MouseEvent((MouseOperations.MouseEventFlags)data[1]!);
                                Thread.Sleep((int)data[0]);

                                autoclickToken.ThrowIfCancellationRequested();
                            };

                            try {
                                if ((int)data[2] == int.MaxValue) {
                                    while (true) { performAutoclick(); }
                                } else {
                                    for (int i = 0; i < (int)data[2]; i++) { performAutoclick(); }
                                    stopAutoclicker();
                                }

                            } catch (OperationCanceledException) { return; }
                        },
                        autoclickToken
                    );

                    //* Stop hotkey (Ctrl + F7)
                    HookManager.AddHook(
                        "autoclickStop",
                        new ModifierKeys[] { ModifierKeys.Control },
                        Keys.F7,
                        stopAutoclicker,
                        () => {
                            Utils.NotifCheck(
                                true,
                                new string[] { "Huh.", "Perhaps you already have an autoclicker running", "4" }
                            );
                        }
                    );

                    await Task.Run(() => autoclickTask.Start());

                } catch (OverflowException) {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Huh.",
                            "Perhaps the count was too large.",
                            "3"
                        }
                    );
                }
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "Perhaps the parameters were not inputted correctly.",
                        "4"
                    }
                );
                return;
            }
        }

        public static MouseOperations.MouseEventFlags? stringToMouseEventFlags(string mouseButton) {
            switch (mouseButton) {
                case "left":
                    return MouseOperations.MouseEventFlags.LeftDown | MouseOperations.MouseEventFlags.LeftUp;
                case "right":
                    return MouseOperations.MouseEventFlags.RightDown | MouseOperations.MouseEventFlags.RightUp;
                case "middle":
                    return MouseOperations.MouseEventFlags.MiddleDown | MouseOperations.MouseEventFlags.MiddleUp;
                default:
                    return null;
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

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint {
            public int X;
            public int Y;

            public MousePoint(int x, int y) {
                X = x;
                Y = y;
            }
        }
    }
}
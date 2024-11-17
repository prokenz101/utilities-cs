using CommandLine;
using System.Runtime.InteropServices;

namespace utilities_cs {
    public class Autoclick {
        public static void Autoclicker(string[] args) {
            Thread.Sleep(100);

            Parser.Default.ParseArguments<AutoclickOptions>(args[1..])
                .WithParsed(AutoclickOptions.HandleSuccessfulParse)
                .WithNotParsed(errs => Utils.NotifCheck(
                    true,
                    [
                        "Exception",
                        "Invalid parameters, try 'help' for more info.",
                        "4"
                    ], "autoclickError"
                )
            );
        }

        public static MouseOperations.MouseEventFlags? StringToMouseEventFlags(string mouseButton) {
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

        public static void PerformAutoclick(int interval, MouseOperations.MouseEventFlags mouseButton, int count) {
            //* tokens
            var autoclickTokenSource = new CancellationTokenSource();
            var autoclickToken = autoclickTokenSource.Token;

            void stopAutoclicker() {
                Utils.NotifCheck(
                    true,
                    [
                        "Stopped the autoclicker.",
                        "The autoclicker has been stopped successfully.",
                        "4"
                    ],
                    "autoclickStop"
                );

                HookManager.UnregisterHook("autoclickStop");
                autoclickTokenSource.Cancel();
                autoclickTokenSource.Dispose();
            }

            Task autoclickTask = new(
                () => {
                    Action performAutoclick = () => {
                        MouseOperations.MouseEvent(mouseButton);
                        Thread.Sleep(interval);

                        autoclickToken.ThrowIfCancellationRequested();
                    };

                    try {
                        if (count == int.MaxValue) {
                            while (true) { performAutoclick(); }
                        } else {
                            for (int i = 0; i < count; i++) { performAutoclick(); }
                            stopAutoclicker();
                        }

                    } catch (OperationCanceledException) { return; }
                }, autoclickToken
            );

            //* Stop hotkey (Ctrl + F7)
            HookManager.AddHook(
                "autoclickStop",
                [ModifierKeys.Control],
                Keys.F7,
                stopAutoclicker,
                () => {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Another autoclicker is already running.", "4"],
                        "autoclickHotkeyError"
                    );
                }
            );

            autoclickTask.Start();
        }
    }

    public class AutoclickOptions {
        [Value(0, Required = true)]
        public string? MouseButton { get; set; }

        [Option('i', "interval", Default = 0)]
        public int Interval { get; set; }

        [Option('c', "count", Default = int.MaxValue)]
        public int Count { get; set; }

        public static void HandleSuccessfulParse(AutoclickOptions opts) {
            if (opts.MouseButton == "left" | opts.MouseButton == "right" | opts.MouseButton == "middle") {
                Autoclick.PerformAutoclick(
                    opts.Interval,
                    (MouseOperations.MouseEventFlags)Autoclick.StringToMouseEventFlags(opts.MouseButton!)!,
                    opts.Count
                );
            } else {
                Utils.NotifCheck(
                    true,
                    [
                        "Exception",
                        "Invalid mouse button, try 'help' for more info.",
                        "4"
                    ], "autoclickError"
                );
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

        public static void SetCursorPosition(int x, int y) { SetCursorPos(x, y); }

        public static void SetCursorPosition(MousePoint point) { SetCursorPos(point.X, point.Y); }

        public static MousePoint GetCursorPosition() {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value) {
            MousePoint position = GetCursorPosition();
            mouse_event((int)value, position.X, position.Y, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint {
            public int X;
            public int Y;

            public MousePoint(int x, int y) { X = x; Y = y; }
        }
    }
}
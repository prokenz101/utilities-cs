using CommandLine;
using System.Runtime.InteropServices;

namespace utilities_cs {
    public class Autoclick {
        public static void Autoclicker(string[] args) {
            Thread.Sleep(100);

            CommandLine.Parser.Default.ParseArguments<AutoclickOptions>(args[1..])
                .WithParsed(opts => AutoclickOptions.HandleSuccessfulParse(opts))
                .WithNotParsed(errs => Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "Perhaps the parameters were not inputted correctly.",
                        "4"
                    }, "autoclickError"
                )
            );
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

        public static void PerformAutoclick(int interval, MouseOperations.MouseEventFlags mouseButton, int count) {
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
                    },
                    "autoclickStop"
                );

                HookManager.UnregisterHook("autoclickStop");
                autoclickTokenSource.Cancel();
                autoclickTokenSource.Dispose();
            };

            Task autoclickTask = new Task(
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
                new ModifierKeys[] { ModifierKeys.Control },
                Keys.F7,
                stopAutoclicker,
                () => {
                    Utils.NotifCheck(
                        true,
                        new string[] { "Huh.", "Perhaps you already have an autoclicker running", "4" },
                        "autoclickHotkeyError"
                    );
                }
            );

            autoclickTask.Start();
        }
    }

    public class AutoclickOptions {
        [Value(0, Required = true)]
        public string? mouseButton { get; set; }

        [Option('i', "interval", Default = 0)]
        public int interval { get; set; }

        [Option('c', "count", Default = int.MaxValue)]
        public int count { get; set; }

        public static void HandleSuccessfulParse(AutoclickOptions opts) {
            if (opts.mouseButton == "left" | opts.mouseButton == "right" | opts.mouseButton == "middle") {
                Autoclick.PerformAutoclick(
                    opts.interval,
                    (MouseOperations.MouseEventFlags)Autoclick.stringToMouseEventFlags(opts.mouseButton!)!,
                    opts.count
                );
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Hey!",
                        "The mousebutton can only be \"left\", \"right\" or \"middle\".",
                        "4"
                    }, "autoclickError"
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
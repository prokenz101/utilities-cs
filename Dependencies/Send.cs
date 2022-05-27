namespace utilities_cs {
    public class Send {
        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public static Dictionary<string, int> keys = new() {
            { "next", VK_MEDIA_NEXT_TRACK },
            { "skip", VK_MEDIA_NEXT_TRACK },
            { "prev", VK_MEDIA_PREV_TRACK },
            { "previous", VK_MEDIA_PREV_TRACK },
            { "play", VK_MEDIA_PLAY_PAUSE },
            { "pause", VK_MEDIA_PLAY_PAUSE },
        };


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        public static void SendKey(int key) {
            try {
                keybd_event((byte)key, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
            } catch (FormatException) {
                Utils.NotifCheck(
                    true, new string[] { "Something went wrong.", "Type-casting to byte failed.", "3" }, "sendError"
                );
            }
        }

        public static void SendMain(string[] args) {
            if (Utils.IndexTest(args)) { return; }

            if (keys.ContainsKey(args[1])) {
                SendKey(keys[args[1]]);
            } else {
                Thread.Sleep(500);
                string text = string.Join(" ", args[1..]);

                foreach (char i in text) {
                    try {
                        SendKeys.SendWait(i.ToString());
                    } catch {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Something went wrong.",
                                "Failed to send keys. Try removing any special characters like ().",
                                "3"
                            }, "sendError"
                        ); break;
                    }
                }
            }
        }
    }
}
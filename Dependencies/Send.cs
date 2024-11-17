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
                    true, ["Exception", "Make sure there aren't any special characters in your text.", "3"], "sendError"
                );
            }
        }

        public static void SendMain(string[] args) {
            if (Utils.IndexTest(args)) { return; }

            if (keys.TryGetValue(args[1], out int key)) {
                SendKey(key);
            } else {
                Thread.Sleep(500);
                string text = string.Join(" ", args[1..]);

                foreach (char i in text) {
                    try {
                        SendKeys.SendWait(i.ToString());
                    } catch {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception",
                                "Failed to send keys, try 'help' for more info.",
                                "3"
                            ], "sendError"
                        ); break;
                    }
                }
            }
        }
    }
}
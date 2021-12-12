using System;
using System.Text;
using System.Linq;

namespace utilities_cs {
    public class Binary {
        public static void Bin(string[] args) {
            string text = string.Join(' ', args[1..]);
            if (Utils.IndexTest(args, "Huh.", "It seems you did not input anything for binary to convert.", 4)) {
                return;
            }
            bool FormatValid(string format) {
                string allowableLetters = "01 ";
                foreach (char c in format) {
                    if (!allowableLetters.Contains(c.ToString()))
                        return false;
                }
                return true;
            }
            if (!FormatValid(text)) {
                byte[] ConvertToByteArray(string str, Encoding encoding) {
                    return encoding.GetBytes(str);
                }

                string ToBinary(Byte[] data) {
                    return string.Join(
                        " ",
                        data.Select(
                            byt => Convert.ToString(byt, 2).PadLeft(8, '0')
                        )
                    );
                }

                string ans = ToBinary(ConvertToByteArray(text, Encoding.ASCII));
                Utils.Notification("Success!", "Message copied to clipboard.", 3);
                WindowsClipboard.SetText(ans);

            } else {
                try {
                    string[] text_list = text.Split(" ");

                    var chars = from split in text_list
                                select ((char)Convert.ToInt32(split, 2)).ToString();
                    Utils.Notification("Success!", $"The message was: {string.Join("", chars)}", 10);
                    WindowsClipboard.SetText(string.Join("", chars));
                } catch {
                    Utils.Notification("Huh.", @"There must be something wrong with the binary that you have inputted.
Please double check that you can actually convert this binary to ASCII characters.", 3);
                }
            }
        }
    }
}
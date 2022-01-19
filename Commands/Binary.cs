using System;
using System.Text;
using System.Linq;

namespace utilities_cs {
    public class Binary {
        public static string? Bin(string[] args, bool copy, bool notif) {
            string text = string.Join(' ', args[1..]);
            if (Utils.IndexTest(args)) {
                return null;
            }
            if (!Utils.FormatValid("01 ", text)) {
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
                Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
                Utils.CopyCheck(copy, ans);
                return ans;

            } else {
                try {
                    string[] text_list = text.Split(" ");

                    var chars = from split in text_list
                                select ((char)Convert.ToInt32(split, 2)).ToString();
                    Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {string.Join("", chars)}", "10" });
                    Utils.CopyCheck(copy, string.Join("", chars));
                    return string.Join("", chars);
                } catch {
                    Utils.Notification("Huh.", @"There must be something wrong with the binary that you have inputted.
Please double check that you can actually convert this binary to ASCII characters.", 3);
                    return null;
                }
            }
        }
    }
}
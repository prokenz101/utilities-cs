using System;
using System.Text;
using System.Collections.Generic;

namespace utilities_cs {
    public class Hex {
        public static string? Hexadecimal(string[] args, bool copy, bool notif) {
            string text = string.Join(' ', args[1..]);

            string[] text_list = text.Split(" ");
            string hex_with_dash = string.Join("-", text_list);

            if (IsHex(string.Join("", args[1..]))) {
                string text_from_hex = Encoding.ASCII.GetString(toText(hex_with_dash));
                Utils.CopyCheck(copy, text_from_hex);
                Utils.NotifCheck(notif, new string[] { "Success!", $"The message was: {text_from_hex}", "10" });
                return text_from_hex;
            } else {
                string hex_from_text = toHex(text);
                Utils.CopyCheck(copy, hex_from_text);
                Utils.NotifCheck(notif, new string[] { "Success!", $"Message copied to clipboard.", "3" });
                return hex_from_text;
            }
        }

        static bool IsHex(IEnumerable<char> chars) {
            bool isHex;
            foreach (var c in chars) {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }
        static byte[] toText(string hex) {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++) {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        static string toHex(string text) {

            byte[] ba = Encoding.Default.GetBytes(text);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", " ");
            hexString = hexString.ToLower();

            return hexString;
        }
    }
}
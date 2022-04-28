namespace utilities_cs {
    public class Hex {
        public static string? HexadecimalMain(string[] args, bool copy, bool notif) {
            string text = string.Join(" ", args[1..]);
            if (Utils.IndexTest(args)) {
                return null;
            }

            string[] textList = text.Split(" ");
            string hexWithDash = string.Join("-", textList);

            if (Hex.IsHex(string.Join("", args[1..]))) {
                try {
                    string textFromHex = System.Text.Encoding.ASCII.GetString(Hex.toText(hexWithDash));
                    Utils.CopyCheck(copy, textFromHex);
                    Utils.NotifCheck(
                        notif, new string[] { "Success!", $"The message was: {textFromHex}", "10" }, "hexSuccess"
                    ); return textFromHex;
                } catch {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.",
                            "An exception occured when trying to convert your text from hexadecimal.",
                            "4"
                        },
                        "hexadecimalError"
                    ); return null;
                }
            } else {
                try {
                    string hexFromText = Hex.toHex(text);
                    Utils.CopyCheck(copy, hexFromText);
                    Utils.NotifCheck(
                        notif, new string[] { "Success!", $"Message copied to clipboard.", "3" }, "hexSuccess"
                    ); return hexFromText;
                } catch {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.",
                            "An exception occured when trying to convert your text into hexadecimal.",
                            "4"
                        },
                        "hexadecimalError"
                    );
                    return null;
                }
            }
        }

        public static bool IsHex(IEnumerable<char> chars) {
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
        public static byte[] toText(string hex) {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++) {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        public static string toHex(string text) {

            byte[] ba = System.Text.Encoding.Default.GetBytes(text);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", " ");
            hexString = hexString.ToLower();

            return hexString;
        }
    }
}
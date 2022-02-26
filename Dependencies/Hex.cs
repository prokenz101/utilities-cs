namespace utilities_cs {
    public class Hex {
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
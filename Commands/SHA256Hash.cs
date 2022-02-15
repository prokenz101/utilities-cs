using System;
using System.Text;
using System.Security.Cryptography;

namespace utilities_cs {
    public class SHA256Hasher {
        public static string? SHA256Hash(string[] args, bool copy, bool notif) {
            string text = string.Join(" ", args[1..]);
            if (Utils.IndexTest(args)) {
                return null;
            }

            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create()) {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(text));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            Utils.CopyCheck(copy, Sb.ToString());
            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            return Sb.ToString();
        }
    }
}
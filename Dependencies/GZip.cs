namespace utilities_cs {
    public class GZip {
        public static string? GZipMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string mode = args[1];
            string text = string.Join(" ", args[2..]);
            if (mode == "to") {
                try {
                    string compressed = Compress(text);

                    Utils.CopyCheck(copy, compressed);
                    Utils.NotifCheck(
                        notif, ["Success!", "Message copied to clipboard.", "2"], "gzipSuccess"
                    );
                    return compressed;
                } catch {
                    Utils.NotifCheck(
                        true,
                        [
                            "Exception",
                            "Something went wrong while trying to convert your text to GZip.",
                            "4"
                        ], "gzipError"
                    );
                    return null;
                }
            } else if (mode == "from") {
                try {
                    string decompressed = Decompress(text);

                    Utils.CopyCheck(copy, decompressed);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The text was: {decompressed}", "2"], "gzipSuccess"
                    );
                    return decompressed;
                } catch {
                    Utils.NotifCheck(
                        true,
                        [
                            "Exception",
                            "Something went wrong while trying to decompress your text from GZip to ASCII.",
                            "4"
                        ],
                        "gzipError"
                    );
                    return null;
                }
            } else {
                Utils.NotifCheck(
                    true,
                    ["Exception", "Invalid mode, try 'help' for more info.", "4"],
                    "gzipError"
                ); return null;
            }
        }

        public static string Decompress(string input) {
            byte[] compressed = Convert.FromBase64String(input);
            byte[] decompressed = Decompress(compressed);
            return System.Text.Encoding.UTF8.GetString(decompressed);
        }

        public static string Compress(string input) {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] compressed = Compress(encoded);
            return Convert.ToBase64String(compressed);
        }

        public static byte[] Decompress(byte[] input) {
            using var source = new MemoryStream(input);
            byte[] lengthBytes = new byte[4];
            source.Read(lengthBytes, 0, 4);

            var length = BitConverter.ToInt32(lengthBytes, 0);
            using var decompressionStream = new System.IO.Compression.GZipStream(source,
                System.IO.Compression.CompressionMode.Decompress);
            var result = new byte[length];
            decompressionStream.ReadExactly(result);
            return result;
        }

        public static byte[] Compress(byte[] input) {
            using var result = new MemoryStream();
            var lengthBytes = BitConverter.GetBytes(input.Length);
            result.Write(lengthBytes, 0, 4);

            using (var compressionStream = new System.IO.Compression.GZipStream(result,
                System.IO.Compression.CompressionMode.Compress))
            {
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Flush();
            }

            return result.ToArray();
        }
    }
}
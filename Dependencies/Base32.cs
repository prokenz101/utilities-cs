namespace utilities_cs {
    /// <summary>
    /// Class used for conversion between byte array and Base32 notation
    /// </summary>
    internal sealed class Base32 {
        /// <summary>
        /// Size of the regular byte in bits
        /// </summary>
        private const int InByteSize = 8;

        /// <summary>
        /// Size of converted byte in bits
        /// </summary>
        private const int OutByteSize = 5;

        /// <summary>
        /// Alphabet
        /// </summary>
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// Convert byte array to Base32 format
        /// </summary>
        /// <param name="bytes">An array of bytes to convert to Base32 format</param>
        /// <returns>Returns a string representing byte array</returns>


        internal static string? ToBase32String(byte[] bytes) {
            if (bytes == null) {
                return null;
            } else if (bytes.Length == 0) {
                return string.Empty;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder(bytes.Length * InByteSize / OutByteSize);

            int bytesPosition = 0;

            int bytesSubPosition = 0;

            byte outputBase32Byte = 0;

            int outputBase32BytePosition = 0;

            while (bytesPosition < bytes.Length) {
                int bitsAvailableInByte = Math.Min(InByteSize - bytesSubPosition, OutByteSize - outputBase32BytePosition);

                outputBase32Byte <<= bitsAvailableInByte;

                outputBase32Byte |= (byte)(bytes[bytesPosition] >> (InByteSize - (bytesSubPosition + bitsAvailableInByte)));

                bytesSubPosition += bitsAvailableInByte;

                if (bytesSubPosition >= InByteSize) {
                    bytesPosition++;
                    bytesSubPosition = 0;
                }

                outputBase32BytePosition += bitsAvailableInByte;

                if (outputBase32BytePosition >= OutByteSize) {
                    outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                    builder.Append(Base32Alphabet[outputBase32Byte]);

                    outputBase32BytePosition = 0;
                }
            }

            if (outputBase32BytePosition > 0) {
                outputBase32Byte <<= (OutByteSize - outputBase32BytePosition);

                outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                builder.Append(Base32Alphabet[outputBase32Byte]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert base32 string to array of bytes.
        /// </summary>
        /// <param name="base32String">Base32 string to convert</param>
        /// <returns>Returns a byte array converted from the string</returns>



        internal static byte[]? FromBase32String(string base32String) {
            if (base32String == null) {
                return null;
            } else if (base32String == string.Empty) {
                return new byte[0];
            }

            string base32StringUpperCase = base32String.ToUpperInvariant();

            byte[] outputBytes = new byte[base32StringUpperCase.Length * OutByteSize / InByteSize];

            if (outputBytes.Length == 0) {
                throw new ArgumentException("Specified string is not valid Base32 format because it doesn''t have enough data to construct a complete byte array");
            }

            int base32Position = 0;

            int base32SubPosition = 0;

            int outputBytePosition = 0;

            int outputByteSubPosition = 0;

            while (outputBytePosition < outputBytes.Length) {
                int currentBase32Byte = Base32Alphabet.IndexOf(base32StringUpperCase[base32Position]);

                if (currentBase32Byte < 0) {
                    throw new ArgumentException(string.Format("Specified string is not valid Base32 format because character \"{0}\" does not exist in Base32 alphabet", base32String[base32Position]));
                }

                int bitsAvailableInByte = Math.Min(OutByteSize - base32SubPosition, InByteSize - outputByteSubPosition);

                outputBytes[outputBytePosition] <<= bitsAvailableInByte;

                outputBytes[outputBytePosition] |= (byte)(currentBase32Byte >> (OutByteSize - (base32SubPosition + bitsAvailableInByte)));

                outputByteSubPosition += bitsAvailableInByte;

                if (outputByteSubPosition >= InByteSize) {
                    outputBytePosition++;
                    outputByteSubPosition = 0;
                }

                base32SubPosition += bitsAvailableInByte;

                if (base32SubPosition >= OutByteSize) {
                    base32Position++;
                    base32SubPosition = 0;
                }
            }

            return outputBytes;
        }
    }
}
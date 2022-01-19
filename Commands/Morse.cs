using System.Collections.Generic;

namespace utilities_cs {
    public class Morse {

        static Dictionary<string, string> textToMorse = new() {
            { "a", ".-" },
            { "b", "-..." },
            { "c", "-.-." },
            { "d", "-.." },
            { "e", "." },
            { "f", "..-." },
            { "g", "--." },
            { "h", "...." },
            { "i", ".." },
            { "j", ".---" },
            { "k", "-.-" },
            { "l", ".-.." },
            { "m", "--" },
            { "n", "-." },
            { "o", "---" },
            { "p", ".--." },
            { "q", "--.-" },
            { "r", ".-." },
            { "s", "..." },
            { "t", "-" },
            { "u", "..-" },
            { "v", "...-" },
            { "w", ".--" },
            { "x", "-..-" },
            { "y", "-.--" },
            { "z", "--.." },
            { "0", "-----" },
            { "1", ".----" },
            { "2", "..---" },
            { "3", "...--" },
            { "4", "....-" },
            { "5", "....." },
            { "6", "-...." },
            { "7", "--..." },
            { "8", "---.." },
            { "9", "----." },
            { ".", ".-.-.-" },
            { ",", "--..--" },
            { "?", "..--.." },
            { "'", ".----." },
            { "!", "-.-.--" },
            { "/", "-..-." },
            { "(", "-.--." },
            { ")", "-.--.-" },
            { "&", ".-..." },
            { ":", "---..." },
            { ";", "-.-.-." },
            { "=", "-...-" },
            { "+", ".-.-." },
            { "-", "-....-" },
            { "_", "..--.-" },
            { "\"", ".-..-." },
            { "$", "...-..-" },
            { "@", ".--.-." },
            { "¿", "..-.-" },
            { "¡", "--...-" },
            { " ", "/" }
        };
        static Dictionary<string, string> morseToText = Utils.invertKeyAndValue(textToMorse);
        public static string? MorseCodeTranslate(string[] args, bool copy, bool notif) {
            string text = string.Join(' ', args[1..]).ToLower();
            if (Utils.IndexTest(args)) {
                return null;
            }

            if (Utils.FormatValid("-./ ", text)) {
                return toText(text, copy, notif);
            } else {
                return toMorse(text, copy, notif);
            }
        }

        public static string toMorse(string text, bool copy, bool notif) {
            List<string> morse_converted = new();
            foreach (char t in text) {
                if (textToMorse.ContainsKey(t.ToString())) {
                    morse_converted.Add(textToMorse[t.ToString()]);
                    morse_converted.Add(" ");
                } else {
                    morse_converted.Add(t.ToString());
                }
            }

            Utils.NotifCheck(notif, new string[] { "Success!", "Message copied to clipboard.", "3" });
            Utils.CopyCheck(copy, string.Join("", morse_converted));
            return string.Join("", morse_converted);
        }

        public static string toText(string morse, bool copy, bool notif) {
            List<string> text_converted = new();
            string[] text_array = morse.Split(" ");

            foreach (string m in text_array) {
                if (morseToText.ContainsKey(m.ToString())) {
                    text_converted.Add(morseToText[m.ToString()]);
                } else {
                    text_converted.Add(m.ToString());
                }
            }

            Utils.NotifCheck(
                notif,
                new string[] {
                    "Success!",
                    $"The message was: {string.Join("", text_converted)}",
                    "10"
                }
            );
            Utils.CopyCheck(copy, string.Join("", text_converted));
            return string.Join("", text_converted);
        }
    }
}
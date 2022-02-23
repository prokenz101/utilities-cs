using System;
using System.Linq;
using System.Collections.Generic;

namespace utilities_cs {
    public class All {
        public static string? ReturnAll(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }

            string category = args[1];
            string? all = returnCategory(args[1..], category, copy, notif);
            if (all != null) {
                Utils.CopyCheck(copy, all);
                Utils.NotifCheck(notif, new string[] { "Success!", "Text copied to clipboard.", "2" });
                return all;
            } else {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.",
                        "It seems you did not input a valid category.",
                        "4"
                    }
                );
                return null;
            }
        }


        static bool isValidCategory(string input) {
            input = input.ToLower();
            switch (input) {
                case "everything":
                    return true;
                case "encodings":
                    return true;
                case "fancy":
                    return true;
                default:
                    return false;
            }
        }

        static string? returnCategory(string[] args, string category, bool copy, bool notif) {
            Dictionary<string, Func<string[], bool, bool, string?>> fancy = new() {
                { "Spacer", Spacer.spacer },
                { "UpperCase", Upper.Uppercase },
                { "LowerCase", Lower.Lowercase },
                { "Cursive", Cursive.cursive },
                { "BubbleText", Bubble.BubbleText },
                { "DoubleStruck", Doublestruck.dbs },
                { "Creepy", Creepy.creepy },
                { "Reversed", Reverse.reverse },
                { "Exponentiated", Exponent.exponent },
                { "Flipped", Flip.flip },
                { "TitleCase", Title.title },
                { "Morse", Morse.MorseCodeTranslate },
                { "MathItalic", MathItalics.MathItalic }
            };
            Dictionary<string, Func<string[], bool, bool, string?>> encodings = new() {
                { "Binary", Binary.Bin },
                { "Hexadecimal", Hex.Hexadecimal },
                { "Base64", Base64Conversion.Base64Convert },
                { "Base32", Base32Conversion.Base32Convert },
                { "GZip", GZip.GZipConversion }
            };

            List<string> b32AndGZipArgs = args[1..].ToList();
            b32AndGZipArgs.Insert(0, "x");
            b32AndGZipArgs.Insert(1, "to");

            List<string> converted = new();
            Action<Dictionary<string, Func<string[], bool, bool, string?>>> base32AndGZIPCheck =
                (Dictionary<string, Func<string[], bool, bool, string?>> dict) => {
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in dict) {
                        switch (kvp.Key) {
                            case "GZip":
                                converted.Add(
                                    $"{kvp.Key}: {kvp.Value.Invoke(b32AndGZipArgs.ToArray(), false, false)}"
                                ); break;
                            case "Base32":
                                converted.Add(
                                    $"{kvp.Key}: {kvp.Value.Invoke(b32AndGZipArgs.ToArray(), false, false)}"
                                ); break;
                            default:
                                converted.Add($"{kvp.Key}: {kvp.Value.Invoke(args, false, false)}");
                                break;
                        }
                    }
                };

            switch (category) {
                case "everything":
                    base32AndGZIPCheck(
                        fancy.Concat(encodings).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    ); break;
                case "encodings": base32AndGZIPCheck(encodings); break;
                case "fancy":
                    foreach (KeyValuePair<string, Func<string[], bool, bool, string?>> kvp in fancy) {
                        converted.Add($@"{kvp.Key}: {kvp.Value.Invoke(args, false, false)}");
                    }
                    break;
                default:
                    return null;
            }
            return string.Join("\n", converted);
        }
    }
}
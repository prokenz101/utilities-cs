using System.Collections.Generic;

namespace utilities_cs {
    public class Fraction {
        public static string? fraction(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) {
                return null;
            }
            string text = string.Join(" ", args[1..]);
            List<string> converted = new();
            Dictionary<char, string[]> fraction_dict = new Dictionary<char, string[]> {
                { '0', new string[]{"⁰", "₀"} },
                { '1', new string[]{"¹", "₁"} },
                { '2', new string[]{"²", "₂"} },
                { '3', new string[]{"³", "₃"} },
                { '4', new string[]{"⁴", "₄"} },
                { '5', new string[]{"⁵", "₅"} },
                { '6', new string[]{"⁶", "₆"} },
                { '7', new string[]{"⁷", "₇"} },
                { '8', new string[]{"⁸", "₈"} },
                { '9', new string[]{"⁹", "₉"} },
                { '=', new string[]{"⁼", "₌"} },
                { '+', new string[]{"⁺", "₊"} },
                { '-', new string[]{"⁻", "₋"} },
                { '(', new string[]{"⁽", "₍"} },
                { ')', new string[]{"⁾", "₎"} },
                { 'a', new string[]{"ᵃ", "ₐ"} },
                { 'b', new string[]{"ᵇ", "failed"} },
                { 'c', new string[]{"ᶜ", "failed"} },
                { 'd', new string[]{"ᵈ", "failed"} },
                { 'e', new string[]{"ᵉ", "ₑ"} },
                { 'f', new string[]{"ᶠ", "failed"} },
                { 'g', new string[]{"ᵍ", "failed"} },
                { 'h', new string[]{"ʰ", "ₕ"} },
                { 'i', new string[]{"ⁱ", "ᵢ"} },
                { 'j', new string[]{"ʲ", "ⱼ"} },
                { 'k', new string[]{"ᵏ", "ₖ"} },
                { 'l', new string[]{"ˡ", "ₗ"} },
                { 'm', new string[]{"ᵐ", "ₘ"} },
                { 'n', new string[]{"ⁿ", "ₙ"} },
                { 'o', new string[]{"ᵒ", "ₒ"} },
                { 'p', new string[]{"ᵖ", "ₚ"} },
                { 'r', new string[]{"ʳ", "ᵣ"} },
                { 's', new string[]{"ˢ", "ₛ"} },
                { 't', new string[]{"ᵗ", "ₜ"} },
                { 'u', new string[]{"ᵘ", "ᵤ"} },
                { 'v', new string[]{"ᵛ", "ᵥ"} },
                { 'w', new string[]{"ʷ", "failed"} },
                { 'x', new string[]{"ˣ", "ₓ"} },
                { 'y', new string[]{"ʸ", "failed"} },
                { 'z', new string[]{"ᶻ", "failed"} },
            };

            string[] slash_split = text.Split("/");
            string numerator = slash_split[0];
            string denominator = slash_split[1];

            foreach (char x in numerator) {
                if (fraction_dict.ContainsKey(x)) {
                    string i = fraction_dict[x][0];
                    converted.Add(i);
                } else {
                    Utils.Notification(
                        "Something went wrong.",
                        "Either the code is broken, or you did not input the parameters correctly.",
                        4
                    );
                    return null;
                }
            }

            converted.Add("⁄");

            foreach (char x in denominator) {
                if (fraction_dict.ContainsKey(x)) {
                    string i = fraction_dict[x][1];
                    if (i != "failed") {
                        converted.Add(i);
                    } else {
                        Utils.Notification("Hey!",
                            @"It seems you tried to input a character that's not supported.",
                            7
                        );
                        return null;
                    }
                } else {
                    Utils.Notification(
                        "Something went wrong.",
                        "Either the code is broken, or you did not input the parameters correctly.",
                        4
                    );
                    return null;
                }
            }

            string fraction = string.Join("", converted);
            Utils.CopyCheck(copy, fraction);
            Utils.NotifCheck(notif, new string[]{"Success!", "Message copied to clipboard.", "3"});
            return fraction;
        }
    }
}

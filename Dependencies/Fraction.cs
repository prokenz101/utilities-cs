using System.Numerics;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public partial class Fractions {

        [GeneratedRegex(@"^(?<numerator>-?\d+)\/(?<denominator>-?\d+)$")]
        private static partial Regex FractionRegex();

        [GeneratedRegex(@"^(?<wholenum>-?\d+) (?<numerator>-?\d+)\/(?<denominator>-?\d+)$")]
        private static partial Regex MixedFractionRegex();

        [GeneratedRegex(@"^(?<numerator1>-?\d+)\/(?<denominator1>-?\d+) and (?<numerator2>-?\d+)\/(?<denominator2>-?\d+)")]
        private static partial Regex OperationFractionRegex();

        [GeneratedRegex(@"^(?<wholenum1>-?\d+) (?<numerator1>-?\d+)\/(?<denominator1>-?\d+) and (?<wholenum2>-?\d+) (?<numerator2>-?\d+)\/(?<denominator2>-?\d+)")]
        private static partial Regex OperationMixedFractionRegex();
        
        [GeneratedRegex(@"^(?<numerator>-?\d+)\/(?<denominator>-?\d+) to (?<conversion>\w+)$")]
        private static partial Regex ConvertFractionRegex();

        [GeneratedRegex(@"^(?<wholenum>-?\d+) (?<numerator>-?\d+)\/(?<denominator>-?\d+) to (?<conversion>\w+)$")]
        private static partial Regex ConvertMixedFractionRegex();

        [GeneratedRegex(@"^if (?<numerator>-?\d+)\/(?<denominator>-?\d+) is (?<check>\w+)$")]
        private static partial Regex CheckFractionRegex();

        [GeneratedRegex(@"^if (?<wholenum>-?\d+) (?<numerator>-?\d+)\/(?<denominator>-?\d+) is (?<check>\w+)$")]
        private static partial Regex CheckMixedFractionRegex();

        public static string? FractionsMain(string[] args, bool copy, bool notif) {
            string mode = args[1];

            if (mode == "get") {
                string text = string.Join(" ", args[2..]);
                List<string> converted = new();

                if (!text.Contains("/")) {
                    Utils.NotifCheck(
                        true,
                        ["An exception occured.", "There must be a / in your string.", "3"],
                        "fractionError"
                    ); return null;
                }

                string[] slashSplit = text.Split("/");
                string numerator = slashSplit[0];
                string denominator = slashSplit[1];

                foreach (char x in numerator) {
                    if (Dictionaries.FractionDict.ContainsKey(x)) {
                        string i = Dictionaries.FractionDict[x][0];
                        converted.Add(i);
                    } else {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception",
                                "Invalid parameters, try 'help' for more info.",
                                "4"
                            ], "fractionError"
                        ); return null;
                    }
                }

                converted.Add("⁄");

                foreach (char x in denominator) {
                    if (Dictionaries.FractionDict.ContainsKey(x)) {
                        string i = Dictionaries.FractionDict[x][1];
                        if (i != "failed") {
                            converted.Add(i);
                        } else {
                            Utils.NotifCheck(
                                true,
                                [
                                    "Exception",
                                    @"Unsupported character inputted, see wiki for more info.",
                                    "4"
                                ], "fractionError"
                            ); return null;
                        }
                    } else {
                        Utils.NotifCheck(
                            true,
                            [
                                "Exception",
                                "Invalid parameters, try 'help' for more info.",
                                "4"
                            ], "fractionError"
                        ); return null;
                    }
                }

                string fraction = string.Join("", converted);
                Utils.CopyCheck(copy, fraction);
                Utils.NotifCheck(
                    notif, ["Success!", "Message copied to clipboard.", "3"], "fractionSuccess"
                ); return fraction;
            } else if (mode == "add" | mode == "subtract" | mode == "multiply" | mode == "divide") {
                Regex fractionRegex = OperationFractionRegex();

                Regex mixedFractionRegex = OperationMixedFractionRegex();

                string input = string.Join(" ", args[2..]);

                if (fractionRegex.IsMatch(input)) {
                    Fraction fc1 = new(
                        fractionRegex.Match(input).Groups["numerator1"]!.Value
                        + "/"
                        + fractionRegex.Match(input).Groups["denominator1"]!.Value
                    );

                    Fraction fc2 = new(
                        fractionRegex.Match(input).Groups["numerator2"]!.Value
                        + "/"
                        + fractionRegex.Match(input).Groups["denominator2"]!.Value
                    );

                    string result = Fraction.Operation(fc1, fc2, mode)!.ToString();

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer is: {result}", "3"], "fractionSuccess"
                    ); return result;
                } else if (mixedFractionRegex.IsMatch(input)) {
                    MixedFraction mfc1 = new(
                        BigInteger.Parse(mixedFractionRegex.Match(input)!.Groups["wholenum1"].Value),
                        mixedFractionRegex.Match(input)!.Groups["numerator1"]!.Value
                        + "/"
                        + mixedFractionRegex.Match(input)!.Groups["denominator1"]!.Value
                    );

                    MixedFraction mfc2 = new(
                        BigInteger.Parse(mixedFractionRegex.Match(input)!.Groups["wholenum2"].Value),
                        mixedFractionRegex.Match(input)!.Groups["numerator2"]!.Value
                        + "/"
                        + mixedFractionRegex.Match(input)!.Groups["denominator2"]!.Value
                    );

                    string result = MixedFraction.Operation(mfc1, mfc2, mode)!.ToString();

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer is: {result}", "3"], "fractionSuccess"
                    ); return result;
                } else {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Invalid syntax, try 'help' for more info.", "3"],
                        "fractionError"
                    ); return null;
                }

            } else if (mode == "simplify") {
                Regex fractionRegex = FractionRegex();
                Regex mixedFractionRegex = MixedFractionRegex();
                string input = string.Join(" ", args[2..]);

                if (fractionRegex.IsMatch(input)) {
                    string result = new Fraction(
                        fractionRegex.Match(input).Groups["numerator"]!.Value
                        + "/"
                        + fractionRegex.Match(input).Groups["denominator"]!.Value
                    ).ToSimplestForm().ToString();

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer is: {result}", "4"], "fractionSuccess"
                    ); return result;
                } else if (mixedFractionRegex.IsMatch(input)) {
                    string result = new MixedFraction(
                        BigInteger.Parse(mixedFractionRegex.Match(input)!.Groups["wholenum"].Value),
                        mixedFractionRegex.Match(input)!.Groups["numerator"].Value
                        + "/"
                        + mixedFractionRegex.Match(input)!.Groups["denominator"].Value
                    ).ToSimplestForm().ToString();

                    Utils.CopyCheck(copy, result);
                    Utils.NotifCheck(
                        notif, ["Success!", $"The answer is: {result}", "4"], "fractionSuccess"
                    ); return result;
                } else {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Invalid syntax, try 'help' for more info.", "3"],
                        "fractionError"
                    ); return null;
                }
            } else if (mode == "convert") {
                Regex fractionRegex = ConvertFractionRegex();
                Regex mixedFractionRegex = ConvertMixedFractionRegex();
                string input = string.Join(" ", args[2..]);

                if (fractionRegex.IsMatch(input)) {
                    Fraction fc = new(
                        fractionRegex.Match(input).Groups["numerator"]!.Value
                        + "/"
                        + fractionRegex.Match(input).Groups["denominator"]!.Value
                    );

                    string conversion = fractionRegex.Match(input).Groups["conversion"].Value;

                    if (
                        conversion == "percent"
                        | conversion == "percentage"
                        | conversion == "decimal"
                        | conversion == "mixed"
                    ) {
                        string ans = conversion == "percent" | conversion == "percentage"
                            ? fc.ToPercentage().ToString() + "%"
                            : conversion == "decimal" ? fc.ToDecimal().ToString()
                            : conversion == "mixed" ? fc.ToMixedFraction().ToString()
                            : "0"; //* should never happen

                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif, ["Success!", $"The answer is: {ans}", "4"], "fractionSuccess"
                        ); return ans;
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "3"],
                            "fractionError"
                        ); return null;
                    }
                } else if (mixedFractionRegex.IsMatch(input)) {
                    MixedFraction mfc = new(
                        BigInteger.Parse(mixedFractionRegex.Match(input).Groups["wholenum"]!.Value),
                        mixedFractionRegex.Match(input).Groups["numerator"]!.Value
                        + "/"
                        + mixedFractionRegex.Match(input).Groups["denominator"]!.Value
                    );

                    string conversion = mixedFractionRegex.Match(input).Groups["conversion"].Value;

                    if (
                        conversion == "percent"
                        | conversion == "percentage"
                        | conversion == "decimal"
                        | conversion == "improper"
                    ) {
                        string ans = conversion == "percent" | conversion == "percentage"
                            ? mfc.ToPercentage().ToString() + "%"
                            : conversion == "decimal" ? mfc.ToDecimal().ToString()
                            : conversion == "improper" ? mfc.ToImproperFraction().ToString()
                            : "0"; //* should never happen

                        Utils.CopyCheck(copy, ans);
                        Utils.NotifCheck(
                            notif, ["Success!", $"The answer is: {ans}", "4"], "fractionSuccess"
                        ); return ans;
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "3"],
                            "fractionError"
                        ); return null;
                    }
                } else {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Invalid syntax, try 'help' for more info.", "3"],
                        "fractionError"
                    ); return null;
                }
            } else if (mode == "check") {
                Regex fractionRegex = CheckFractionRegex();
                Regex mixedFractionRegex = CheckMixedFractionRegex();

                string input = string.Join(" ", args[2..]);
                if (fractionRegex.IsMatch(input)) {
                    Fraction fc = new(
                        fractionRegex.Match(input).Groups["numerator"]!.Value
                        + "/"
                        + fractionRegex.Match(input).Groups["denominator"]!.Value
                    );

                    string check = fractionRegex.Match(input).Groups["check"].Value;
                    if (check == "proper" | check == "improper") {
                        bool result =
                            check == "proper" ? fc.IsProper()
                            : check == "improper" ? fc.IsImproper()
                            : false; //* should never happen

                        Utils.CopyCheck(copy, result.ToString());
                        Utils.NotifCheck(
                            notif, ["Success!", $"The answer is: {result}", "4"], "fractionSuccess"
                        ); return result.ToString();
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid input, try 'help' for more info.", "3"],
                            "fractionError"
                        ); return null;
                    }
                } else if (mixedFractionRegex.IsMatch(input)) {
                    Utils.NotifCheck(
                        true,
                        [
                            "Exception", "Mode does not support Mixed Fractions", "3"
                        ], "fractionError"
                    ); return null;
                } else {
                    Utils.NotifCheck(
                        true,
                        ["Exception", "Invalid syntax, try 'help' for more info.", "3"],
                        "fractionError"
                    ); return null;
                }
            } else {
                Utils.NotifCheck(
                    true, ["Exception", "Invalid mode, try 'help' for more info.", "3"], "fractionError"
                ); return null;
            }
        }
    }

    public class Fraction {
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }

        public Fraction(string fraction) {
            if (!fraction.Contains('/')) {
                throw new NoSlashException();
            }

            var parts = fraction.Split('/');
            Numerator = BigInteger.Parse(parts[0]);
            Denominator = BigInteger.Parse(parts[1]);

            if (Denominator == 0) {
                throw new DivideByZeroException();
            }
        }

        public override string ToString() { return $"{Numerator}/{Denominator}"; }

        public MixedFraction ToMixedFraction() {
            BigInteger Quotient = Numerator / Denominator;
            BigInteger Remainder = Numerator % Denominator;

            //* Q R/D
            return new MixedFraction(Quotient, $"{Remainder}/{Denominator}");
        }

        public double ToDecimal() { return (double)(Numerator / Denominator); }

        public double ToPercentage() { return (double)Numerator / (double)Denominator * 100; }

        public bool IsProper() { return this.Denominator >= this.Numerator; }

        public bool IsImproper() { return this.Denominator < this.Numerator; }

        public static bool AreLike(Fraction fc1, Fraction fc2) { return fc1.Denominator == fc2.Denominator; }

        public static bool AreUnlike(Fraction fc1, Fraction fc2) { return fc1.Denominator != fc2.Denominator; }

        public static Fraction? Operation(Fraction fc1, Fraction fc2, string operation, bool simplify = true) {
            if (operation == "add") {
                return Fraction.Add(fc1, fc2, simplify);
            } else if (operation == "subtract") {
                return Fraction.Subtract(fc1, fc2, simplify);
            } else if (operation == "multiply") {
                return Fraction.Multiply(fc1, fc2, simplify);
            } else if (operation == "divide") {
                return Fraction.Divide(fc1, fc2, simplify);
            } else {
                return null;
            }
        }

        public static Fraction Add(Fraction fc1, Fraction fc2, bool simplify = true) {
            BigInteger lcm = LCMClass.FindLCM([fc1.Denominator, fc2.Denominator]);
            BigInteger numerator = fc1.Numerator * (lcm / fc1.Denominator) + fc2.Numerator * (lcm / fc2.Denominator);
            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Subtract(Fraction fc1, Fraction fc2, bool simplify = true) {
            BigInteger lcm = LCMClass.FindLCM([fc1.Denominator, fc2.Denominator]);
            BigInteger numerator = fc1.Numerator * (lcm / fc1.Denominator) - fc2.Numerator * (lcm / fc2.Denominator);
            return SimplifyIfRequired(new Fraction(numerator.ToString() + "/" + lcm.ToString()), simplify);
        }

        public static Fraction Multiply(Fraction fc1, Fraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                new Fraction($"{fc1.Numerator * fc2.Numerator}/{fc1.Denominator * fc2.Denominator}"), simplify
            );
        }

        public static Fraction Divide(Fraction fc1, Fraction fc2, bool simplify = true) {
            Fraction reciprocal = new Fraction($"{fc2.Denominator}/{fc2.Numerator}");
            return SimplifyIfRequired(Fraction.Multiply(fc1, reciprocal), simplify);
        }

        public Fraction ToSimplestForm() {
            //* get GCD of numerator and denominator
            BigInteger gcd = HCF.FindGCD([Numerator, Denominator], 2);

            Fraction fractionObtained = new(Numerator / gcd + "/" + Denominator / gcd);
            if (fractionObtained.Denominator < 0) {
                //* if denominator is negative, then convert to rational number simplest form
                fractionObtained.Numerator = 0 - fractionObtained.Numerator;
                fractionObtained.Denominator = -1 * fractionObtained.Denominator;
            }

            return fractionObtained;
        }

        public static Fraction SimplifyIfRequired(Fraction fc, bool simplify) {
            if (simplify) {
                return fc.ToSimplestForm();
            } else {
                return fc;
            }
        }

        public class NoSlashException : Exception {
            public NoSlashException() : base("Fraction must be in the form of a/b") { }
        }
    }

    public class MixedFraction {
        public BigInteger WholeNumber { get; set; }
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }

        public MixedFraction(BigInteger wholeNumber, string fraction) {
            if (!fraction.Contains('/')) { throw new Fraction.NoSlashException(); }
            string[] parts = fraction.Split("/");

            WholeNumber = wholeNumber;
            Numerator = BigInteger.Parse(parts[0]);
            Denominator = BigInteger.Parse(parts[1]);
        }

        public override string ToString() { return $"{WholeNumber} {Numerator}/{Denominator}"; }

        public Fraction ToImproperFraction() {
            return new Fraction($"{(WholeNumber * Denominator) + Numerator}/{Denominator}");
        }

        public double ToDecimal() {
            Fraction asImproper = this.ToImproperFraction();
            return (double)asImproper.Numerator / (double)asImproper.Denominator;
        }

        public double ToPercentage() {
            return this.ToDecimal() * 100;
        }

        public MixedFraction ToSimplestForm() {
            return new MixedFraction(
                WholeNumber, new Fraction($"{Numerator}/{Denominator}").ToSimplestForm().ToString()
            );
        }

        public static MixedFraction? Operation(
            MixedFraction mfc1, MixedFraction mfc2, string operation, bool simplify = true
        ) {
            if (operation == "add") {
                return MixedFraction.Add(mfc1, mfc2, simplify);
            } else if (operation == "subtract") {
                return MixedFraction.Subtract(mfc1, mfc2, simplify);
            } else if (operation == "multiply") {
                return MixedFraction.Multiply(mfc1, mfc2, simplify);
            } else if (operation == "divide") {
                return MixedFraction.Divide(mfc1, mfc2, simplify);
            } else {
                return null;
            }
        }

        public static MixedFraction Add(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Add(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Subtract(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Subtract(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Multiply(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Multiply(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction Divide(MixedFraction fc1, MixedFraction fc2, bool simplify = true) {
            return SimplifyIfRequired(
                Fraction.Divide(fc1.ToImproperFraction(), fc2.ToImproperFraction()).ToMixedFraction(), simplify
            );
        }

        public static MixedFraction SimplifyIfRequired(MixedFraction mfc, bool simplify) {
            if (simplify) {
                return mfc.ToSimplestForm();
            } else {
                return mfc;
            }
        }
    }
}
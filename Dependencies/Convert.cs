using System.Text.RegularExpressions;

namespace utilities_cs {
    public partial class Converter {
        [GeneratedRegex(@"(?<amount>^-?\d+(\.\d+)?) (?<fromunit>[a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?)*((\/[a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?)*)?)( [Oo][Ff] [a-zA-Z]+)?) [Tt][Oo] (?<tounit>[a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?)*((\/[a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-.Ωµμ ]+(\^-?\d+(\.\d+)?)?)*)?)( [Oo][Ff] [a-zA-Z]+)?)")]
        private static partial Regex ParseUnitRegex();
        
        public static string? ConvertMain(string[] args, bool copy, bool notif) {
            if (Utils.IndexTest(args)) return null;

            string prompt = string.Join(" ", args[1..]);
            var parser = ParseUnitRegex();

            if (parser.IsMatch(prompt)) {
                double amount = double.Parse(parser.Match(prompt).Groups["amount"].Value);
                Unit? unit1 = Unit.ParseCompoundUnit(parser.Match(prompt).Groups["fromunit"].Value);
                Unit? unit2 = Unit.ParseCompoundUnit(parser.Match(prompt).Groups["tounit"].Value);

                if (unit1 is not null && unit2 is not null) {
                    double? result = ConvertUnits(amount, unit1, unit2);
                    if (result is not null) {
                        string output = $"{result} {unit2.Symbol}";
                        Utils.CopyCheck(copy, output);
                        Utils.NotifCheck(
                            notif,
                            ["Success!", $"{amount} {unit1.Symbol} = {output}.", "3"],
                            "convertSuccess"
                        );
                        return output;
                    }
                }

                return null;
            } else {
                Utils.NotifCheck(
                    true,
                    ["Exception", "Invalid format. Use 'help' for more info.", "3"],
                    "convertError"
                ); return null;
            }
        }

        public static double? ConvertUnits(double value, Unit fromUnit, Unit toUnit) {
            if (!AreDimensionsCompatible(fromUnit.Dimensions, toUnit.Dimensions)) {
                Utils.NotifCheck(true, ["Exception", "Units are not compatible for conversion.", "3"], "convertError");
                return null;
            }

            if (IsMarketUnit(fromUnit) && IsMarketUnit(toUnit)) {
                try {
                    string? fromMarketSymbol = GetMarketSymbol(fromUnit);
                    string? toMarketSymbol = GetMarketSymbol(toUnit);
                    if (fromMarketSymbol is null || toMarketSymbol is null) {
                        Utils.NotifCheck(true, ["Exception", "Unable to determine market symbols for conversion.", "3"], "convertError");
                        return null;
                    }

                    using HttpClient client = new();
                    var response = client.GetStringAsync($"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/{fromMarketSymbol}.min.json");
                    var rates = System.Text.Json.JsonDocument.Parse(response.Result);

                    if (rates.RootElement.TryGetProperty(fromMarketSymbol, out var fromRates)
                        && fromRates.TryGetProperty(toMarketSymbol, out var toRateElement)
                        && toRateElement.ValueKind == System.Text.Json.JsonValueKind.Number) {
                        if (fromUnit.Unittype == Unit.UnitType.Currency) fromUnit.Symbol = fromUnit.Symbol.ToUpper();
                        if (toUnit.Unittype == Unit.UnitType.Currency) toUnit.Symbol = toUnit.Symbol.ToUpper();
                        return value * fromUnit.ConversionFactor * toRateElement.GetDouble() / toUnit.ConversionFactor;
                    }

                    Utils.NotifCheck(true, ["Exception", $"Market rate not found: {fromMarketSymbol.ToUpper()} -> {toMarketSymbol.ToUpper()}", "3"], "convertError");
                    return null;
                } catch { }
            }

            if (IsAbsoluteTemperatureUnit(fromUnit) && IsAbsoluteTemperatureUnit(toUnit) && IsPureTemperature(fromUnit) && IsPureTemperature(toUnit)) {
                double kelvin = ToKelvin(value, fromUnit);
                return FromKelvin(kelvin, toUnit);
            }

            return value * (fromUnit.ConversionFactor / toUnit.ConversionFactor);
        }

        public static bool IsPureTemperature(Unit unit) {
            double[] temperatureDimensions = [0, 0, 0, 0, 1, 0, 0, 0];
            return AreDimensionsCompatible(unit.Dimensions, temperatureDimensions);
        }

        private static bool IsAbsoluteTemperatureUnit(Unit unit) {
            return unit.Symbol is "K" or "c" or "f" or "r";
        }

        private static double ToKelvin(double value, Unit unit) {
            return unit.Symbol switch {
                "K" => value,
                "c" => value + 273.15,
                "f" => (value + 459.67) * (5.0 / 9.0),
                "r" => value * (5.0 / 9.0),
                _ => value
            };
        }

        private static double FromKelvin(double kelvin, Unit unit) {
            return unit.Symbol switch {
                "K" => kelvin,
                "c" => kelvin - 273.15,
                "f" => (kelvin * (9.0 / 5.0)) - 459.67,
                "r" => kelvin * (9.0 / 5.0),
                _ => kelvin
            };
        }

        private static bool AreDimensionsCompatible(double[] left, double[] right) {
            if (left.Length != right.Length)
                return false;

            const double tolerance = 1e-12;
            for (int i = 0; i < left.Length; i++) {
                if (Math.Abs(left[i] - right[i]) > tolerance)
                    return false;
            }

            return true;
        }

        private static bool IsMarketUnit(Unit unit) {
            return unit.Unittype == Unit.UnitType.Currency
                || unit.Unittype == Unit.UnitType.Gold
                || unit.Unittype == Unit.UnitType.Silver
                || unit.Unittype == Unit.UnitType.Platinum
                || unit.Unittype == Unit.UnitType.Palladium;
        }

        private static string? GetMarketSymbol(Unit unit) {
            return unit.Unittype switch {
                Unit.UnitType.Currency => unit.Symbol.ToLower(),
                Unit.UnitType.Gold => "xau",
                Unit.UnitType.Silver => "xag",
                Unit.UnitType.Platinum => "xpt",
                Unit.UnitType.Palladium => "xpd",
                _ => null
            };
        }
    }

    public partial class Unit {
        public string Symbol { get; set; }
        public string[]? AliasSymbols { get; set; }
        public double ConversionFactor { get; set; }
        public double[] Dimensions { get; set; }
        //* [Mass, Length, Time, Electric Current, Temperature, Amount of Substance, Luminous Intensity]
        public SIPrefix SiPrefix { get; set; }
        public UnitType Unittype { get; set; }
        public enum SIPrefix {
            Symbol = 1,
            NotSymbol = 0
        }
        public enum UnitType {
            Physical = 0,
            Currency = 1,
            Gold = 2,
            Silver = 3,
            Platinum = 4,
            Palladium = 5
        }
        [GeneratedRegex(@"(?<unit>[a-zA-Z0-9-. ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-. ]+(\^-?\d+(\.\d+)?)?)*((\/[a-zA-Z0-9-. ]+(\^-?\d+(\.\d+)?)?([*·×][a-zA-Z0-9-. ]+(\^-?\d+(\.\d+)?)?)*)?)) [Oo][Ff] (?<commodity>\w+)")]
        private static partial Regex ParseCommodityRegex();

        public Unit(
            string symbol,
            double conversionFactor,
            double[] dimensions,
            string[]? aliasSymbols,
            SIPrefix siPrefix = SIPrefix.Symbol,
            UnitType unitType = UnitType.Physical
        ) {
            Symbol = symbol;
            ConversionFactor = conversionFactor;
            Dimensions = dimensions;
            AliasSymbols = aliasSymbols;
            SiPrefix = siPrefix;
            Unittype = unitType;
            Units[symbol] = this;
            if (aliasSymbols != null) {
                foreach (var alias in aliasSymbols)
                    Units[alias] = this;
            }
        }
        public static Dictionary<string, Unit> Units = new();
        public static Dictionary<string, double[]> SIConversion = new() {
            //* Long prefix -> 1
            //* Short prefix -> 0
            {"ronto", new[] {1, 1e-27}},
            {"r", new[] {0, 1e-27}},
            {"yocto", new[] {1, 1e-24}},
            {"y", new[] {0, 1e-24}},
            {"zepto", new[] {1, 1e-21}},
            {"z", new[] {0, 1e-21}},
            {"atto", new[] {1, 1e-18}},
            {"a", new[] {0, 1e-18}},
            {"femto", new[] {1, 1e-15}},
            {"f", new[] {0, 1e-15}},
            {"pico", new[] {1, 1e-12}},
            {"p", new[] {0, 1e-12}},
            {"nano", new[] {1, 1e-9}},
            {"n", new[] {0, 1e-9}},
            {"micro", new[] {1, 1e-6}},
            {"μ", new[] {0, 1e-6}},
            {"µ", new[] {0, 1e-6}},
            {"mu ", new[] {0, 1e-6}},
            {"milli", new[] {1, 1e-3}},
            {"m", new[] {0, 1e-3}},
            {"centi", new[] {1, 1e-2}},
            {"c", new[] {0, 1e-2}},
            {"deci", new[] {1, 0.1}},
            {"d", new[] {0, 0.1}},
            {"deca", new[] {1, 10.0}},
            {"deka", new[] {1, 10.0}},
            {"da", new[] {0, 10.0}},
            {"hecto", new[] {1, 100.0}},
            {"h", new[] {0, 100.0}},
            {"kilo", new[] {1, 1000.0}},
            {"k", new[] {0, 1000.0}},
            {"mega", new[] {1, 1e6}},
            {"M", new[] {0, 1e6}},
            {"giga", new[] {1, 1e9}},
            {"G", new[] {0, 1e9}},
            {"tera", new[] {1, 1e12}},
            {"T", new[] {0, 1e12}},
            {"peta", new[] {1, 1e15}},
            {"P", new[] {0, 1e15}},
            {"exa", new[] {1, 1e18}},
            {"E", new[] {0, 1e18}},
            {"zetta", new[] {1, 1e21}},
            {"Z", new[] {0, 1e21}},
            {"yotta", new[] {1, 1e24}},
            {"Y", new[] {0, 1e24}},
            {"ronna", new[] {1, 1e27}},
            {"R", new[] {0, 1e27}}
        };

        public static void RegisterUnits() {
            //* Fundamental units
            new Unit("g", 1e-3, [1, 0, 0, 0, 0, 0, 0, 0], ["gram", "grams"]);
            new Unit("m", 1.0, [0, 1, 0, 0, 0, 0, 0, 0], ["meter", "metre", "meters", "metres"]);
            new Unit("s", 1.0, [0, 0, 1, 0, 0, 0, 0, 0], ["second", "sec", "s", "seconds", "secs"]);
            new Unit("A", 1.0, [0, 0, 0, 1, 0, 0, 0, 0], ["ampere", "amperes", "amps"]);
            new Unit("K", 1.0, [0, 0, 0, 0, 1, 0, 0, 0], ["kelvin"]);
            new Unit("mol", 1.0, [0, 0, 0, 0, 0, 1, 0, 0], ["mole", "moles"]);
            new Unit("cd", 1.0, [0, 0, 0, 0, 0, 0, 1, 0], ["candela", "candelas"]);
            new Unit("b", 1.0, [0, 0, 0, 0, 0, 0, 0, 1], ["bit", "bits"]);

            //* Derived units

            //* Mass
            new Unit("quintal", 100.0, [1, 0, 0, 0, 0, 0, 0, 0], ["quintals"], SIPrefix.NotSymbol);
            new Unit("ton", 1000.0, [1, 0, 0, 0, 0, 0, 0, 0], ["tons", "tonne", "tonnes"], SIPrefix.NotSymbol);
            new Unit("lb", 0.453592, [1, 0, 0, 0, 0, 0, 0, 0], ["pound", "pounds"]);
            new Unit("oz", 0.0283495, [1, 0, 0, 0, 0, 0, 0, 0], ["ounce", "ounces"]);
            new Unit("troy oz", 0.0311035, [1, 0, 0, 0, 0, 0, 0, 0], ["troy ounce", "troy ounces"]);
            new Unit("stone", 6.35029, [1, 0, 0, 0, 0, 0, 0, 0], ["stones"], SIPrefix.NotSymbol);
            new Unit("amu", 1.66054e-27, [1, 0, 0, 0, 0, 0, 0, 0], ["atomic mass unit", "atomic mass units"]);
            new Unit("u", 1.66054e-27, [1, 0, 0, 0, 0, 0, 0, 0], ["unified atomic mass unit", "unified atomic mass units", "unified mass unit", "unified mass units"]);
            new Unit("kip", 444.822, [1, 0, 0, 0, 0, 0, 0, 0], ["kilopound", "kilopounds"]);
            new Unit("planck mass", 2.176709999e-8, [1, 0, 0, 0, 0, 0, 0, 0], ["planck masses"], SIPrefix.NotSymbol);
            new Unit("electron mass", 9.109389699e-31, [1, 0, 0, 0, 0, 0, 0, 0], ["electron masses"], SIPrefix.NotSymbol);
            new Unit("proton mass", 1.672623099e-27, [1, 0, 0, 0, 0, 0, 0, 0], ["proton masses"], SIPrefix.NotSymbol);
            new Unit("neutron mass", 1.6749286e-27, [1, 0, 0, 0, 0, 0, 0, 0], ["neutron masses"], SIPrefix.NotSymbol);
            new Unit("deuteron mass", 3.343585999e-27, [1, 0, 0, 0, 0, 0, 0, 0], ["deuteron masses"], SIPrefix.NotSymbol);
            new Unit("earth mass", 5.972e24, [1, 0, 0, 0, 0, 0, 0, 0], ["earth masses"], SIPrefix.NotSymbol);
            new Unit("sun mass", 1.989e30, [1, 0, 0, 0, 0, 0, 0, 0], ["sun masses", "solar mass", "solar masses"], SIPrefix.NotSymbol);

            //* Length
            new Unit("in", 0.0254, [0, 1, 0, 0, 0, 0, 0, 0], ["inch", "inches"]);
            new Unit("ft", 0.3048, [0, 1, 0, 0, 0, 0, 0, 0], ["foot", "feet"]);
            new Unit("yd", 0.9144, [0, 1, 0, 0, 0, 0, 0, 0], ["yard", "yards"]);
            new Unit("mi", 1609.34, [0, 1, 0, 0, 0, 0, 0, 0], ["mile", "miles"]);
            new Unit("ly", 9.461e15, [0, 1, 0, 0, 0, 0, 0, 0], ["light-year", "light-years", "light year", "light years"]);
            new Unit("pc", 3.086e16, [0, 1, 0, 0, 0, 0, 0, 0], ["parsec", "parsecs"]);
            new Unit("AU", 1.496e11, [0, 1, 0, 0, 0, 0, 0, 0], ["astronomical unit", "astronomical units"]);
            new Unit("NM", 1852.0, [0, 1, 0, 0, 0, 0, 0, 0], ["nautical mile", "nautical miles"]);
            new Unit("micron", 1e-6, [0, 1, 0, 0, 0, 0, 0, 0], null, SIPrefix.NotSymbol);
            new Unit("angstrom", 1e-10, [0, 1, 0, 0, 0, 0, 0, 0], ["angstroms"], SIPrefix.NotSymbol);
            new Unit("fermi", 1e-15, [0, 1, 0, 0, 0, 0, 0, 0], ["fermis"], SIPrefix.NotSymbol);
            new Unit("furlong", 201.168, [0, 1, 0, 0, 0, 0, 0, 0], ["furlongs"], SIPrefix.NotSymbol);
            new Unit("rod", 5.0292, [0, 1, 0, 0, 0, 0, 0, 0], ["rod", "rods"], SIPrefix.NotSymbol);
            new Unit("chain", 20.1168, [0, 1, 0, 0, 0, 0, 0, 0], ["chain", "chains"], SIPrefix.NotSymbol);
            new Unit("league", 4828.03, [0, 1, 0, 0, 0, 0, 0, 0], ["league", "leagues"], SIPrefix.NotSymbol);
            new Unit("planck length", 1.616255e-35, [0, 1, 0, 0, 0, 0, 0, 0], ["planck lengths"], SIPrefix.NotSymbol);
            new Unit("electron radius", 2.81794092e-15, [0, 1, 0, 0, 0, 0, 0, 0], ["classical electron radius", "classical electron radii"], SIPrefix.NotSymbol);
            new Unit("sun radius", 696342000, [0, 1, 0, 0, 0, 0, 0, 0], ["sun radius", "solar radius", "solar radii"], SIPrefix.NotSymbol);
            new Unit("earth equatorial radius", 6378160, [0, 1, 0, 0, 0, 0, 0, 0], ["earth equatorial radii"], SIPrefix.NotSymbol);
            new Unit("earth polar radius", 6356777, [0, 1, 0, 0, 0, 0, 0, 0], ["earth polar radii"], SIPrefix.NotSymbol);

            //* Area
            new Unit("ha", 10000.0, [0, 2, 0, 0, 0, 0, 0, 0], ["hectare", "hectares"]);
            new Unit("acre", 4046.86, [0, 2, 0, 0, 0, 0, 0, 0], ["acre", "acres"], SIPrefix.NotSymbol);
            new Unit("barn", 1e-28, [0, 2, 0, 0, 0, 0, 0, 0], ["barns"], SIPrefix.NotSymbol);

            //* Volume
            new Unit("L", 0.001, [0, 3, 0, 0, 0, 0, 0, 0], ["liter", "liters", "litre", "litres"]);
            new Unit("cc", 1e-6, [0, 3, 0, 0, 0, 0, 0, 0], ["cubic centimeter", "cubic centimeters"]);
            new Unit("gal", 0.00378541, [0, 3, 0, 0, 0, 0, 0, 0], ["gallon", "gallons", "gals", "gal.", "gals."]);
            new Unit("qt", 0.000946353, [0, 3, 0, 0, 0, 0, 0, 0], ["quart", "quarts"]);
            new Unit("pt", 0.000473176, [0, 3, 0, 0, 0, 0, 0, 0], ["pint", "pints"]);
            new Unit("cup", 0.000236588, [0, 3, 0, 0, 0, 0, 0, 0], ["cup", "cups"], SIPrefix.NotSymbol);
            new Unit("teaspoon", 0.0000049289, [0, 3, 0, 0, 0, 0, 0, 0], ["teaspoons", "tsp", "tsps"], SIPrefix.NotSymbol);
            new Unit("tablespoon", 0.0000147868, [0, 3, 0, 0, 0, 0, 0, 0], ["tablespoons", "tbsp", "tbsps"], SIPrefix.NotSymbol);
            new Unit("fl oz", 2.9574e-5, [0, 3, 0, 0, 0, 0, 0, 0], ["fluid ounce", "fluid ounces"]);
            new Unit("earth volume", 1.082999999e21, [0, 3, 0, 0, 0, 0, 0, 0], ["earth volumes"], SIPrefix.NotSymbol);

            //* Time
            new Unit("min", 60.0, [0, 0, 1, 0, 0, 0, 0, 0], ["minute", "minutes"]);
            new Unit("hr", 3600.0, [0, 0, 1, 0, 0, 0, 0, 0], ["hour", "hours"]);
            new Unit("week", 604800.0, [0, 0, 1, 0, 0, 0, 0, 0], ["weeks"], SIPrefix.NotSymbol);
            new Unit("day", 86400.0, [0, 0, 1, 0, 0, 0, 0, 0], ["days"], SIPrefix.NotSymbol);
            new Unit("yr", 31536000.0, [0, 0, 1, 0, 0, 0, 0, 0], ["year", "years", "calendar year", "calendar years"]);
            new Unit("decade", 315360000.0, [0, 0, 1, 0, 0, 0, 0, 0], ["decade", "decades"], SIPrefix.NotSymbol);
            new Unit("century", 3153600000.0, [0, 0, 1, 0, 0, 0, 0, 0], ["century", "centuries"], SIPrefix.NotSymbol);
            new Unit("millennium", 31536000000.0, [0, 0, 1, 0, 0, 0, 0, 0], ["millennium", "millennia"], SIPrefix.NotSymbol);
            new Unit("planck time", 5.390559999e-44, [0, 0, 1, 0, 0, 0, 0, 0], ["planck times"], SIPrefix.NotSymbol);

            //* Temperature
            new Unit("c", 1.0, [0, 0, 0, 0, 1, 0, 0, 0], ["celsius", "centigrade", "degree celsius", "degrees celsius"], SIPrefix.NotSymbol);
            new Unit("f", 5.0 / 9.0, [0, 0, 0, 0, 1, 0, 0, 0], ["fahrenheit", "degree fahrenheit", "degrees fahrenheit"], SIPrefix.NotSymbol);
            new Unit("r", 5.0 / 9.0, [0, 0, 0, 0, 1, 0, 0, 0], ["rankine", "degree rankine", "degrees rankine"], SIPrefix.NotSymbol);

            //* Force
            new Unit("N", 1.0, [1, 1, -2, 0, 0, 0, 0, 0], ["newton", "newtons"]);
            new Unit("dyn", 1e-5, [1, 1, -2, 0, 0, 0, 0, 0], ["dyne", "dynes", "dyns"]);
            new Unit("lbf", 4.44822, [1, 1, -2, 0, 0, 0, 0, 0], ["pound-force", "pound-forces"]);
            new Unit("kgf", 9.80665, [1, 1, -2, 0, 0, 0, 0, 0], ["kilogram-force", "kilogram-forces"]);
            new Unit("kipf", 4448.22, [1, 1, -2, 0, 0, 0, 0, 0], ["kip-force", "kip-forces", "kilopound-force", "kilopound-forces"]);
            new Unit("ozf", 0.278013851, [1, 1, -2, 0, 0, 0, 0, 0], ["ounce-force", "ounce-forces"]);
            new Unit("poundal", 0.1382549544, [1, 1, -2, 0, 0, 0, 0, 0], ["poundals"], SIPrefix.NotSymbol);
            new Unit("ton-force", 9806.65, [1, 1, -2, 0, 0, 0, 0, 0], ["ton-forces"], SIPrefix.NotSymbol);

            //* Energy
            new Unit("J", 1.0, [1, 2, -2, 0, 0, 0, 0, 0], ["joule", "joules"]);
            new Unit("erg", 1e-7, [1, 2, -2, 0, 0, 0, 0, 0], ["erg", "ergs"]);
            new Unit("cal", 4.184, [1, 2, -2, 0, 0, 0, 0, 0], ["calorie", "calories"]);
            new Unit("kWh", 3.6e6, [1, 2, -2, 0, 0, 0, 0, 0], ["kilowatt-hour", "kilowatt-hours"]);
            new Unit("eV", 1.60218e-19, [1, 2, -2, 0, 0, 0, 0, 0], ["electronvolt", "electronvolts", "electron volt", "electron volts", "electron-volt", "electron-volts"]);
            new Unit("BTU", 1055.06, [1, 2, -2, 0, 0, 0, 0, 0], ["british thermal unit", "british thermal units"]);

            //* Power
            new Unit("W", 1.0, [1, 2, -3, 0, 0, 0, 0, 0], ["watt", "watts"]);
            new Unit("hp", 745.7, [1, 2, -3, 0, 0, 0, 0, 0], ["horsepower", "horsepower"]);

            //* Pressure
            new Unit("Pa", 1.0, [1, -1, -2, 0, 0, 0, 0, 0], ["pascal", "pascals"]);
            new Unit("bar", 1e5, [1, -1, -2, 0, 0, 0, 0, 0], ["bars"]);
            new Unit("atm", 101325.0, [1, -1, -2, 0, 0, 0, 0, 0], ["atmosphere", "atmospheres"]);
            new Unit("psi", 6894.7572932, [1, -1, -2, 0, 0, 0, 0, 0], ["pound per square inch", "pounds per square inch"]);
            new Unit("ksi", 6894757.2932, [1, -1, -2, 0, 0, 0, 0, 0], ["kips per square inch", "kilopounds per square inch", "kilopound per square inch"]);
            new Unit("torr", 133.32236842, [1, -1, -2, 0, 0, 0, 0, 0], ["torrs"]);
            new Unit("mmHg", 133.322, [1, -1, -2, 0, 0, 0, 0, 0], ["millimeters of mercury", "millimeter of mercury"]);
            new Unit("cmHg", 1333.22, [1, -1, -2, 0, 0, 0, 0, 0], ["centimeters of mercury", "centimeter of mercury"]);
            new Unit("inHg", 3386.38, [1, -1, -2, 0, 0, 0, 0, 0], ["inches of mercury", "inch of mercury"]);

            //* Speed
            new Unit("knot", 0.514444, [0, 1, -1, 0, 0, 0, 0, 0], ["knot", "knots"], SIPrefix.NotSymbol);
            new Unit("kph", 0.277778, [0, 1, -1, 0, 0, 0, 0, 0], ["kilometer per hour", "kilometers per hour"], SIPrefix.NotSymbol);
            new Unit("kmh", 0.277778, [0, 1, -1, 0, 0, 0, 0, 0], null, SIPrefix.NotSymbol);
            new Unit("mph", 0.44704, [0, 1, -1, 0, 0, 0, 0, 0], ["mile per hour", "miles per hour"], SIPrefix.NotSymbol);
            new Unit("fps", 0.3048, [0, 1, -1, 0, 0, 0, 0, 0], ["foot per second", "feet per second"], SIPrefix.NotSymbol);
            new Unit("mach", 343, [0, 1, -1, 0, 0, 0, 0, 0], null);
            new Unit("speed of light", 299792458.0, [0, 1, -1, 0, 0, 0, 0, 0], ["light speed", "lightspeed"], SIPrefix.NotSymbol);

            //* Frequency
            new Unit("Hz", 1.0, [0, 0, -1, 0, 0, 0, 0, 0], ["hertz"]);

            //* Angle
            new Unit("rad", 1.0, [0, 0, 0, 0, 0, 0, 0, 0], ["radian", "radians"]);
            new Unit("deg", Math.PI / 180.0, [0, 0, 0, 0, 0, 0, 0, 0], ["degree", "degrees"]);
            new Unit("grad", Math.PI / 200.0, [0, 0, 0, 0, 0, 0, 0, 0], ["gradian", "gradians"]);
            new Unit("arcminute", Math.PI / 10800.0, [0, 0, 0, 0, 0, 0, 0, 0], ["arcminutes"], SIPrefix.NotSymbol);
            new Unit("arcsecond", Math.PI / 648000.0, [0, 0, 0, 0, 0, 0, 0, 0], ["arcseconds"], SIPrefix.NotSymbol);

            //* Electric Charge
            new Unit("C", 1.0, [0, 0, 1, 1, 0, 0, 0, 0], ["coulomb", "coulombs"]);
            new Unit("e", 1.60218e-19, [0, 0, 1, 1, 0, 0, 0, 0], ["elementary charge", "elementary charges"]);
            new Unit("F", 96485.309, [0, 0, 1, 1, 0, 0, 0, 0], ["faradays"]);
            new Unit("abC", 10.0, [0, 0, 1, 1, 0, 0, 0, 0], ["abcoulomb", "abcoulombs"]);
            new Unit("stC", 3.33564e-10, [0, 0, 1, 1, 0, 0, 0, 0], ["statcoulomb", "statcoulombs", "franklin", "franklins", "statC"]);

            //* Electric Potential
            new Unit("V", 1.0, [1, 2, -3, -1, 0, 0, 0, 0], ["volt", "volts"]);
            new Unit("stV", 299.7925, [1, 2, -3, -1, 0, 0, 0, 0], ["statvolt", "statvolts", "statV"]);
            new Unit("abV", 1e-8, [1, 2, -3, -1, 0, 0, 0, 0], ["abvolt", "abvolts"]);

            //* Electric Current
            new Unit("Bi", 10, [0, 0, 0, 1, 0, 0, 0, 0], ["biot", "biots", "abampere", "abamperes"]);
            new Unit("abA", 10, [0, 0, 0, 1, 0, 0, 0, 0], ["abampere", "abamperes"]);
            new Unit("stA", 3.335641e-10, [0, 0, 0, 1, 0, 0, 0, 0], ["statampere", "statamperes", "statA"]);

            //* Linear Current Density
            new Unit("Oe", 79.57747151, [0, -1, 0, 1, 0, 0, 0, 0], ["oersted", "oersteds"]);

            //* Electrical Resistance / Impedance
            new Unit("Ω", 1.0, [1, 2, -3, -2, 0, 0, 0, 0], ["ohm", "ohms"]);
            new Unit("stΩ", 898755200000, [1, 2, -3, -2, 0, 0, 0, 0], ["statohm", "statohms", "statΩ"]);
            new Unit("abΩ", 1e-9, [1, 2, -3, -2, 0, 0, 0, 0], ["abohm", "abohms"]);
            new Unit("megohm", 1e6, [1, 2, -3, -2, 0, 0, 0, 0], ["megohms"], SIPrefix.NotSymbol);
            new Unit("microhm", 1e-6, [1, 2, -3, -2, 0, 0, 0, 0], ["microhms"], SIPrefix.NotSymbol);

            //* Electrical Conductance
            new Unit("S", 1.0, [-1, -2, 3, 2, 0, 0, 0, 0], ["siemen", "siemens"]);
            new Unit("mho", 1.0, [-1, -2, 3, 2, 0, 0, 0, 0], null);
            new Unit("statmho", 1.112347052e-12, [-1, -2, 3, 2, 0, 0, 0, 0], null);
            new Unit("abmho", 1e9, [-1, -2, 3, 2, 0, 0, 0, 0], null);
            new Unit("gemmho", 0.000001, [-1, -2, 3, 2, 0, 0, 0, 0], null, SIPrefix.NotSymbol);
            new Unit("micromho", 0.000001, [-1, -2, 3, 2, 0, 0, 0, 0], null, SIPrefix.NotSymbol);

            //* Electrical Capacitance
            new Unit("F", 1.0, [-1, -2, 4, 2, 0, 0, 0, 0], ["farad", "farads"]);
            new Unit("stF", 1.112650056e-12, [-1, -2, 4, 2, 0, 0, 0, 0], ["statfarad", "statfarads", "statF"]);
            new Unit("abF", 1000000000, [-1, -2, 4, 2, 0, 0, 0, 0], ["abfarad", "abfarads"]);

            //* Inductance
            new Unit("H", 1.0, [1, 2, -2, -2, 0, 0, 0, 0], ["henry"]);
            new Unit("stH", 898755200000, [1, 2, -2, -2, 0, 0, 0, 0], ["stathenry", "statH"]);
            new Unit("abH", 1e-9, [1, 2, -2, -2, 0, 0, 0, 0], ["abhenry"]);

            //* Luminous Intensity
            new Unit("lm", 1.0, [0, 0, 0, 0, 0, 0, 1, 0], ["lumen", "lumens"]);
            new Unit("candle", 1.0, [0, 0, 0, 0, 0, 0, 1, 0], ["candles"], SIPrefix.NotSymbol);

            //* Luminance
            new Unit("sb", 10000, [0, -2, 0, 0, 0, 0, 1, 0], ["stilb", "stilbs"]);
            new Unit("nit", 1.0, [0, -2, 0, 0, 0, 0, 1, 0], ["nits"]);
            new Unit("lambert", 3183.0988618, [0, -2, 0, 0, 0, 0, 1, 0], ["lamberts"], SIPrefix.NotSymbol);
            new Unit("foot-lambert", 3.4262590996, [0, -2, 0, 0, 0, 0, 1, 0], ["foot-lamberts"], SIPrefix.NotSymbol);

            //* Illumination
            new Unit("lx", 1.0, [0, -2, 0, 0, 0, 0, 1, 0], ["lux"]);
            new Unit("phot", 10000, [0, -2, 0, 0, 0, 0, 1, 0], ["phot", "phots"]);

            //* Magnetic Flux
            new Unit("Wb", 1.0, [1, 2, -2, -1, 0, 0, 0, 0], ["weber", "webers"]);
            new Unit("Mx", 1e-8, [1, 2, -2, -1, 0, 0, 0, 0], ["maxwell", "maxwells"]);

            //* Magnetic Flux Density
            new Unit("T", 1.0, [1, 0, -2, -1, 0, 0, 0, 0], ["tesla"]);
            new Unit("G", 1e-4, [1, 0, -2, -1, 0, 0, 0, 0], ["gauss", "gausses"]);

            //* Molarity
            new Unit("molar", 1.0, [0, -3, 0, 0, 0, 1, 0, 0], ["molars"]);

            //* Molality
            new Unit("molal", 1.0, [-1, 0, 0, 0, 0, 1, 0, 0], ["molals"]);

            //* Viscosity
            new Unit("P", 0.1, [1, -1, -1, 0, 0, 0, 0, 0], ["poise", "poises"]);

            //* Data
            new Unit("B", 8.0, [0, 0, 0, 0, 0, 0, 0, 1], ["byte", "bytes"]);
        }

        public static Unit Multiply(Unit a, Unit b) {
            double[] newDims = new double[a.Dimensions.Length];
            for (int i = 0; i < newDims.Length; i++)
                newDims[i] = a.Dimensions[i] + b.Dimensions[i];

            return new Unit(
                "",
                0,
                newDims,
                null
            ) {
                Symbol = a.Symbol + "·" + b.Symbol,
                ConversionFactor = a.ConversionFactor * b.ConversionFactor,
                Dimensions = newDims
            };
        }

        public static Unit Divide(Unit a, Unit b) {
            double[] newDims = new double[a.Dimensions.Length];
            for (int i = 0; i < newDims.Length; i++)
                newDims[i] = a.Dimensions[i] - b.Dimensions[i];

            return new Unit(
                "",
                0,
                newDims,
                null
            ) {
                Symbol = a.Symbol + "/" + b.Symbol,
                ConversionFactor = a.ConversionFactor / b.ConversionFactor,
                Dimensions = newDims
            };
        }

        public static Unit Raise(Unit unit, double exponent) {
            double[] newDims = new double[unit.Dimensions.Length];
            for (int i = 0; i < newDims.Length; i++)
                newDims[i] = unit.Dimensions[i] * exponent;

            return new Unit(
                $"{unit.Symbol}^{exponent}",
                Math.Pow(unit.ConversionFactor, exponent),
                newDims,
                null
            );
        }

        public static Unit? Parse(string symbol) {
            var succession = "";

            for (int i = 0; i < symbol.Length; i++) {
                char c = symbol[i];

                succession += c;
                if (SIConversion.ContainsKey(succession)) {
                    string remaining = symbol[(i + 1)..];
                    if (Units.TryGetValue(remaining, out Unit? val)) {
                        bool isSymbol = val.Symbol == remaining;
                        //* If we are dealing with symbol, then only allow short SI prefixes

                        if (((SIConversion[succession][0] == 0 && isSymbol && Units[remaining].SiPrefix == SIPrefix.Symbol) || SIConversion[succession][0] == 1) && !Converter.IsPureTemperature(Units[remaining])) {
                            return new Unit(
                                symbol,
                                Units[remaining].ConversionFactor * SIConversion[succession][1],
                                Units[remaining].Dimensions,
                                null
                            ) {
                                Symbol = symbol
                            };
                        }
                    }
                }
            }

            if (Units.TryGetValue(symbol, out Unit? value)) {
                return value;
            }

            //* Check if it's a currency code
            try {
                using var client = new HttpClient();
                var response = client.GetStringAsync("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.min.json").Result;
                var currencies = System.Text.Json.JsonDocument.Parse(response);
                if (currencies.RootElement.TryGetProperty(symbol.ToLower(), out _)) {
                    return new Unit(symbol.ToLower(), 1.0, [0, 0, 0, 0, 0, 0, 0, 0], null, SIPrefix.Symbol, UnitType.Currency);
                }
            } catch { }

            Utils.NotifCheck(true, ["Exception", "Unknown unit: " + symbol, "4"], "convertParserError");
            return null;
        }

        public static Unit? ParseCompoundUnit(string compoundSymbols) {

            //* Check if it matches commodity format
            var commodityFormat = ParseCommodityRegex();
            var commodityMatch = commodityFormat.Match(compoundSymbols);
            if (commodityMatch.Success && commodityMatch.Index == 0 && commodityMatch.Length == compoundSymbols.Length) {
                Dictionary<string, UnitType> commoditySymbols = new() {
                    {"gold", UnitType.Gold},
                    {"au", UnitType.Gold},
                    {"xau", UnitType.Gold},
                    {"silver", UnitType.Silver},
                    {"ag", UnitType.Silver},
                    {"xag", UnitType.Silver},
                    {"platinum", UnitType.Platinum},
                    {"pt", UnitType.Platinum},
                    {"xpt", UnitType.Platinum},
                    {"palladium", UnitType.Palladium},
                    {"pd", UnitType.Palladium},
                    {"xpd", UnitType.Palladium},
                };

                string commodity = commodityMatch.Groups["commodity"].Value.ToLower();

                if (commoditySymbols.TryGetValue(commodity, out UnitType commodityType)) {
                    double? conversionFactor = Converter.ConvertUnits(
                        1,
                        ParseCompoundUnit(commodityMatch.Groups["unit"].Value)!,
                        Units["troy oz"]
                    );
                    if (conversionFactor is not null) {
                        return new Unit(
                            compoundSymbols,
                            (double)conversionFactor,
                            [0, 0, 0, 0, 0, 0, 0, 0],
                            null, SIPrefix.Symbol, commodityType
                        );
                    }
                }
            }

            Func<Unit?> invalid = () => {
                Utils.NotifCheck(
                true, ["Exception", "Invalid units format: " + compoundSymbols, "3"], "convertParserError"
            );
                return null;
            };

            if (compoundSymbols.Contains('/')) {
                string numerator = compoundSymbols.Split('/')[0];
                string denominator = compoundSymbols.Split('/')[1];

                Unit? numUnit = ParseCompoundUnit(numerator);
                Unit? denomUnit = ParseCompoundUnit(denominator);
                if (numUnit is not null && denomUnit is not null)
                    return Divide(numUnit, denomUnit);

                return invalid();

            } else if (compoundSymbols.Contains('*')) {
                string[] parts = compoundSymbols.Split('*');
                Unit? initial = ParseCompoundUnit(parts[0]);

                if (initial is not null) {
                    for (int i = 1; i < parts.Length; i++) {
                        Unit? nextUnit = ParseCompoundUnit(parts[i]);
                        if (nextUnit is not null)
                            initial = Multiply(initial, nextUnit);
                    }
                    return initial;
                }
                return invalid();

            } else if (compoundSymbols.Contains('^')) {
                string[] parts = compoundSymbols.Split('^');
                Unit? baseUnit = ParseCompoundUnit(parts[0]);
                if (baseUnit is not null) {
                    if (double.TryParse(parts[1], out double exponent)) {
                        return Raise(baseUnit, exponent);
                    } else {
                        Utils.NotifCheck(
                            true,
                            ["Exception", "Invalid exponent in unit: " + parts[1], "3"],
                            "convertParserError"
                        ); return null;
                    }
                }
                return invalid();

            } else {
                return Parse(compoundSymbols);
            }
        }
    }
}
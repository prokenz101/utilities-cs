namespace utilities_cs {
    public static class DictionaryExtensions {
        public static string ReplaceKeyInString(this Dictionary<string, string> dictionary, string inputString) {
            var regex = new System.Text.RegularExpressions.Regex("{(.*?)}");
            var matches = regex.Matches(inputString);
            foreach (System.Text.RegularExpressions.Match? match in matches) {
                if (match != null) {
                    var valueWithoutBrackets = match.Groups[1].Value;
                    var valueWithBrackets = match.Value;

                    if (dictionary.ContainsKey(valueWithoutBrackets))
                        inputString = inputString.Replace(valueWithBrackets, dictionary[valueWithoutBrackets]);
                }
            }

            return inputString;
        }
    }
}
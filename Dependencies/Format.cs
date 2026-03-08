using System.Text.RegularExpressions;
using System.Text;

namespace utilities_cs {
    public static partial class Format {

        [GeneratedRegex(@"{(?<command>[^}]+)}")]
        private static partial Regex CommandRegex();

        [GeneratedRegex("{(.*?)}")]
        private static partial Regex ReplaceRegex();

        public static void FormatMain(string[] args) {
            if (Utils.IndexTest(args)) { return; }

            string? result = FindAndReplace(string.Join(" ", args[1..]));
            if (result is not null) {
                Utils.CopyCheck(true, result);
                Utils.NotifCheck(true, ["Success!", "Message copied to clipboard.", "3"], "formatSuccess");
            }
        }

        static string? FindAndReplace(string? input) {
            ArgumentNullException.ThrowIfNull(input);

            while (true) {
                int openIndex = -1;
                for (int i = input.Length - 1; i >= 0; i--) {
                    if (input[i] != '{') {
                        continue;
                    }

                    int slashCount = 0;
                    for (int j = i - 1; j >= 0 && input[j] == '\\'; j--) {
                        slashCount++;
                    }

                    if (slashCount % 2 == 0) {
                        openIndex = i;
                        break;
                    }
                }

                if (openIndex == -1) {
                    break;
                }

                int closeIndex = -1;
                for (int i = openIndex; i < input.Length; i++) {
                    if (input[i] != '}') {
                        continue;
                    }

                    int slashCount = 0;
                    for (int j = i - 1; j >= 0 && input[j] == '\\'; j--) {
                        slashCount++;
                    }

                    if (slashCount % 2 == 0) {
                        closeIndex = i;
                        break;
                    }
                }

                if (closeIndex == -1) {
                    Utils.NotifCheck(true, ["Exception", "Unmatched opening braces.", "3"], "formatError");
                    return null;
                }

                string inner = input.Substring(openIndex + 1, closeIndex - openIndex - 1);
                string[] commandParts = inner.Split(" ");
                string replacement = FormattableCommand.FindAndExecute(commandParts[0], commandParts, false, false) ?? "errored";
                if (replacement == "errored") {
                    Utils.NotifCheck(true, ["Exception", $"Error executing command.", "3"], "formatError");
                    return null;
                }

                input = input[..openIndex] + replacement + input[(closeIndex + 1)..];
            }

            for (int i = input.Length - 1; i >= 0; i--) {
                if (input[i] != '}') {
                    continue;
                }

                int slashCount = 0;
                for (int j = i - 1; j >= 0 && input[j] == '\\'; j--) {
                    slashCount++;
                }

                if (slashCount % 2 == 0) {
                    Utils.NotifCheck(true, ["Exception", "Unmatched closing braces.", "3"], "formatError");
                    return null;
                }
            }

            var output = new StringBuilder(input.Length);
            int index = 0;

            while (index < input.Length) {
                if (input[index] != '\\') {
                    output.Append(input[index]);
                    index++;
                    continue;
                }

                int slashStart = index;
                while (index < input.Length && input[index] == '\\') {
                    index++;
                }

                int slashCount = index - slashStart;
                bool nextIsBrace = index < input.Length && (input[index] == '{' || input[index] == '}');

                if (nextIsBrace && slashCount % 2 == 1) {
                    output.Append('\\', slashCount - 1);
                    output.Append(input[index]);
                    index++;
                    continue;
                }

                output.Append('\\', slashCount);
                if (index < input.Length) {
                    output.Append(input[index]);
                    index++;
                }
            }

            return output.ToString();
        }
    }
}
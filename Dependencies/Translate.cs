namespace utilities_cs {
    public class Translate {
        public static Dictionary<HashSet<string>, string> languages = new() {
            { new() { "tofrench", "french", "f" }, "fr" },
            { new() { "toarabic", "arabic", "a" }, "ar" },
            { new() { "tospanish", "spanish", "s" }, "es" },
            { new() { "todutch", "dutch", "d" }, "nl" },
            { new() { "tochinese", "chinese", "c" }, "zh-TW" },
            { new() { "tojapanese", "japanese", "j" }, "ja" }
        };
        public static Dictionary<string, Action<string>> englishDict = new() {
            { "toenglish", toEnglish },
            { "english", toEnglish },
            { "e", toEnglish }
        };

        public static void TranslateMain(string[] args) {
            string lang = args[1];
            string text = string.Join('+', args[2..]);

            //* checking if lang is english

            foreach (var englishLangAliases in Translate.englishDict.Keys) {
                if (lang == englishLangAliases) {
                    Translate.toEnglish(text);
                    return;
                }
            }

            //* if lang is not english, then use toOtherLang()

            foreach (var langAliases in Translate.languages.Keys) {
                if (langAliases.Contains(lang)) {
                    Translate.toOtherLang(Translate.languages[langAliases], text);
                    break;
                }
            }
        }

        public static void toEnglish(string text) {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                "cmd", $"/c start https://translate.google.com/?sl=auto^&tl=en^&text={text}^&op=translate"
            ) { CreateNoWindow = true });
        }

        public static void toOtherLang(string lang, string text) {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                "cmd", $"/c start https://translate.google.com/?sl=en^&tl={lang}^&text={text}^&op=translate"
            ) { CreateNoWindow = true });
        }
    }
}
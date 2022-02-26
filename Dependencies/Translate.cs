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
        public static Dictionary<string, Action<string>> english_dict = new() {
            { "toenglish", toEnglish },
            { "english", toEnglish },
            { "e", toEnglish }
        };
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
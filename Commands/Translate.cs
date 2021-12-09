using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace utilities_cs {
    public class Translate {

        static Dictionary<HashSet<string>, string> languages = new() {
            { new() { "tofrench", "french", "f" }, "fr" },
            { new() { "toarabic", "arabic", "a" }, "ar" },
            { new() { "tospanish", "spanish", "s" }, "es" },
            { new() { "todutch", "dutch", "d" }, "nl" },
            { new() { "tochinese", "chinese", "c" }, "zh-TW" },
            { new() { "tojapanese", "japanese", "j" }, "ja" }
        };

        static Dictionary<string, Action<string>> english_dict = new() {
            { "toenglish", toEnglish },
            { "english", toEnglish },
            { "e", toEnglish }
        };
        static void toEnglish(string text) {
            Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://translate.google.com/?sl=auto^&tl=en^&text={text}^&op=translate"
            ) { CreateNoWindow = true });
        }

        static void toOtherLang(string lang, string text) {
            Process.Start(new ProcessStartInfo(
                "cmd", $"/c start https://translate.google.com/?sl=en^&tl={lang}^&text={text}^&op=translate"
            ) { CreateNoWindow = true });
        }

        public static void Translator(string[] args) {
            string lang = args[1];
            string text = string.Join('+', args[2..]);

            // checking if lang is english

            foreach (var englishLangAliases in english_dict.Keys) {
                if (lang == englishLangAliases) {
                    toEnglish(text);
                    return;
                }
            }

            // if lang is not english, then use toOtherLang()

            foreach (var langAliases in languages.Keys) {
                Utils.Notification("ok", languages[langAliases], 4);
                if (langAliases.Contains(lang)) {
                    toOtherLang(languages[langAliases], text);
                    break;
                }
            }
        }
    }
}
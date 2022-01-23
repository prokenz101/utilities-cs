using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace utilities_cs {
    public class Reminder {
        public async static void Remind(string[] args) {
            string text = string.Join(' ', args[1..]);

            List<Dictionary<Match, GroupCollection>>? list_of_dicts = Utils.RegexFind(
                text,
                @"(?<time>\d+)(?<unit>h|m|s)(?<text> .*)?",
                useIsMatch: true,
                () => {
                    Utils.NotifCheck(true, new string[] { "Huh.", "It seems the parameters were not given properly.", "3" });
                }
            );

            if (list_of_dicts != null) {

                List<int> time_enumerable = new();
                List<char> unit_enumerable = new();
                List<string> text_enumerable = new();

                foreach (Dictionary<Match, GroupCollection> dict in list_of_dicts) {
                    foreach (KeyValuePair<Match, GroupCollection> kvp in dict) {
                        time_enumerable.Add(int.Parse(kvp.Value["time"].ToString())); // float
                        unit_enumerable.Add(kvp.Value["unit"].ToString().ToCharArray()[0]); // char
                        text_enumerable.Add(kvp.Value["text"].ToString()); // string
                    }
                }

                int time = time_enumerable[0];
                char unit = unit_enumerable[0];
                string reminder_text = text_enumerable[0];

                Dictionary<char, string[]> time_options = new() {
                    { 's', new string[] { "1", "second" } },
                    { 'm', new string[] { "60", "minute" } },
                    { 'h', new string[] { "3600", "hour" } }
                };

                await Task.Run(() => { // Task for reminder.
                    if (time_options.ContainsKey(unit)) {
                        int multiplier = int.Parse(time_options[unit][0]);
                        string word = time_options[unit][1].ToString();
                        int time_seconds = (time * 1000) * multiplier;

                        Task.Delay(time_seconds).Wait();

                        if (time == 1 && reminder_text == string.Empty) {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Reminder!",
                                    $"Hey! You set a reminder for 1 {word} and it's time!",
                                    "6"
                                }
                            );
                            Console.WriteLine($"Reminder! Hey! You set a reminder for 1 {word} and it's time! 6");
                        } else if (reminder_text == string.Empty) {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Reminder!",
                                    $"Hey! You set a reminder for {time} {word}s and it's time!",
                                    "6"
                                }
                            );
                            Console.WriteLine($"Reminder! Hey! You set a reminder for {time} {word}s and it's time! 6");
                        } else {
                            Utils.NotifCheck(
                                true,
                                new string[] {
                                    "Reminder!",
                                    $"Hey! Your reminder was: {reminder_text}",
                                    "6"
                                }
                            );
                            Console.WriteLine("Reminder! Hey! Your reminder was: {reminder_text} 6");
                        }
                    }
                }
                );
            }
        }
    }
}
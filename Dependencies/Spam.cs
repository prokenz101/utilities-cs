using Microsoft.Toolkit.Uwp.Notifications;

namespace utilities_cs {
    public class Spam {
        public static List<object> data = new List<object>();
        static CancellationTokenSource spamTokenSource = new CancellationTokenSource();
        static CancellationToken spamToken = spamTokenSource.Token;

        public static void SpamMain(string[] args) {
            /*
            ! Variables for Spam
            * Text (string)
            * Count (int)
            * Interval (int)
            * Typing Speed (int)
            * Press Enter? (bool)
            */

            ToastContentBuilder starter = new ToastContentBuilder() //* The first spam config toast
                .AddText("Spam Config")
                .AddText("This will be a series of notifications to collect the settings for your spambot.")
                .AddButton(new ToastButton()
                    .SetContent("Continue")
                    .AddArgument("spam", "init")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            starter.Show(toast => toast.Tag = "spam");
        }

        public static void HandleOnActivatedToast(string value, List<KeyValuePair<string, object>>? userInput) {
            ToastContentBuilder getText = new ToastContentBuilder()
                //* Toast for getting the text to spam

                .AddText("Spam Config 1/5")
                .AddText("What is the text that you would like to spam?")
                .AddInputTextBox("textInputBox", "Type any text here...")
                .AddButton(new ToastButton()
                    .SetContent("Continue")
                    .AddArgument("spam", "getTextContinue")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            ToastContentBuilder getCount = new ToastContentBuilder()
                //* Toast to get the number of times to spam (count: int)

                .AddText("Spam Config 2/5")
                .AddText("How many times do you want to spam?")
                .AddInputTextBox("countInputBox", "Type a number, or \"Infinite\".")
                .AddButton(new ToastButton()
                    .SetContent("Continue")
                    .AddArgument("spam", "getCountContinue")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            ToastContentBuilder getInterval = new ToastContentBuilder()
                //* Toast to get the interval between each spam (interval: int)

                .AddText("Spam Config 3/5")
                .AddText(@"How many seconds do you want to wait between each spam?
This will be the delay in between each spam.")
                .AddInputTextBox("intervalInputBox", "You can also use \"0\" for no interval.", "Type a number (in milliseconds)...")
                .AddButton(new ToastButton()
                    .SetContent("Continue")
                    .AddArgument("spam", "getIntervalContinue")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            ToastContentBuilder getTypingSpeed = new ToastContentBuilder()
                //* Toast to get the typing speed, or the delay in between each key press (typingSpeed: int)

                .AddText("Spam Config 4/5")
                .AddText(@"How fast do you want to type?
This will be the delay in milliseconds in between each key press.")
                .AddInputTextBox(
                    "typingSpeedInputBox",
                    "You can also use \"0\" for very fast typing speed.",
                    "Type a number (in milliseconds)..."
                )

                .AddButton(new ToastButton()
                    .SetContent("Continue")
                    .AddArgument("spam", "getTypingSpeedContinue")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            ToastContentBuilder getPressEnter = new ToastContentBuilder()
                //* Toast to get whether or not to press enter after each spam (pressEnter: bool)

                .AddText("Spam Config 5/5")
                .AddText("Do you want to press enter after each spam?")

                .AddButton(new ToastButton()
                    .SetContent("Yes")
                    .AddArgument("spam", "getPressEnterYes")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("No")
                    .AddArgument("spam", "getPressEnterNo")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            ToastContentBuilder startSpamming = new ToastContentBuilder()
                //* Final toast which asks the user to start spamming

                .AddText("Start Spamming?")
                .AddText("Would you like to start spamming now?")

                .AddButton(new ToastButton()
                    .SetContent("Yes")
                    .AddArgument("spam", "startSpammingYes")
                    .SetBackgroundActivation()
                )

                .AddButton(new ToastButton()
                    .SetContent("Cancel")
                    .AddArgument("spam", "cancel")
                    .SetBackgroundActivation()
                );

            /*
                ! Data Format
                * data[0] = text (string)
                * data[1] = count (int)
                * data[2] = interval (int)
                * data[3] = typingSpeed (int)
                * data[4] = pressEnter (bool)
            */

            Action formatExceptionNotification = () => {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Huh.", "Perhaps the number you inputted was not a number.", "3"
                    }, "spamError"
                );
            };

            switch (value) {
                case "init":
                    getText.Show(toast => toast.Tag = "spam");
                    break;

                case "cancel":
                    ToastNotificationManagerCompat.History.Remove("spam");
                    data.Clear();
                    break;

                case "getTextContinue":
                    if (userInput != null && userInput.Count > 0) {
                        if (userInput[0].Value.ToString() != null) {
                            data.Add(userInput[0].Value.ToString()!);
                        }
                    }

                    getCount.Show(toast => toast.Tag = "spam");
                    break;

                case "getCountContinue":
                    try {
                        if (userInput != null && userInput.Count > 0) {
                            if (userInput[0].Value.ToString()!.ToLower() == "infinite") {
                                data.Add(int.MaxValue);
                            } else {
                                data.Add(int.Parse(userInput[0].Value.ToString()!));
                            }
                        }

                        getInterval.Show(toast => toast.Tag = "spam");
                        break;
                    } catch (FormatException) { formatExceptionNotification(); break; }

                case "getIntervalContinue":
                    try {
                        if (userInput != null && userInput.Count > 0) {
                            data.Add(int.Parse(userInput[0].Value.ToString()!));
                        }

                        getTypingSpeed.Show(toast => toast.Tag = "spam");
                        break;
                    } catch (FormatException) { formatExceptionNotification(); break; }

                case "getTypingSpeedContinue":
                    try {
                        if (userInput != null && userInput.Count > 0) {
                            data.Add(int.Parse(userInput[0].Value.ToString()!));
                        }

                        getPressEnter.Show(toast => toast.Tag = "spam");
                        break;
                    } catch (FormatException) { formatExceptionNotification(); break; }

                case "getPressEnterYes":
                    data.Add(true);
                    startSpamming.Show(toast => toast.Tag = "spam");
                    break;

                case "getPressEnterNo":
                    data.Add(false);
                    startSpamming.Show(toast => toast.Tag = "spam");
                    break;

                case "startSpammingYes":
                    Thread.Sleep(2000);

                    try {
                        string text = data[0].ToString()!;
                        int count = int.Parse(data[1].ToString()!);
                        int interval = int.Parse(data[2].ToString()!);
                        int typingSpeed = int.Parse(data[3].ToString()!);
                        bool pressEnter = bool.Parse(data[4].ToString()!);

                        PerformSpam(text, count, interval, typingSpeed, pressEnter);
                        break;

                    } catch (NullReferenceException) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Something went wrong.",
                                "An exception occured while trying to start spamming.",
                                "4"
                            }, "spamError"
                        ); break;
                    }

                case "stopSpam":
                    HookManager.UnregisterHook("spam");
                    spamTokenSource.Cancel();
                    spamTokenSource.Dispose();
                    break;
            }
        }

        public static async void PerformSpam(string text, int count, int interval, int typingSpeed, bool pressEnter) {
            Task spamTask = new Task(
                () => {
                    try {
                        Action performSpam = () => {
                            foreach (char x in text) {
                                SendKeys.SendWait(x.ToString());
                                spamToken.ThrowIfCancellationRequested();
                                Thread.Sleep(typingSpeed);
                                spamToken.ThrowIfCancellationRequested();
                            }

                            if (pressEnter) { SendKeys.SendWait("{ENTER}"); }
                            spamToken.ThrowIfCancellationRequested();
                            Thread.Sleep(interval);
                            spamToken.ThrowIfCancellationRequested();
                        };

                        if (count != int.MaxValue) {
                            //* If the count is not infinite, spam the text for the specified number of times
                            for (int i = 0; i < count; i++) { performSpam(); }
                        } else {
                            //* Infinite spamming
                            while (true) { performSpam(); }
                        }

                        throw new OperationCanceledException();
                    } catch (OperationCanceledException) {
                        HookManager.UnregisterHook("spam");
                        ToastNotificationManagerCompat.History.Remove("stopSpam");
                        Utils.NotifCheck(
                            true,
                            new string[] { "Stopped Spam.", "The spammer was stopped.", "3" },
                            "spamComplete"
                        );
                        return;
                    } catch {
                        HookManager.UnregisterHook("spam");
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "Huh.", "Something went wrong while spamming.", "3"
                            }, "spamError"
                        ); return;
                    }
                }, spamToken
            );

            bool errored = false;

            HookManager.AddHook(
                "spam",
                new ModifierKeys[] { ModifierKeys.Control },
                Keys.F7,
                onPressed: () => {
                    spamTokenSource.Cancel();
                    spamTokenSource.Dispose();
                    HookManager.UnregisterHook("spam");
                },
                onFail: () => {
                    errored = true;
                    Utils.NotifCheck(
                        true,
                        new string[] { "Huh.", "Perhaps you already have a spammer running.", "3" },
                        "spamError"
                    );
                }
            );

            if (errored) { return; }

            ToastContentBuilder stopSpamToast = new ToastContentBuilder() //* Toast to stop the spammer
                .AddText("Stop the spammer?")
                .AddText(@"Clicking on the Stop button will stop the spammer.
You can also press Ctrl + F7 but the hotkey may not register every time.")
                .SetToastScenario(ToastScenario.Reminder)

                .AddButton(new ToastButton()
                    .SetContent("Stop")
                    .AddArgument("spam", "stopSpam")
                    .SetBackgroundActivation()
                );

            await Task.Run(() => stopSpamToast.Show(toast => toast.Tag = "stopSpam"));

            spamTask.Start();
        }
    }
}
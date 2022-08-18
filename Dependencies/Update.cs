using Octokit;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;

namespace utilities_cs {
    public class Update {
        public static GitHubClient client = new GitHubClient(new ProductHeaderValue("utilities-cs"));

        public static void Check(bool alertEvenIfUpdateIsNotRequired = true) {
            try {
                var currentVersion = Program.Version[3..];

                Task<Release>? latestRelease = client.Repository.Release.GetLatest("prokenz101", "utilities-cs");

                string? latestVersion;
                try {
                    latestVersion = latestRelease.Result.TagName[3..];
                } catch { throw new ApiException(); }

                if (double.Parse(latestVersion) > double.Parse(currentVersion)) {
                    ToastContentBuilder toast = new ToastContentBuilder()
                        .AddText("There is a new version of utilities-cs available!")
                        .AddText($@"Your version: v1.{currentVersion}
Latest version: v1.{latestVersion}")

                        .AddButton(
                            new ToastButton()
                                .SetContent("Update")
                                .AddArgument("update", "update")
                                .SetBackgroundActivation()
                        );

                    if (alertEvenIfUpdateIsNotRequired) {
                        toast.AddButton(
                            new ToastButton().SetContent("Cancel").AddArgument("update", "cancel").SetBackgroundActivation()
                        );
                    } else {
                        toast.AddButton(
                            new ToastButton().SetContent("Later").AddArgument("update", "later").SetBackgroundActivation()
                        );
                    }

                    Utils.NotifCheck(toast, "updateInstall", clearToast: false, 4);

                } else if (double.Parse(latestVersion) == double.Parse(currentVersion)) {
                    if (alertEvenIfUpdateIsNotRequired) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "You are running the latest version of utilities-cs!",
                                "There is nothing to update.",
                                "4"
                            }, "updateUpToDate"
                        );
                    }
                } else {
                    if (alertEvenIfUpdateIsNotRequired) {
                        Utils.NotifCheck(
                            true,
                            new string[] {
                                "You are in the future.",
                                "Your utilities-cs version is above the latest one on GitHub.",
                                "4"
                            }, "updateFuture"
                        );
                    }
                }
            } catch (ApiException) {
                if (alertEvenIfUpdateIsNotRequired) {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Unable to get information from the server.",
                            "This (could) be because you are not connected to the internet.",
                            "4"
                        }, "updateAPIError"
                    );
                }
            } catch {
                if (alertEvenIfUpdateIsNotRequired) {
                    Utils.NotifCheck(
                        true,
                        new string[] {
                            "Something went wrong.",
                            "An exception occured whilst trying to check for updates.",
                            "4"
                        }, "updateError"
                    );
                }
            }
        }

        public static void InstallLatestVersion() {
            try {
                var latestRelease = client.Repository.Release.GetLatest("prokenz101", "utilities-cs");
                var assets = latestRelease.Result.Assets;

                string downloadURL = "";

                foreach (var x in assets) {
                    if (x.Name.Contains("fd") && Program.buildMode == Program.BuildMode.FrameworkDependent) {
                        downloadURL = x.BrowserDownloadUrl; //* framework-dependent
                        break;
                    } else if (x.Name.Contains("sc") && Program.buildMode == Program.BuildMode.SelfContained) {
                        downloadURL = x.BrowserDownloadUrl; //* self-contained
                        break;
                    }
                }

                Process.Start(new ProcessStartInfo("cmd", $"/c start {downloadURL}") { CreateNoWindow = true });
            } catch (ApiException) {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Unable to get information from the server.",
                        "This (could) be because you are not connected to the internet.",
                        "4"
                    }, "updateAPIError"
                );
            } catch {
                Utils.NotifCheck(
                    true,
                    new string[] {
                        "Something went wrong.",
                        "An exception occured whilst trying to check for updates.",
                        "4"
                    }, "updateError"
                );
            }
        }

        public static void HandleOnActivatedToast(string value) {
            switch (value) {
                case "update":
                    ToastNotificationManagerCompat.History.Remove("updateInstall");
                    Utils.NotifCheck(
                        true,
                        new string[] {
                                "Installing the latest version.",
                                "Opening download link in your browser.",
                                "3"
                        }, "updateInstalling"
                    );

                    Update.InstallLatestVersion(); break;

                case "cancel":
                    ToastNotificationManagerCompat.History.Remove("updateInstall"); break;

                case "later":
                    ToastNotificationManagerCompat.History.Remove("updateInstall"); break;
            }
        }
    }
}
using Microsoft.Toolkit.Uwp.Notifications;

namespace jklm_hacks {
    class Program {
        public static NotifyIcon trayIcon = new() { Text = "jklm-hacks" };

        /// <summary>
        /// The entry point of the program.
        /// </summary>
        /// <param name="args">Command-Line arguments.</param>

        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            ContextMenuStrip contextMenuStrip = new();

            contextMenuStrip.Items.Add(
                "Exit", null, delegate { Exit(); }
            );

            trayIcon.Icon = Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!);
            trayIcon.ContextMenuStrip = contextMenuStrip;
            trayIcon.Visible = true;

            Application.ApplicationExit += delegate { Exit(); };

            HookManager.AddHook(
                "jklm",
                new[] { ModifierKeys.Control },
                Keys.F7,
                () => {
                    SendKeys.SendWait("^a");
                    Thread.Sleep(150);
                    SendKeys.SendWait("^c");
                    Thread.Sleep(50);
                    string clipboard = Clipboard.GetText();
                    jklmRun(clipboard);
                },
                () => {
                    Notification("Something went wrong.", @"Are you trying to open this program multiple times?", 2);
                    Exit();
                }
            );

            Notification("Opened jklm-hacks.", "I am now in your system tray, right click and press Exit to close.", 2);

            Application.Run();
        }

        public static void Exit() {
            HookManager.UnregisterAllHooks();
            trayIcon.Visible = false;
            Application.Exit();
        }

        public async static void Notification(string title, string subtitle, int toastexpirationtime = 1) {
            await Task.Run(() => {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(subtitle)
                    .Show();

                Task.Delay((toastexpirationtime + 1) * 1000).Wait();
                ToastNotificationManagerCompat.History.Clear();
            });
        }

        public static void jklmRun(string clipboard) {
            string input = clipboard.ToLower();

            // check if input is there in any of the Words lists
            if (Words.AllWords.Any(x => x.Contains(input))) {
                List<string> words = Words.AllWords.FindAll(x => x.Contains(input));
                
                // take 10 RANDOM from "words"
                List<string> randomWords = new List<string>();
                for (int i = 0; i < 10; i++) {
                    randomWords.Add(words[new Random().Next(0, words.Count)]);
                }

                // get the longest word from "randomWords" and call the variable "word"
                string word = randomWords.OrderByDescending(x => x.Length).First();

                words.Remove(word);

                SendKeys.SendWait(word);
                SendKeys.SendWait("{ENTER}");
            } else { Notification("Couldn't find a word.", "Sorry"); }
        }
    }
}

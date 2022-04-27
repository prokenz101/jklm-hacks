using Microsoft.Toolkit.Uwp.Notifications;

namespace jklm_hacks {
    class Program {
        public static NotifyIcon trayIcon = new() { Text = "jklm-hacks" };

        /// <summary>
        /// The entry point of the program.
        /// </summary>
        /// <param name="args"></param>

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
                    Thread.Sleep(50);
                    SendKeys.SendWait("^c");
                    Thread.Sleep(50);
                    SendKeys.Send("{ESC}");
                    Thread.Sleep(50);
                    string clipboard = Clipboard.GetText();
                    jklmRun(clipboard);
                },
                () => {
                    Notification("Something went wrong.", @"Are you trying to open this program multiple times?
if not then RIP bozo", 2);
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
            Func<string, ToastButton> createButton = (string num) => {
                return new ToastButton()
                    .SetContent(num)
                    .AddArgument(num, int.Parse(num) - 1)
                    .SetBackgroundActivation();
            };
            
            try {
                string input = clipboard;

                // check if input is there in any of the Words lists
                if (
                    Words.AllWords.Any(x => x.Contains(input))
                ) {
                    List<string> words = Words.AllWords.FindAll(x => x.Contains(input));
                    var longestWords = words.OrderByDescending(x => x.Length).Take(4);
                    string wordsString = string.Join("\n", longestWords);

                    if (wordsString != null) {
                        ToastContentBuilder toast =
                            new ToastContentBuilder()
                                .AddText($"Word search for: \"{input}\"")
                                .AddText(wordsString);

                        string[] splitWords = wordsString.Split('\n');

                        if (splitWords.Length >= 1) { toast.AddButton(createButton("1")); }
                        if (splitWords.Length >= 2) { toast.AddButton(createButton("2")); }
                        if (splitWords.Length >= 3) { toast.AddButton(createButton("3")); }
                        if (splitWords.Length >= 4) { toast.AddButton(createButton("4")); }

                        toast.Show();

                        ToastNotificationManagerCompat.OnActivated += toastArgs => {
                            string args = toastArgs.Argument;

                            var x = args.Split("=");

                            string pressedButton = x[0];
                            string value = x[1];

                            Console.WriteLine("pressedButton: " + pressedButton);
                            Console.WriteLine("value: " + value);
                            Console.WriteLine("Thing to copy: " + splitWords[int.Parse(value)]);
                            
                            try {
                                Thread thread = new Thread(() => Clipboard.SetText(splitWords[int.Parse(value)]));
                                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                                thread.Start(); 
                                thread.Join();
                            } catch (Exception e) {
                                // print the exception
                                Console.WriteLine("Exception: " + e);
                            }
                        };

                    } else {
                        Notification("Cannot find word", "Sorry we couldnt find that word you know");
                    }
                } else {
                    Notification("Couldn't find a word.", "Sorry");
                }
            } catch (ArgumentOutOfRangeException) {
                Notification("Bro", "You need to input something you know");
            }
        }
    }
}
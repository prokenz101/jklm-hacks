using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace jklm_hacks {
    public class Program {
        public static TcpListener Server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8005);
        public static bool ContinueExecution = true;

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args) {
            //* Creating the python file
            File.WriteAllText("jklm-hacks-sockets.py", @"from time import sleep
from os import system
from json import loads
from socket import socket
from pynput import keyboard
from pyperclip import paste
from pyautogui import typewrite

ws = socket()
port = 8005
ip = ""127.0.0.1""
ws.connect((ip, port))
controller = keyboard.Controller()


def notification(title, subtitle):
    system(f'notify-send ""{title}"" ""{subtitle}""')


def on_release(key):
    if key == keyboard.Key.f9:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed(""a""):
                sleep(50 / 1000)
            with controller.pressed(""c""):
                pass

        send_receive_data(paste())


def send_receive_data(data):
    ws.send(data.encode(""UTF-8""))
    packets = []
    while True:
        packet = ws.recv(1024)
        packets.append(packet.decode(""UTF-8""))
        if len(packet) < 1024:
            break

    data = """".join(packets)
    if data == ""Something went wrong."":
        print(""Couldn't find a word."")
        return

    else:
        print(f""Python: Received {data}"")
        typewrite(data + ""\n"", interval=0.01)


def main():
    notification(""Hello"", ""How are you"")

    with keyboard.Listener(on_release = on_release) as listener:
        listener.join()


if __name__ == ""__main__"":
    main()
");
            Process.Start(new ProcessStartInfo("python3", "jklm-hacks-sockets.py") { CreateNoWindow = true });
            Console.WriteLine("Python started");
            Task.WaitAll(socketListener());
        }

        static async Task socketListener(string? streamRequest = null) {
            //* creating server on localhost
            Server.Start();

            var client = Server.AcceptSocket(); //! This represents python.
            Console.WriteLine("C# connected");

            byte[] buffer = new byte[1024];
            int i; //* this will be the length of the byte array read from the stream
            NetworkStream stream = new NetworkStream(client);

            while (ContinueExecution) {
                string data = "";
                i = await stream.ReadAsync(buffer, 0, buffer.Length);
                // System.Console.WriteLine("read something");

                data = Encoding.UTF8.GetString(buffer, 0, i);
                buffer = new byte[1024]; //* flushing buffer

                if (data != "") {
                    Console.WriteLine($"C#: Received {data}");

                    string? result = Handler(data).Result;
                    if (result != null) {
                        byte[] msg = Encoding.UTF8.GetBytes(result);
                        // if (msg.Length > 1024) { msg = msg[0..1023]; }

                        await stream.WriteAsync(msg, 0, msg.Length);
                    }
                }
            }

            Server.Stop();
        }

        static async Task<string?> Handler(string clipboard) {
            return await Task.Run(
                () => {
                    string input = clipboard.ToLower();

                    if (Words.AllWords.Any(x => x.Contains(input))) {
                        List<string> words = Words.AllWords.FindAll(x => x.Contains(input));

                        //* take 10 random words from the dictionary
                        List<string> randomWords = new List<string>();
                        for (int i = 0; i < 10; i++) {
                            randomWords.Add(words[new Random().Next(0, words.Count)]);
                        }

                        string word = randomWords.OrderByDescending(x => x.Length).Last();
                        Words.AllWords.Remove(word); //* Removing the word from the list so that it is not repeated

                        //* Return the longest word
                        return word;
                    } else {
                        return "Something went wrong.";
                    }
                }
            );
        }
    }
}
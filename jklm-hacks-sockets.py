from time import sleep
from os import system
from json import loads
from socket import socket
from pynput import keyboard
from pyperclip import paste
from pyautogui import typewrite

ws = socket()
port = 8005
ip = "127.0.0.1"
ws.connect((ip, port))
controller = keyboard.Controller()


def notification(title, subtitle):
    system(f'notify-send "{title}" "{subtitle}"')


def on_release(key):
    if key == keyboard.Key.f9:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed("a"):
                sleep(50 / 1000)
            with controller.pressed("c"):
                pass

        send_receive_data(paste())


def send_receive_data(data):
    ws.send(data.encode("UTF-8"))
    packets = []
    while True:
        packet = ws.recv(1024)
        packets.append(packet.decode("UTF-8"))
        if len(packet) < 1024:
            break

    data = "".join(packets)
    if data == "Something went wrong.":
        print("Couldn't find a word.")
        return

    else:
        print(f"Python: Received {data}")
        typewrite(data + "\n", interval=0.01)


def main():
    notification("Hello", "How are you")

    with keyboard.Listener(on_release = on_release) as listener:
        listener.join()


if __name__ == "__main__":
    main()

from time import sleep
from pynput import keyboard
from pyautogui import typewrite, hotkey
from pyperclip import paste
from pathlib import Path
from random import sample

# * F9 keyboard hotkey

words = Path("dictionary").read_text().split("\n")


def on_press(key):
    try:
        key.char

    except AttributeError:
        if str(key) == "Key.f9":

            sleep(0.2)
            hotkey("ctrl", "a")
            sleep(0.2)
            hotkey("ctrl", "c")

            sleep(0.2)
            clipboard = paste()
            print(f"Prompt: {clipboard}")

            valid_words = list(filter(lambda x: clipboard in x, words))
            random_words = None
            if len(valid_words) == 0:
                print("No word found!")
                print()
                return

            elif len(valid_words) >= 10:
                random_words = sample(valid_words, 10)

            else:
                random_words = sample(valid_words, len(valid_words))

            result = max(random_words)

            print(f"Result: {result}")
            typewrite(result, interval=0.02)
            print()


with keyboard.Listener(on_press=on_press) as listener:
    listener.join()

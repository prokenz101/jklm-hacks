# jklm-hacks

### This is a hack program written for the game [JKLM](https://jklm.fun), but written in python.

### Intended only for use on Linux, if you're a Windows user please install [this](https://github.com/prokenz101/jklm-hacks).

### OS X? Too bad.

<br />

## Install

You must have [git](https://git-scm.com/) installed.

Run the following command through terminal in any directory.
```bash
git clone https://github.com/prokenz101/jklm-hacks/tree/python
```

<br />

## Getting dependencies

This program requires three pip packages and a utility called [xsel](https://github.com/kfish/xsel).

### Installing pip packages and xsel:
Simply run this command in terminal, and make sure your working directory is inside the cloned folder.
(Requires root permissions)
```bash
pip install -r requirements.txt ; sudo apt-get install xsel
```

<br />

## Use 

Run the file using `python3 /main.py`.

It should show "JKLM-HACKS" and a few instructions on how to use and exit the program.

### To start hacking:
In your game of JKLM, type out the prompt that you just recieved into the input box.

So if you recieved "ER" as a prompt (just an example), type out ER and then **press F9** or **Fn + F9** on some keyboards.

After pressing F9, if all goes well, the program should automatically type out a valid word for you.

If the program just removes your prompt and doesn't type anything, it means no valid word was found.

<br />

## Uninstall

Just delete the folder.

If you want to delete the pip packages and xsel, run this command.

```bash
pip uninstall pyautogui ; pip uninstall pyperclip ; pip uninstall pynput ; sudo apt-get remove xsel
```

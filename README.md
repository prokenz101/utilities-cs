# utilities-cs
A version of [utilities-py](https://github.com/prokenz101/utilities-py) written in C#.
 
## Installation:
Check the [releases](https://github.com/prokenz101/utilities-cs/releases) and download the latest release based on self-contained and framework-dependent. If you do not know what those mean, then read the below text.
 
### Meaning of self-contained and framework-dependent:
#### Self-Contained means that the `utilities-cs.exe` will not require `dotnet` to run. If you do not have dotnet, you must use the self-contained version.

How to check if you have dotnet or not:

Open `cmd` in Windows search and type "dotnet". If it says the following, then you have to use the self-contained version. 
```bash
'dotnet' is not recognized as an internal or external command,
operable program or batch file.
```

If it does not say the above text, then you may use the framework dependent version.

Unfortunately, the self-contained version will have a tremendously large file size and there is nothing I can do about this.

#### Framework-dependent means that the file depends on `dotnet` to run. If you have `dotnet`, then this is definitely the version to install.

File size for this version is generally around 600KB, much better than the ~150MB of the self-contained version.

## Use:
Run `utilities-cs.exe`. It will not open any sort of window, but it should sit in your system-tray when running.

**Do not** run `utilities-cs.exe` multiple times, I have added a failsafe for this, but the Ctrl F8 hotkey will bug out if there are multiple utilities-cs files running.

Using this is very similar to the [python version](https://github.com/prokenz101/utilities-py) of utilities, only a change in the hotkey.

Simply open [PowerToys Run](https://github.com/microsoft/powertoys) or any place when you can type text, type your command and press Ctrl + F8. If you don't know any commands then try `cursive hello world` as a test, then read the [wiki](https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)) for more info.

If you are confused on what just happened when you ran a command, try reading the toast notification that follows. If there is no toast notification then something has probably gone wrong.

## Help
If you are confused on how to install/use utilities-cs, or have trouble understanding how a command works, you may [contact me on discord](https://github.com/prokenz101/utilities-py/wiki/Utilities-Wiki-(Windows,-C%23-and-Python)#got-any-doubts).

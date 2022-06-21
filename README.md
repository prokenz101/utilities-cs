# utilities-cs
Utilities is a program that has several uses, most of them may not appeal to you for this is mostly used by me. There are commands like `cursive` that can convert text to cursive, commands like `commaseperator` that adds commas in between numbers like 487377 ⭢ 487,377 etc and several others. Using utilities is very memory based, you must remember name and syntax of a command (although there is a [Wiki](https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki) incase you needed it).

**Remember, this is intended for windows only. Go away OS X users, and sorry Linux users.**

## Installation
Stable builds can be found in the [releases](https://github.com/prokenz101/utilities-cs/releases) but if you want to download the very latest version, you can go to the [Github Actions](https://github.com/prokenz101/utilities-cs/actions) of this repository, more on that at the end. There are two versions of utilities-cs, they are `utilities-cs-sc.exe` and `utilities-cs-fd.exe`. Read the below paragraphs to understand which one to download.

It is highly recommended to download [Microsoft PowerToys](https://github.com/microsoft/powertoys/releases), as utilities-cs was basically built to be used along-side a feature in it, called PowerToys Run, and the PowerToys installer will also automatically handle the installation of dotnet, if you do not have it already. (Scroll down to the bottom of the latest release and hit `PowerToys Setup-1234-x64.exe`, then run the installer to download.) If you get powertoys, then you can ignore the rest of the paragraphs below and just download the `utilities-cs-fd.exe` and go to Using.

**If you don't want to download PowerToys for whatever reason:**
If you don't want to download PowerToys (not recommended) then just make sure you have dotnet, you can run the command `dotnet --list-runtimes` in Command Prompt. If it says "Microsoft.NetCoreApp 6.0" ANYWHERE on the screen then you most likely have dotnet.

If it says "dotnet is not recognized as an internal or external command, operable program or batch file.", then you do not have dotnet at all and you will have to [install it](https://dotnet.microsoft.com/en-us/download). 

If you just don't want to download anything at all for utilities-cs, then you can get the `self-contained` version. I wouldn't suggest this, because using utilities-cs without PowerToys already kind of sucks AND the self-contained version is almost 200 MB in size. Just get .net6 and use framework-dependent, its better.

### Getting the latest version from GitHub Actions:
The releases are not always the quickest for getting new versions of utilities-cs. If you want to test new commands you can check out the GitHub Actions which automatically compiles the latest commit of utilities-cs into a .exe for you. Just download the ZIP file, then extract it to get access to the executable. This .exe is not completely stable and will open up an empty console window, which you can just ignore. If you have Visual Studio then you can run `editbin /subsytem:windows .\utilities-cs-fd.exe` or self-contained.exe to get rid of this console window. Now, you can use the latest version of utilities-cs, even newer than the newest release!

## Using
Open PowerToys, and then type any command that utilities-cs will recognise and press Ctrl + F8 to trigger. If you don't know any commands, then just try `cursive hello world` and Ctrl + F8 as a test and then check the [wiki](https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki) to understand how to use utilities-cs.

If you didn't want to install PowerToys, then open the windows search or and place where you can type text and then type your command, then press Ctrl + F8.

## Help
If you are confused on how to install/use utilities-cs, or have trouble understanding how a command works, you may [view the wiki](https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki) or [contact me on discord](https://github.com/prokenz101/utilities-cs/wiki/Utilities-Wiki#got-any-doubts).

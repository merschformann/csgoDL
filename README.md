# csgoDL
This is a small graphical user interface to launch dedicated Counter-Strike: Global Offensive servers more easily (from Windows). ([Download](https://github.com/merschformann/csgoDL/releases/tag/0.5))
I wrote this code a while ago myself and did not really care about efficiency and structure too much. Hence, please be nice. ;)

## Current status ##
It serves my own requirements, but I wanted to share it in case someone else is looking for a simple and quick way to start a dedicated server.

## Preliminaries ##
- Download & install CS:GO dedicated servers before using the GUI. Simply follow the intructions at this link: [https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Dedicated_Servers#Downloading_the_CS:GO_Dedicated_Server](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Dedicated_Servers#Downloading_the_CS:GO_Dedicated_Server)
- Microsoft .Net framework 4.6 is required
- For developers: MS Visual Studio in an up-to-date version should be fine (e.g. the Community Edition)

## How to use ##
Either use the pre-compiled .exe in the attached .zip-file or build it yourself.
Building should be very straightforward since it is a very small project.

The GUI itself offers some basic configuration of a CS:GO dedicated server.
Quick guide:
- Start "csgoDL.exe"
- Set the path to the directory containing the SRCDS.exe under basic settings (see preliminaries / usually in "<steamcmdpath>\steamapps\common\Counter-Strike Global Offensive Beta - Dedicated Server")
- For an internet game specify your GSLT account
- Set server PW (if desired) and RCON PW for dropping admin commands remotely
- Select the type of the game
- For a game using weapon progression you can specify the weapons in the corresponding tabs (rows can be deleted by pressing "del" and added by pressing "enter" with the last line focused)
- Any additional server commands can be specified in CFG
- Click on the play button to start the server
- Stopping the server currently only works either by classically typing "exit" or by clicking the stop button that simply kills the server's process

As you can see the application is really raw as it only writes out the corresponding gamemode_server.txt and gamemode_*_server.cfg files and starts the server by calling the SRCDS.exe with the respective arguments.
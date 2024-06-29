# Arcemi Save Game Editor
A save game editor for the games
- Pathfinder - Kingmaker
- Pathfinder - Wrath of the Righteous
- Warhammer 40000 Rogue Trader

Compiled files are available at https://www.nexusmods.com/warhammer40kroguetrader/mods/40/?tab=posts

# Structure
- Arcemi.Models is a library project.
- Arcemi.SaveGameEditor is the new project for the new editor using Electron.NET in an attempt to make it cross platform

# Credits

Electron.NET https://github.com/ElectronNET/Electron.NET

# Start/Build Windows
To start the application from the source code, first install Electron.Net as specified at their project page.
Open a console and set your location to Arcemi.Pathfinder.SaveGameEditor project folder and run:
electronize start

To buid run
electronize build /target win

# Start/Build Linux
To start the application from the source code, first install Electron.Net as specified at their project page.
Open a console and set your location to Arcemi.Pathfinder.SaveGameEditor project folder and run:
electronize start

To buid run
electronize build /target linux /PublishReadyToRun false

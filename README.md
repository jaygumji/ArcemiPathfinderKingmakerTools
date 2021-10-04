# Arcemi Pathfinder Wrath of the Righteous Editor
A save game editor for the game Pathfinder Wrath of the Righteous Editor.

Inventory items are now loaded from the game resources, provided the path to the game installation folder has been configured in the settings.

Compiled files are available at https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/91/ where you can also find other neat mods.

# Structure
- Arcemi.Pathfinder.Kingmaker is a library project.
- Arcemi.Pathfinder.Kingmaker.Editor is an experiment project and can be ignored.
- Arcemi.Pathfinder.Kingmaker.SaveGameEditor is a WPF project which currently hosts the main ui of the old editor, can be ignored.
- Arcemi.Pathfinder.SaveGameEditor is the new project for the new editor using Electron.NET in an attempt to make it cross platform

# Credits

Electron.NET https://github.com/ElectronNET/Electron.NET

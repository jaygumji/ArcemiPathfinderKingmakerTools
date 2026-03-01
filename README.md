# Arcemi Save Game Editor
A save game editor for the games
- Pathfinder - Kingmaker
- Pathfinder - Wrath of the Righteous
- Warhammer 40000 Rogue Trader

Compiled files are available at https://www.nexusmods.com/warhammer40kroguetrader/mods/40/?tab=posts

# Structure
- Arcemi.Models is a library project.
- Arcemi.SaveGameEditor is the desktop editor project using ElectronNET.Core and ASP.NET/Blazor.

# Credits

Electron.NET https://github.com/ElectronNET/Electron.NET

# Prerequisites
- .NET SDK 10
- Node.js 22+

# Start locally
Open a console and set your location to the `Arcemi.SaveGameEditor` project folder, then run:
`dotnet run`

# Build desktop packages
Run these from the `Arcemi.SaveGameEditor` folder:
- Windows: `dotnet publish -c Release /p:ElectronTarget=custom /p:ElectronBuildTarget=win /p:ElectronPlatform=win32 /p:ElectronArch=x64`
- Linux: `dotnet publish -c Release /p:ElectronTarget=custom /p:ElectronBuildTarget=linux /p:ElectronPlatform=linux /p:ElectronArch=x64`
- macOS: `dotnet publish -c Release /p:ElectronTarget=custom /p:ElectronBuildTarget=osx /p:ElectronPlatform=darwin /p:ElectronArch=x64`

Artifacts are generated under `Arcemi.SaveGameEditor/bin/Desktop`.

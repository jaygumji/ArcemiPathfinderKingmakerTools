using Arcemi.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Arcemi.Tests
{
    public class GameFolderDiscoveryTests
    {
        [Fact]
        public void LinuxFindsSteamInstallationAndProtonSaveInSecondaryLibrary()
        {
            using var temporary = new TemporaryFolder();
            var home = temporary.Path;
            var steamApps = Path.Combine(home, ".local", "share", "Steam", "steamapps");
            var library = Path.Combine(home, "Secondary Library");
            Directory.CreateDirectory(steamApps);
            var game = GameDefinition.Warhammer40K_RogueTrader;
            var install = Path.Combine(library, "steamapps", "common", "Custom Rogue Trader directory");
            WriteBlueprintData(install);
            WriteBlueprintData(Path.Combine(steamApps, "common", game.WindowsGameFolderName));

            var escapedLibrary = library.Replace(@"\", @"\\");
            File.WriteAllText(Path.Combine(steamApps, "libraryfolders.vdf"), $$"""
                "libraryfolders"
                {
                    "1" { "path" "{{escapedLibrary}}" }
                }
                """);
            File.WriteAllText(Path.Combine(library, "steamapps", "appmanifest_2186680.acf"), """
                "AppState" { "installdir" "Custom Rogue Trader directory" }
                """);

            var protonData = Path.Combine(library, "steamapps", "compatdata", "2186680", "pfx", "drive_c", "users",
                "steamuser", "AppData", "LocalLow", "Owlcat Games", game.AppDataFolderName);
            var saveFolder = Directory.CreateDirectory(Path.Combine(protonData, "Saved Games"));
            File.WriteAllText(Path.Combine(saveFolder.FullName, "slot.zks"), "");

            var discovery = new GameFolderDiscovery(home, OSPlatform.Linux);
            Assert.Equal(install, discovery.FindGameFolder(game));
            Assert.Equal(protonData, discovery.FindSaveDataFolder(game));
        }

        [Fact]
        public void LinuxPrefersNewerSaveWhenNativeAndProtonPathsBothExist()
        {
            using var temporary = new TemporaryFolder();
            var home = temporary.Path;
            var config = Path.Combine(home, "xdg");
            var game = GameDefinition.Pathfinder_Kingmaker;
            var nativeData = Path.Combine(config, "unity3d", "Owlcat Games", game.AppDataFolderName);
            var protonData = Path.Combine(home, ".steam", "steam", "steamapps", "compatdata", "640820", "pfx",
                "drive_c", "users", "steamuser", "AppData", "LocalLow", "Owlcat Games", game.AppDataFolderName);
            var native = Directory.CreateDirectory(Path.Combine(nativeData, "Saved Games"));
            var proton = Directory.CreateDirectory(Path.Combine(protonData, "Saved Games"));
            var nativeSave = Path.Combine(native.FullName, "native.zks");
            File.WriteAllText(nativeSave, "");
            File.WriteAllText(Path.Combine(proton.FullName, "proton.zks"), "");
            File.SetLastWriteTimeUtc(nativeSave, DateTime.UtcNow.AddDays(-1));

            var discovery = new GameFolderDiscovery(home, OSPlatform.Linux, config);
            Assert.Equal(protonData, discovery.FindSaveDataFolder(game));
            File.SetLastWriteTimeUtc(nativeSave, DateTime.UtcNow.AddDays(1));
            Assert.Equal(nativeData, discovery.FindSaveDataFolder(game));
        }

        [Fact]
        public void MacResolvesAnAppBundleAndDetectsApplicationSupportSaves()
        {
            using var temporary = new TemporaryFolder();
            var game = GameDefinition.Warhammer40K_RogueTrader;
            var support = Path.Combine(temporary.Path, "Library", "Application Support");
            var data = Path.Combine(support, "Owlcat Games", game.AppDataFolderName);
            Directory.CreateDirectory(Path.Combine(data, "Saved Games"));
            var applications = Path.Combine(temporary.Path, "Applications");
            var bundle = Path.Combine(applications, "WH40KRT.app");
            var resources = Path.Combine(bundle, "Contents", "Resources");
            var executable = Path.Combine(Directory.CreateDirectory(Path.Combine(bundle, "Contents", "MacOS")).FullName, "WH40KRT");
            File.WriteAllText(executable, "");
            WriteBlueprintData(resources);

            var discovery = new GameFolderDiscovery(temporary.Path, OSPlatform.OSX);
            Assert.Equal(data, discovery.FindSaveDataFolder(game));
            Assert.Equal(resources, discovery.FindGameFolder(game));
            Assert.Equal(resources, GameFolderDiscovery.ResolveGameFolder(bundle, game));
            Assert.Equal(resources, GameFolderDiscovery.ResolveGameFolder(executable, game));
            Assert.Equal(resources, GameFolderDiscovery.ResolveGameFolder(applications, game));
            Assert.Null(GameFolderDiscovery.ResolveGameFolder(bundle, GameDefinition.NotSet));

            var wrath = Path.Combine(applications, "Wrath.app");
            WriteBlueprintData(wrath);
            Assert.Equal(wrath, discovery.FindGameFolder(GameDefinition.Pathfinder_WrathOfTheRighteous));
        }

        [Fact]
        public void WindowsStillFindsLocalLowAndConventionalSteamInstallation()
        {
            using var temporary = new TemporaryFolder();
            var game = GameDefinition.Warhammer40K_DarkHeresy;
            var steam = Path.Combine(temporary.Path, "Steam");
            var install = Path.Combine(steam, "steamapps", "common", game.WindowsGameFolderName);
            var data = Path.Combine(temporary.Path, "AppData", "LocalLow", "Owlcat Games", game.AppDataFolderName);
            Directory.CreateDirectory(Path.Combine(data, "Saved Games"));
            WriteBlueprintData(install);
            var discovery = new GameFolderDiscovery(temporary.Path, OSPlatform.Windows, additionalSteamRoots: new[] { steam });

            Assert.Equal(data, discovery.FindSaveDataFolder(game));
            Assert.Equal(install, discovery.FindGameFolder(game));
        }

        private static void WriteBlueprintData(string folder)
        {
            var bundles = Directory.CreateDirectory(Path.Combine(folder, "Bundles"));
            File.WriteAllText(Path.Combine(bundles.FullName, "cheatdata.json"), "{}");
        }

        private sealed class TemporaryFolder : IDisposable
        {
            private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory();
            public string Path => _directory.FullName;
            public void Dispose() => _directory.Delete(recursive: true);
        }
    }
}

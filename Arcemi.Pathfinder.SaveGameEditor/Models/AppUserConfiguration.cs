using ElectronNET.API;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class AppUserConfiguration
    {
        public string AppDataFolder { get; set; }
        public string GameFolder { get; set; }
        public AppUserDevelopmentConfiguration Development { get; set; } = new AppUserDevelopmentConfiguration();

        public void ApplyOn(AppUserConfiguration target)
        {
            target.GameFolder = GameFolder;
            target.AppDataFolder = AppDataFolder;
            if (target.Development is null) target.Development = new AppUserDevelopmentConfiguration();
            target.Development.IsEnabled = Development?.IsEnabled ?? false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AppDataFolder, GameFolder);
        }

        public override bool Equals(object obj)
        {
            if (obj is not AppUserConfiguration other) return false;
            return string.Equals(GameFolder, other.GameFolder, StringComparison.OrdinalIgnoreCase)
                && string.Equals(AppDataFolder, other.AppDataFolder, StringComparison.OrdinalIgnoreCase);
        }

        public AppUserConfiguration Clone()
        {
            var clone = new AppUserConfiguration();
            ApplyOn(clone);
            return clone;
        }

        public bool ValidateAppDataFolder()
        {
            if (string.IsNullOrEmpty(AppDataFolder)) return false;
            var folder = Path.Combine(AppDataFolder, "Saved Games");
            return Directory.Exists(folder);
        }

        public bool ValidateGameFolder()
        {
            if (string.IsNullOrEmpty(GameFolder)) return false;
            var cheatdataPath = Path.Combine(GameFolder, "Bundles", "cheatdata.json");
            return File.Exists(cheatdataPath);
        }


        public string GetSaveGamesFolder()
        {
            return Path.Combine(AppDataFolder, "Saved Games");
        }

        public async void Save(string path)
        {
            var bckPath = Path.ChangeExtension(path, ".bck.config");
            if (File.Exists(bckPath)) File.Delete(bckPath);
            if (File.Exists(path)) File.Move(path, bckPath);

            using var stream = File.OpenWrite(path);
            JsonSerializer.Serialize(stream, this);
        }

        public async Task SaveAsync(string path)
        {
            var bckPath = Path.ChangeExtension(path, ".bck.config");
            if (File.Exists(bckPath)) File.Delete(bckPath);
            if (File.Exists(path)) File.Move(path, bckPath);

            using (var stream = File.OpenWrite(path)) {
                await JsonSerializer.SerializeAsync(stream, this);
            }
        }

        public static async Task<AppUserConfiguration> LoadAsync(string path)
        {
            if (!File.Exists(path)) return await DetectAsync();
            Exception lastException = null;
            for (var i = 0; i < 3; i++) {
                try {
                    using (var stream = File.OpenRead(path)) {
                        return await JsonSerializer.DeserializeAsync<AppUserConfiguration>(stream);
                    }
                }
                catch (Exception ex) {
                    lastException = ex;
                }
            }
            if (lastException != null) {
                ExceptionDispatchInfo.Capture(lastException).Throw();
            }
            return await DetectAsync();
        }

        public static async Task<AppUserConfiguration> DetectAsync()
        {
            return new AppUserConfiguration {
                AppDataFolder = await DetectAppDataFolderAsync()
            };
        }

        private const string KeyAppData = "%appdata%";
        private static readonly string[] ProfilePaths = new[] {
            KeyAppData + @"\Owlcat Games\Pathfinder Wrath Of The Righteous",
            KeyAppData + @"\Owlcat Games\Pathfinder Kingmaker"
        };

        public static async Task<string> GetAppDataDirectory()
        {
            if (HybridSupport.IsElectronActive)
            {
                return await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.AppData);
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
        }

        public static async Task<string> GetAppUserConfigFilename()
        {
            string userConfigPath;
            if (HybridSupport.IsElectronActive)
            {
                userConfigPath = await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.UserData);
            }
            else
            {
                userConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            return Path.Combine(userConfigPath, "user.config");
        }

        private static async Task<string> DetectAppDataFolderAsync()
        {
            var appDataPath = await GetAppDataDirectory();
            var localLowAppDataPath = Path.Combine(Path.GetDirectoryName(appDataPath), "LocalLow");

            foreach (var dir in ProfilePaths) {
                if (dir[0] != '%') {
                    if (Directory.Exists(dir)) {
                        return dir;
                    }
                    continue;
                }

                var path = dir.Replace("%appdata%", localLowAppDataPath);
                if (Directory.Exists(path)) {
                    return path;
                }

                path = dir.Replace("%appdata%", appDataPath);
                if (Directory.Exists(path)) {
                    return path;
                }
            }

            return null;
        }
    }
}
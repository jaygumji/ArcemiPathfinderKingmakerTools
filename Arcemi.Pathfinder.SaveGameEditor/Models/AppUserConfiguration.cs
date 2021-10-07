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

        public string GetSaveGamesFolder()
        {
            return Path.Combine(AppDataFolder, "Saved Games");
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

        private static async Task<string> DetectAppDataFolderAsync()
        {
            var appDataPath = await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.AppData);
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
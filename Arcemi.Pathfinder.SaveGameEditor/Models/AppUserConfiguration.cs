using ElectronNET.API;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class AppUserConfiguration
    {
        public string AppDataFolder { get; set; }

        public async Task SaveAsync(string path)
        {
            using (var stream = File.OpenWrite(path)) {
                await JsonSerializer.SerializeAsync(stream, this);
            }
        }

        public static async Task<AppUserConfiguration> LoadAsync(string path)
        {
            if (!File.Exists(path)) return await DetectAsync();
            using (var stream = File.OpenRead(path)) {
                return await JsonSerializer.DeserializeAsync<AppUserConfiguration>(stream);
            }
        }

        private static async Task<AppUserConfiguration> DetectAsync()
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
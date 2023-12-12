using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditorGameConfiguration
    {
        public string DefinitionId { get; set; }
        public string AppDataFolder { get; set; }
        public string GameFolder { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(AppDataFolder, GameFolder);
        }

        public override bool Equals(object obj)
        {
            if (obj is not EditorGameConfiguration other) return false;
            return string.Equals(GameFolder, other.GameFolder, StringComparison.OrdinalIgnoreCase)
                && string.Equals(AppDataFolder, other.AppDataFolder, StringComparison.OrdinalIgnoreCase);
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

        public GameDefinition GetDefinition()
        {
            foreach (var definition in SupportedGames.All) {
                if (definition.Id.Eq(DefinitionId)) return definition;
            }
            return GameDefinition.NotSet;
        }
    }
    public class AppUserConfiguration
    {
        public AppUserDevelopmentConfiguration Development { get; set; } = new AppUserDevelopmentConfiguration();
        public List<EditorGameConfiguration> Games { get; set; } = new List<EditorGameConfiguration>();

        public EditorGameConfiguration GetGame(GameDefinition definition)
        {
            var game = Games.FirstOrDefault(g => g.DefinitionId.Eq(definition.Id));
            if (game is null) {
                game = new EditorGameConfiguration { DefinitionId = definition.Id };
                Games.Add(game);
            }
            return game;
        }

        public void EnsureDefaults()
        {
            foreach (var def in SupportedGames.All) {
                var game = Games.FirstOrDefault(g => g.DefinitionId.Eq(def.Id));
                if (game is null) {
                    game = new EditorGameConfiguration { DefinitionId = def.Id };
                    Games.Add(game);
                }
            }
        }

        public void ApplyOn(AppUserConfiguration target)
        {
            target.EnsureDefaults();
            foreach (var game in Games) {
                var targetGame = target.Games.First(g => g.DefinitionId.Eq(game.DefinitionId));
                targetGame.AppDataFolder = game.AppDataFolder;
                targetGame.GameFolder = game.GameFolder;
            }
            if (target.Development is null) target.Development = new AppUserDevelopmentConfiguration();
            target.Development.IsEnabled = Development?.IsEnabled ?? false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            for (var i = 0; i < Games.Count; i++) {
                hashCode.Add(Games[i]);
            }
            return hashCode.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not AppUserConfiguration other) return false;
            if (Development?.IsEnabled != other.Development?.IsEnabled) return false;
            foreach (var l in Games) {
                var r = other.Games.FirstOrDefault(g => g.DefinitionId.Eq(l.DefinitionId));
                if (r is null) return false;
                if (!l.Equals(r)) return false;
            }
            return true;
        }

        public AppUserConfiguration Clone()
        {
            var clone = new AppUserConfiguration();
            clone.EnsureDefaults();
            ApplyOn(clone);
            return clone;
        }

        public bool ValidateAppDataFolder(GameDefinition definition)
        {
            return GetGame(definition).ValidateAppDataFolder();
        }

        public bool ValidateGameFolder(GameDefinition definition)
        {
            return GetGame(definition).ValidateGameFolder();
        }

        public string GetSaveGamesFolder(GameDefinition definition)
         {
            return GetGame(definition).GetSaveGamesFolder();
        }

        public void Save(string path)
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
            var cfg = new AppUserConfiguration();
            foreach (var def in SupportedGames.All) {
                var game = new EditorGameConfiguration { DefinitionId = def.Id };
                game.AppDataFolder = await DetectAppDataFolderAsync(def);
            }
            return cfg;
        }

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

        private static async Task<string> DetectAppDataFolderAsync(GameDefinition def)
        {
            var appDataPath = await GetAppDataDirectory();
            var path = Path.Combine(Path.GetDirectoryName(appDataPath), def.WindowsRelativeAppDataPath);

            if (Directory.Exists(path)) {
                return path;
            }

            return null;
        }
    }
}
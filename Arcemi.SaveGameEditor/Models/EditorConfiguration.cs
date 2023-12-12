using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditorConfigurationGameView
    {
        private readonly EditorGameConfiguration game;
        private readonly Action save;

        public GameDefinition Definition { get; }

        public EditorConfigurationGameView(EditorGameConfiguration game, Action save)
        {
            this.game = game;
            this.save = save;
            Definition = game.GetDefinition();
        }

        public string AppDataFolder { get => game.AppDataFolder; set { game.AppDataFolder = value; save(); } }
        public string GameFolder { get => game.GameFolder; set { game.GameFolder = value; save(); } }

        public bool IsValidAppDataFolder => game.ValidateAppDataFolder();
        public bool IsValidGameFolder => game.ValidateGameFolder();
    }
    public class EditorConfiguration
    {
        private bool _isInitialized;

        public string ConfigPath { get; set; }
        public AppUserConfiguration Instance { get; private set; }

        private string FormatError(Exception ex)
        {
            return string.Concat(ex.Message, Environment.NewLine, ex.StackTrace.CutAt(500));
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            ConfigPath = await AppUserConfiguration.GetAppUserConfigFilename();
            try {
                Instance = await AppUserConfiguration.LoadAsync(ConfigPath);
            }
            catch (Exception ex) {
                Instance = await AppUserConfiguration.DetectAsync();

                if (HybridSupport.IsElectronActive)
                    Electron.Dialog.ShowErrorBox("Configuration error", $"Failed to load the configuration file. Please go to settings page and setup your settings again. Error was '{FormatError(ex)}'");
            }
            LoadConfigResources();
        }

        public IReadOnlyList<EditorConfigurationGameView> GetGamesView(Action save)
        {
            return Instance.Games.Select(g => new EditorConfigurationGameView(g, save)).ToArray();
        }

        private void LoadConfigResources()
        {
            Instance.EnsureDefaults();
            foreach (var game in Instance.Games) {
                var def = game.GetDefinition();
                def.Resources.SetDevelopmentMode(Instance.Development?.IsEnabled ?? false);
                def.Resources.LoadGameFolder(game.GameFolder);
                def.Resources.LoadFeatTemplates();
                def.Resources.LoadAppDataWwwRoot(game.AppDataFolder);
            }
        }

        public async Task SaveConfigAsync()
        {
            await Instance.SaveAsync(ConfigPath);
            LoadConfigResources();
        }

        public void SaveConfig()
        {
            Instance.Save(ConfigPath);
            LoadConfigResources();
        }
    }
}
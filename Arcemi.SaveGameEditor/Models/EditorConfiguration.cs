using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
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
                Logger.Current.Information("Loading configuration from " + ConfigPath);
                Instance = await AppUserConfiguration.LoadAsync(ConfigPath);
            }
            catch (Exception ex) {
                Instance = await AppUserConfiguration.DetectAsync();

                if (HybridSupport.IsElectronActive)
                    Electron.Dialog.ShowErrorBox("Configuration error", $"Failed to load the configuration file. Please go to settings page and setup your settings again. Error was '{FormatError(ex)}'");
            }
            await LoadConfigResourcesAsync();
        }

        public IReadOnlyList<EditorConfigurationGameView> GetGamesView()
        {
            return Instance.Games.Select(g => new EditorConfigurationGameView(g, Save)).ToArray();
        }

        private async Task LoadConfigResourcesAsync()
        {
            var workingDirectory = Path.GetDirectoryName(ConfigPath);
            Instance.EnsureDefaults();
            foreach (var game in Instance.Games) {
                var def = game.GetDefinition();
                def.Resources.SetDevelopmentMode(Instance.Development?.IsEnabled ?? false);
                var gameWorkingDirectory = Path.Combine(workingDirectory, def.Id);
                await def.Resources.LoadGameFolderAsync(gameWorkingDirectory, game.GameFolder);
                def.Resources.LoadFeatTemplates();
                def.Resources.LoadAppDataWwwRoot(game.AppDataFolder);
            }
        }

        private async Task SaveConfigAsync()
        {
            await Instance.SaveAsync(ConfigPath);
            await LoadConfigResourcesAsync();
        }

        public string SaveMessage { get; private set; }

        private int _saveMarker;
        private int _isSaving;
        public void Save()
        {
            System.Threading.Interlocked.Increment(ref _saveMarker);
            var s = System.Threading.Interlocked.CompareExchange(ref _isSaving, 1, 0);
            if (s == 1) return; // Already saving
            SaveMessage = null;

            Task.Factory.StartNew(async () => {
                while (_saveMarker > 0) {
                    System.Threading.Interlocked.Exchange(ref _saveMarker, 0);
                    try {
                        await SaveConfigAsync();
                    }
                    catch (Exception ex) {
                        SaveMessage = "Error when saving: " + ex.Message;
                    }
                }
                System.Threading.Interlocked.Exchange(ref _isSaving, 0);
            });
        }

    }
}
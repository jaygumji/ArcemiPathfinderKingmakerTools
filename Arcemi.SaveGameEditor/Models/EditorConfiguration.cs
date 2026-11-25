using Arcemi.Models;
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
        public string GameFolder { get => game.GameFolder; set { game.GameFolder = GameFolderDiscovery.ResolveGameFolder(value, Definition) ?? value; save(); } }

        public bool IsValidAppDataFolder => game.ValidateAppDataFolder();
        public bool IsValidGameFolder => game.ValidateGameFolder();
        public bool HasMissingFolders => string.IsNullOrWhiteSpace(AppDataFolder)
            || (Definition.IsReadingGameFolder && string.IsNullOrWhiteSpace(GameFolder));

        public void DetectMissingFolders()
        {
            var previous = (game.AppDataFolder, game.GameFolder);
            game.DetectMissingFolders(GameFolderDiscovery.ForCurrentUser());
            if (previous != (game.AppDataFolder, game.GameFolder)) save();
        }
    }
    public class EditorConfiguration
    {
        private readonly EditorErrorReports _reports;
        private readonly object _initializationLock = new();
        private Task _initialization;

        public EditorConfiguration(EditorErrorReports reports)
        {
            _reports = reports;
        }

        public string ConfigPath { get; set; }
        public AppUserConfiguration Instance { get; private set; }

        public string DefaultBackupFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify),
            "Arcemi Save Game Editor", "Backups");

        public SaveBackupStorage Backups => new(DefaultBackupFolder, Instance?.BackupFolder, Instance?.AllowSaveFolderBackupFallback == true);

        public void SaveBackupSettings() => QueueSave(reloadResources: false);

        public async Task InitializeAsync()
        {
            Task initialization;
            lock (_initializationLock) {
                initialization = _initialization ??= InitializeCoreAsync();
            }

            try {
                await initialization;
            }
            catch {
                lock (_initializationLock) {
                    if (_initialization == initialization) {
                        _initialization = null;
                    }
                }
                throw;
            }
        }

        private async Task InitializeCoreAsync()
        {
            ConfigPath = await AppUserConfiguration.GetAppUserConfigFilename();
            try {
                Logger.Current.Information("Loading configuration from " + ConfigPath);
                Instance = await AppUserConfiguration.LoadAsync(ConfigPath)
                    ?? throw new InvalidDataException("The settings file is empty.");
            }
            catch (Exception ex) {
                Instance = new AppUserConfiguration();

                _reports.Report("Load settings", ex);
            }
            Instance.DetectMissingFolders();
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

        public string SaveMessage { get; private set; }
        private readonly object _saveLock = new();
        private bool _isSaving;
        private bool _savePending;
        private bool _reloadResources;

        public void Save() => QueueSave(reloadResources: true);

        public void RememberGame(GameDefinition game)
        {
            if (Instance.LastGameDefinitionId.Eq(game.Id)) return;
            Instance.LastGameDefinitionId = game.Id;
            QueueSave(reloadResources: false);
        }

        private void QueueSave(bool reloadResources)
        {
            lock (_saveLock) {
                _savePending = true;
                _reloadResources |= reloadResources;
                if (_isSaving) return;
                _isSaving = true;
                SaveMessage = null;
            }
            _ = Task.Run(SavePendingAsync);
        }

        private async Task SavePendingAsync()
        {
            while (true) {
                bool reloadResources;
                lock (_saveLock) {
                    if (!_savePending) {
                        _isSaving = false;
                        return;
                    }
                    _savePending = false;
                    reloadResources = _reloadResources;
                    _reloadResources = false;
                }

                try {
                    await Instance.SaveAsync(ConfigPath);
                    if (reloadResources) await LoadConfigResourcesAsync();
                }
                catch (Exception ex) {
                    SaveMessage = "Error when saving: " + ex.Message;
                    _reports.Report("Save settings", ex);
                }
            }
        }

    }
}
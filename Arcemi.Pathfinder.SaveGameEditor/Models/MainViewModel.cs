using Arcemi.Pathfinder.Kingmaker;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class MainViewModel : ISaveDataProvider
    {
        private SaveGameFile _file;
        private JsonPartSaveGameFile _partyFile;
        private JsonPartSaveGameFile _playerFile;
        private JsonPartSaveGameFile _headerFile;

        private bool _isInitialized;
        private string _playerCharacterName;

        public string CurrentPath { get; private set; }
        public IEnumerable<UnitEntityModel> Characters => (Party?.UnitEntities?.Where(x => x.Descriptor != null)) ?? Array.Empty<UnitEntityModel>();

        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }
        public PartyModel Party { get; private set; }
        public HeaderModel Header { get; private set; }

        public UnitEntityModel MainCharacter { get; private set; }
        UnitEntityModel ISaveDataProvider.PlayerEntity
        {
            get => MainCharacter;
            set {
                MainCharacter = value;
                Inventory = value.Descriptor.Inventory;
            }
        }

        public InventoryModel Inventory { get; private set; }
        public InventoryModel SharedStash { get; private set; }

        private string ConfigPath { get; set; }
        public AppUserConfiguration Config { get; private set; }

        private readonly GameResources _resources;
        public IGameResourcesProvider Resources => _resources;

        public MainViewModel()
        {
            _resources = new GameResources();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            var userConfigPath = await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.UserData);
            ConfigPath = Path.Combine(userConfigPath, "user.config");
            try {
                Config = await AppUserConfiguration.LoadAsync(ConfigPath);
            }
            catch (Exception ex) {
                Config = await AppUserConfiguration.DetectAsync();
                Electron.Dialog.ShowErrorBox("Configuration error", $"Failed to load the configuration file. Please go to settings page and setup your settings again. Error was '{FormatError(ex)}'");
            }
            LoadConfigResources();
        }

        private string FormatError(Exception ex)
        {
            return string.Concat(ex.Message, Environment.NewLine, ex.StackTrace.CutAt(500));
        }

        public bool ValidateAppDataFolder()
        {
            if (string.IsNullOrEmpty(Config.AppDataFolder)) return false;
            var folder = Path.Combine(Config.AppDataFolder, "Saved Games");
            return Directory.Exists(folder);
        }

        public bool ValidateGameFolder()
        {
            if (string.IsNullOrEmpty(Config.GameFolder)) return false;
            var cheatdataPath = Path.Combine(Config.GameFolder, "Bundles", "cheatdata.json");
            return File.Exists(cheatdataPath);
        }

        private void LoadConfigResources()
        {
            _resources.Blueprints = BlueprintData.Load(Config.GameFolder);

            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            _resources.AppData = new PathfinderAppData(new WwwRootResourceProvider(wwwRoot, () => Config.AppDataFolder));
        }

        public async Task SaveConfigAsync()
        {
            await Config.SaveAsync(ConfigPath);
            LoadConfigResources();
        }

        public async Task OpenAsync(string path)
        {
            _file = new SaveGameFile(path, Resources);
            _partyFile = _file.GetParty();
            _playerFile = _file.GetPlayer();
            _headerFile = _file.GetHeader();
            Player = _playerFile.GetRoot<PlayerModel>();
            Party = _partyFile.GetRoot<PartyModel>();
            Header = _headerFile.GetRoot<HeaderModel>();

            foreach (var character in Characters) {
                if (character.Descriptor == null) continue;
                if (string.Equals(character.UniqueId, Player.MainCharacterId, StringComparison.Ordinal)) {
                    MainCharacter = character;
                }
            }

            CurrentPath = path;
            Inventory = MainCharacter?.Descriptor?.Inventory;
            SharedStash = Player.SharedStash;
            _playerCharacterName = GetMainCharacterName();
            CanEdit = true;
        }

        public async Task SaveAsync(string path)
        {
            var currentMainCharacterName = GetMainCharacterName();
            if (!string.IsNullOrEmpty(_playerCharacterName) && !string.IsNullOrEmpty(currentMainCharacterName)
                && !string.Equals(_playerCharacterName, currentMainCharacterName, StringComparison.OrdinalIgnoreCase)) {
                // If the main character name was detected as changed, update the header and game id
                Header.PlayerCharacterName = currentMainCharacterName;
                Header.GameId = Guid.NewGuid().ToString("N");
            }
            if (Header.GameSaveTime != Player.GameTime) {
                Header.GameSaveTime = Player.GameTime;
            }
            CanEdit = false;
            _headerFile.Save();
            _partyFile.Save();
            _playerFile.Save();
            _file.Save(path);
            CurrentPath = path;
            CanEdit = true;
        }

        private string GetMainCharacterName()
        {
            return (MainCharacter?.Descriptor?.CustomName)
                ?? Characters.FirstOrDefault(c => c.Descriptor.IsPlayer)?.Descriptor.CustomName;
        }
    }
}

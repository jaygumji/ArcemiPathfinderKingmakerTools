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
        public IEnumerable<UnitEntityModel> Characters { get; private set; }

        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }
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

        public PathfinderAppData AppData { get; private set; }

        public MainViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            var userConfigPath = await Electron.App.GetPathAsync(ElectronNET.API.Entities.PathName.UserData);
            ConfigPath = Path.Combine(userConfigPath, "user.config");
            Config = await AppUserConfiguration.LoadAsync(ConfigPath);

            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            AppData = new PathfinderAppData(new WwwRootResourceProvider(wwwRoot, () => Config.AppDataFolder));
        }

        public async Task SaveConfigAsync()
        {
            await Config.SaveAsync(ConfigPath);
        }

        public async Task OpenAsync(string path)
        {
            _file = new SaveGameFile(path);
            _partyFile = _file.GetParty();
            _playerFile = _file.GetPlayer();
            _headerFile = _file.GetHeader();
            Player = _playerFile.GetRoot<PlayerModel>();
            var party = _partyFile.GetRoot<PartyModel>();

            Header = _headerFile.GetRoot<HeaderModel>();

            var characters = party.UnitEntities
                .Where(u => u.Descriptor != null)
                .ToList();

            foreach (var character in characters) {
                if (string.Equals(character.UniqueId, Player.MainCharacterId, StringComparison.Ordinal)) {
                    MainCharacter = character;
                }
                character.Descriptor.UISettings.Init(AppData.Portraits);
            }

            if (Player.LeadersManager?.Leaders?.Count > 0) {
                foreach (var leader in Player.LeadersManager.Leaders) {
                    leader.Init(AppData.Portraits);
                }
            }

            CurrentPath = path;
            Inventory = MainCharacter?.Descriptor?.Inventory;
            SharedStash = Player.SharedStash;
            Characters = characters;
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

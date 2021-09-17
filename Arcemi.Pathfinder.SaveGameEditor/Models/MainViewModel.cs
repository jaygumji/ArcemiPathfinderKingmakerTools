using Arcemi.Pathfinder.Kingmaker;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class MainViewModel
    {
        private SaveGameFile _file;
        private JsonPartSaveGameFile _partyFile;
        private JsonPartSaveGameFile _playerFile;

        private bool _isInitialized;

        public string CurrentPath { get; private set; }
        public List<UnitEntityModel> Characters { get; private set; }

        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }

        public UnitEntityModel PlayerEntity { get; private set; }
        public InventoryViewModel Inventory { get; private set; }
        public InventoryViewModel SharedStash { get; private set; }

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
            var appPath = await Electron.App.GetAppPathAsync();
            ConfigPath = Path.Combine(appPath, "user.config");
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
            Player = _playerFile.GetRoot<PlayerModel>();
            var party = _partyFile.GetRoot<PartyModel>();

            var characters = party.UnitEntities
                .Where(u => u.Descriptor != null)
                .ToList();

            UnitEntityModel mainCharacter = null;
            foreach (var character in characters) {
                if (string.Equals(character.UniqueId, Player.MainCharacterId, StringComparison.Ordinal)) {
                    mainCharacter = character;
                }
                character.Descriptor.UISettings.Init(AppData.Portraits);
            }

            CurrentPath = path;
            Inventory = mainCharacter == null ? null : new InventoryViewModel(mainCharacter.Descriptor.Inventory);
            SharedStash = new InventoryViewModel(Player.SharedStash);
            Characters = characters;
            CanEdit = true;
        }

        public async Task SaveAsync(string path)
        {
            CanEdit = false;
            _partyFile.Save();
            _playerFile.Save();
            _file.Save(path);
            CurrentPath = path;
            CanEdit = true;
        }
    }
}

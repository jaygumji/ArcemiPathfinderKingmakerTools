using Arcemi.Pathfinder.Kingmaker;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public SaveFileLocation Location { get; private set; }
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
        public AppUserConfiguration EditConfig { get; private set; }
        public bool HasUnsavedConfigChanges => !Config.Equals(EditConfig);

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
            
            ConfigPath = await AppUserConfiguration.GetAppUserConfigFilename();
            try {
                Config = await AppUserConfiguration.LoadAsync(ConfigPath);
                EditConfig = Config.Clone();
            }
            catch (Exception ex) {
                Config = await AppUserConfiguration.DetectAsync();
                EditConfig = Config.Clone();

                if (HybridSupport.IsElectronActive)
                    Electron.Dialog.ShowErrorBox("Configuration error", $"Failed to load the configuration file. Please go to settings page and setup your settings again. Error was '{FormatError(ex)}'");
            }
            LoadConfigResources();

            if (HybridSupport.IsElectronActive)
                Electron.App.BeforeQuit += App_BeforeQuit;
        }

        private Task App_BeforeQuit(QuitEventArgs arg)
        {
            _file?.Close();
            return Task.CompletedTask;
        }

        private string FormatError(Exception ex)
        {
            return string.Concat(ex.Message, Environment.NewLine, ex.StackTrace.CutAt(500));
        }

        public bool ValidateAppDataFolder()
        {
            return Config?.ValidateAppDataFolder() ?? false;
        }

        public bool ValidateGameFolder()
        {
            return Config?.ValidateGameFolder() ?? false;
        }

        private void LoadConfigResources()
        {
            _resources.Blueprints = BlueprintMetadata.Load(Config.GameFolder);
            LoadFeatTemplates();

            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
#if DEBUG
            if (!Directory.Exists(wwwRoot))
            {
                // We're probably running in the debugger without dotnet publish
                wwwRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
#endif
            _resources.AppData = new PathfinderAppData(new WwwRootResourceProvider(wwwRoot, () => Config.AppDataFolder));
        }

        private void LoadFeatTemplates()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "FeatTemplates.json");
            var contents = File.ReadAllText(path);
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(contents);
            var templates = new List<FeatureFactItemModel>();
            foreach (var item in jObjects)
            {
                templates.Add(new FeatureFactItemModel(new ModelDataAccessor(item, new References(Resources), Resources)));
            }
            _resources.FeatTemplates = templates;
        }

        public async Task SaveConfigAsync()
        {
            EditConfig.ApplyOn(Config);
            await Config.SaveAsync(ConfigPath);
            LoadConfigResources();
        }

        public void OpenBackup(SaveFileLocation loc)
        {
            Open(loc.BackupFilePath, loc.FilePath);
        }

        public void Open(string path, string originalPath = null)
        {
            _file?.Close();
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

            Location = string.IsNullOrEmpty(originalPath)
                ? new SaveFileLocation(path)
                : new SaveFileLocation(originalPath);
            Inventory = MainCharacter?.Descriptor?.Inventory;
            SharedStash = Player.SharedStash;
            _playerCharacterName = GetMainCharacterName();
            CanEdit = true;
        }

        public void Restore()
        {
            var tmp = Path.ChangeExtension(Location.FilePath, ".zks.tmp");
            try {
                File.Move(Location.FilePath, tmp);
                File.Move(Location.BackupFilePath, Location.FilePath);
                File.Delete(tmp);
            }
            catch (Exception) {
                if (!File.Exists(Location.FilePath) && File.Exists(tmp)) {
                    File.Move(tmp, Location.FilePath);
                }
                throw;
            }
            Location = Location.Refresh();
            Open(Location.FilePath);
        }

        public void Save(SaveFileLocation location)
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
            foreach (var character in Characters) {
                if (character.Descriptor?.Alignment?.History != null) {
                    var vector = character.Descriptor.Alignment.Vector;
                    var history = character.Descriptor.Alignment.History.LastOrDefault();
                    if (!string.Equals(vector.Value, history?.Position, StringComparison.Ordinal)) {
                        history = character.Descriptor.Alignment.History.Add();
                        history.Vector.X = vector.X;
                        history.Vector.Y = vector.Y;
                        history.Direction = vector.DirectionText;
                        history.Provider = null;
                    }
                }
            }

            if (string.Equals(Location.FilePath, location.FilePath, StringComparison.Ordinal)) {
                // Overwriting the same file. No metadata needs to be changed.
            }
            else if (location.FileExists) {
                // We're overwriting another file than the one we opened.
                var oldHeader = SaveGameFile.ReadHeader(location.FilePath, Resources); ;
                Header.Type = oldHeader.Type;
                Header.Name = oldHeader.Name;
                Header.QuickSaveNumber = oldHeader.QuickSaveNumber;
            }
            else {
                Header.Type = SaveFileType.Manual;
                Header.QuickSaveNumber = 0;
                Header.Name = location.Name;
            }

            CanEdit = false;
            _headerFile.Save();
            _partyFile.Save();
            _playerFile.Save();

            if (location.BackupExists) {
                File.Delete(location.BackupFilePath);
            }
            if (location.FileExists) {
                File.Move(location.FilePath, location.BackupFilePath);
            }
            _file.Save(location.FilePath);
            Location = location.Refresh();
            CanEdit = true;
        }

        private string GetMainCharacterName()
        {
            return (MainCharacter?.Descriptor?.CustomName)
                ?? Characters.FirstOrDefault(c => c.IsPlayer)?.Descriptor.CustomName;
        }
    }
}

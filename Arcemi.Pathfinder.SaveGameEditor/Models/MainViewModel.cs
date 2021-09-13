using Arcemi.Pathfinder.Kingmaker;
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

        public string CurrentPath { get; private set; }
        public List<CharacterModel> Characters { get; private set; }

        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }

        public CharacterModel PlayerCharacter { get; private set; }
        public InventoryViewModel InventoryModel { get; private set; }
        public InventoryViewModel SharedStashModel { get; private set; }

        public PathfinderAppData AppData { get; }

        public MainViewModel()
        {
            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            AppData = new PathfinderAppData(new WwwRootResourceProvider(wwwRoot, FindDefaultFolder()));
        }

        private const string KeyAppData = "%appdata%";
        private static readonly string[] ProfilePaths = new[] {
            KeyAppData + @"\Owlcat Games\Pathfinder Kingmaker",
            KeyAppData + @"\Owlcat Games\Pathfinder Wrath Of The Righteous"
        };
        private static string FindDefaultFolder()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
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

                path = dir.Replace("%appdata%", localAppDataPath);
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

        public async Task OpenAsync(string path)
        {
            _file = new SaveGameFile(path);
            _partyFile = _file.GetParty();
            _playerFile = _file.GetPlayer();
            Player = _playerFile.GetRoot<PlayerModel>();
            var party = _partyFile.GetRoot<PartyModel>();

            var characters = party.UnitEntities
                .Where(u => u.Descriptor != null)
                .Select(u => u.Descriptor)
                .ToList();

            var playerUnit = party.Find(Player.MainCharacterId);
            if (playerUnit != null) {
                playerUnit.Descriptor.UISettings.Init(AppData.Portraits);
                PlayerCharacter = playerUnit.Descriptor;
                InventoryModel = new InventoryViewModel(playerUnit.Descriptor.Inventory);
            }

            var isMainCharacterFound = false;
            foreach (var character in characters) {
                character.UISettings.Init(AppData.Portraits);
                var isMainCharacter = string.Equals(character.Id, playerUnit.Descriptor.Id, StringComparison.Ordinal);
                if (isMainCharacter) {
                    isMainCharacterFound = true;
                }
            }
            if (playerUnit != null && !isMainCharacterFound) {
                var newCharacters = characters.ToList();
                newCharacters.Insert(0, playerUnit.Descriptor);
                characters = newCharacters;
            }

            CurrentPath = path;
            SharedStashModel = new InventoryViewModel(Player.SharedStash);
            Characters = characters;
            //Character = null;
            //Leader = null;
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

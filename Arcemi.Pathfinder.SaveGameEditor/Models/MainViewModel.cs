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

        public string CurrentPath { get; private set; }
        public List<UnitEntityModel> Characters { get; private set; }

        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }

        public UnitEntityModel PlayerEntity { get; private set; }
        public InventoryViewModel SharedStashModel { get; private set; }

        public PathfinderAppData AppData { get; private set; }

        public MainViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            var defaultFolder = await FindDefaultFolderAsync();
            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            AppData = new PathfinderAppData(new WwwRootResourceProvider(wwwRoot, defaultFolder));
        }

        private const string KeyAppData = "%appdata%";
        private static readonly string[] ProfilePaths = new[] {
            KeyAppData + @"\Owlcat Games\Pathfinder Wrath Of The Righteous",
            KeyAppData + @"\Owlcat Games\Pathfinder Kingmaker"
        };
        private static async Task<string> FindDefaultFolderAsync()
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

            foreach (var character in characters) {
                character.Descriptor.UISettings.Init(AppData.Portraits);
            }

            CurrentPath = path;
            SharedStashModel = new InventoryViewModel(Player.SharedStash);
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

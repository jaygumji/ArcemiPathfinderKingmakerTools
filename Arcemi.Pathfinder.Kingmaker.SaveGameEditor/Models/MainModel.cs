#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models
{
    public class MainModel : ViewModel
    {
        private const string KeyAppData = "%appdata%";
        private static readonly string[] ProfilePaths = new[] {
            KeyAppData + @"\Owlcat Games\Pathfinder Kingmaker"
        };

        public UserSettings UserSettings { get; }
        private SaveGameFile _file;
        private JsonPartSaveGameFile _partyFile;
        private JsonPartSaveGameFile _playerFile;
        public PathfinderAppData AppData { get; }

        private PlayerModel _player;
        public PlayerModel Player {
            get => _player;
            private set {
                _player = value;
                NotifyPropertyChanged();
            }
        }

        private IReadOnlyList<CharacterModel> _characters;
        public IReadOnlyList<CharacterModel> Characters {
            get => _characters;
            private set {
                _characters = value;
                NotifyPropertyChanged();
            }
        }

        private int _leaderBonus;
        public int LeaderBonus
        {
            get => _leaderBonus;
            set {
                _leaderBonus = value;
                NotifyPropertyChanged();
                Leader.SetSelectedLeaderBonus(value);
            }
        }

        public bool CanEditLeader => _leader != null;

        private PlayerKingdomLeaderModel _leader;
        public PlayerKingdomLeaderModel Leader
        {
            get => _leader;
            set {
                _leader = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CanEditLeader));

                _leaderBonus = _leader?.SpecificBonuses?
                    .Where(b => string.Equals(b.Key, _leader.LeaderSelection, StringComparison.OrdinalIgnoreCase))
                    .Select(b => b.Value)
                    .FirstOrDefault() ?? 0;
                NotifyPropertyChanged("LeaderBonus");
            }
        }

        public bool CanEditCharacter => _character != null;

        private CharacterModel _character;
        public CharacterModel Character
        {
            get => _character;
            set {
                _character = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CanEditCharacter));
            }
        }

        private CharacterModel _playerCharacter;
        public CharacterModel PlayerCharacter
        {
            get => _playerCharacter;
            set {
                _playerCharacter = value;
                NotifyPropertyChanged();
            }
        }

        private InventoryViewModel _inventoryModel;
        public InventoryViewModel InventoryModel
        {
            get => _inventoryModel;
            set {
                _inventoryModel = value;
                NotifyPropertyChanged();
            }
        }

        private InventoryViewModel _sharedStashModel;
        public InventoryViewModel SharedStashModel
        {
            get => _sharedStashModel;
            set {
                _sharedStashModel = value;
                NotifyPropertyChanged();
            }
        }

        private bool _canEdit;
        public bool CanEdit
        {
            get => _canEdit;
            set {
                _canEdit = true;
                NotifyPropertyChanged();
            }
        }

        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }

        public MainModel()
        {
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save, s => _file != null);
            UserSettings = UserSettings.FromDefaultPath();
            AppData = new PathfinderAppData(typeof(MainModel), FindDefaultFolder());
        }

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

        private void Open(object parameter)
        {
            var dlg = new OpenFileDialog {
                Filter = "Save files|*.zks",
                FilterIndex = 1
            };

            var path = UserSettings.OpenPath
                ?? AppData.SavedGamesDirectory;

            if (!string.IsNullOrEmpty(path)) {
                dlg.InitialDirectory = path;
            }

            if (dlg.ShowDialog() == true) {
                var filePath = dlg.FileName;
                _file?.Close();
                _file = new SaveGameFile(filePath);
                LoadFile();

                UserSettings.OpenPath = Path.GetDirectoryName(filePath);
                UserSettings.Save();
            }
        }

        private void LoadFile()
        {
            _partyFile = _file.GetParty();
            _playerFile = _file.GetPlayer();
            Player = _playerFile.GetRoot<PlayerModel>();

            var characters = _partyFile.GetAllOf<CharacterModel>();

            foreach (var character in characters) {
                character.UISettings.Init(AppData.Portraits);
                if (character.IsPlayer) {
                    PlayerCharacter = character;
                    InventoryModel = new InventoryViewModel(character.Inventory);
                }
            }
            if (Player.Kingdom?.Leaders != null) {
                foreach (var leader in Player.Kingdom.Leaders) {
                    leader.Init(AppData.Portraits);
                }
            }
            SharedStashModel = new InventoryViewModel(Player.SharedStash);
            Characters = characters;
            Character = null;
            Leader = null;
            CanEdit = true;
        }

        private void Save(object parameter)
        {
            var dlg = new SaveFileDialog {
                Filter = "Save files|*.zks",
                FilterIndex = 1
            };

            var dir = Path.GetDirectoryName(_file.Filepath);
            var fileName = Path.GetFileName(_file.Filepath);
            dlg.InitialDirectory = dir;
            dlg.FileName = fileName;
 
            if (dlg.ShowDialog() == true) {
                CanEdit = false;
                var filePath = dlg.FileName;
                _partyFile.Save();
                _playerFile.Save();
                _file.Save(filePath);
                CanEdit = true;
            }
        }

        public void HandleClose()
        {
            _file?.Close();
            _file = null;
        }
    }
}

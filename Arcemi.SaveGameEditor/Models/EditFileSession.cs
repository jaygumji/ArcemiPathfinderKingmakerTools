using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditFileSession : IEditFileSession
    {
        public event EventHandler<EditFileSessionOpenedArgs> Opened;

        private SaveGameFile _file;
        private JsonPartSaveGameFile _partyFile;
        private JsonPartSaveGameFile _playerFile;
        private JsonPartSaveGameFile _headerFile;

        private string _playerCharacterName;

        public SaveFileLocation Location { get; private set; }
        public IGameAccessor Game { get; private set; }
        public bool CanEdit { get; private set; }
        public PlayerModel Player { get; private set; }
        public PartyModel Party { get; private set; }
        public HeaderModel Header { get; private set; }

        public EditFileSession()
        {
            Game = new NotSetGameAccessor();

            if (HybridSupport.IsElectronActive)
                Electron.App.BeforeQuit += App_BeforeQuit;
        }

        private Task App_BeforeQuit(QuitEventArgs arg)
        {
            _file?.Close();
            return Task.CompletedTask;
        }

        public void OpenBackup(SaveFileLocation loc)
        {
            Open(loc.BackupFilePath, loc.FilePath);
        }

        public void Open(string path, string originalPath = null)
        {
            _file?.Close();
            _file = new SaveGameFile(path);
            _partyFile = _file.GetParty();
            _playerFile = _file.GetPlayer();
            _headerFile = _file.GetHeader();
            Player = _playerFile.GetRoot<PlayerModel>();
            Party = _partyFile.GetRoot<PartyModel>();
            Header = _headerFile.GetRoot<HeaderModel>();

            Game = SupportedGames.Detect(this);
            //if (Game.Definition.Resources.AppData is null) {
            //    _editorConfig.Instance
            //}

            Location = string.IsNullOrEmpty(originalPath)
                ? new SaveFileLocation(path)
                : new SaveFileLocation(originalPath);

            _playerCharacterName = Game.MainCharacter?.Name;

            CanEdit = true;
            if (Opened is not null) Opened.Invoke(this, new EditFileSessionOpenedArgs(Game, Location));
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
            var currentMainCharacterName = Game.MainCharacter.Name;
            if (!string.IsNullOrEmpty(_playerCharacterName) && !string.IsNullOrEmpty(currentMainCharacterName)
                && !string.Equals(_playerCharacterName, currentMainCharacterName, StringComparison.OrdinalIgnoreCase)) {
                // If the main character name was detected as changed, update the header and game id
                Header.PlayerCharacterName = currentMainCharacterName;
                Header.GameId = Guid.NewGuid().ToString("N");
            }
            if (Header.GameSaveTime != Player.GameTime) {
                Header.GameSaveTime = Player.GameTime;
            }
            Game.BeforeSave();

            if (string.Equals(Location.FilePath, location.FilePath, StringComparison.Ordinal)) {
                // Overwriting the same file. No metadata needs to be changed.
            }
            else if (location.FileExists) {
                // We're overwriting another file than the one we opened.
                var oldHeader = SaveGameFile.ReadHeader(location.FilePath); ;
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
    }
}
using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditFileSession : IEditFileSession
    {
        public event EventHandler<EditFileSessionOpenedArgs> Opened;

        private readonly EditorConfiguration _config;
        private SaveGameDocument _document;
        private string _playerCharacterName;

        public SaveFileLocation Location { get; private set; }
        public IGameAccessor Game { get; private set; }
        public bool IsOpen => _document is not null;
        public bool CanEdit => _document?.CanSave == true;
        public PlayerModel Player => _document?.Json?.Player;

        public string BackupPath => Location is null ? null : _config.Backups.Find(Location.FilePath);

        public EditFileSession(EditorConfiguration config)
        {
            _config = config;
            Game = new NotSetGameAccessor();

            if (HybridSupport.IsElectronActive)
                Electron.App.BeforeQuit += App_BeforeQuit;
        }

        private Task App_BeforeQuit(QuitEventArgs arg)
        {
            _document?.Dispose();
            return Task.CompletedTask;
        }

        public void OpenBackup(SaveFileLocation loc)
        {
            var backup = _config.Backups.Find(loc.FilePath) ?? throw new FileNotFoundException("No backup was found for this save.");
            Open(backup, loc.FilePath);
        }

        public void Open(string path, string originalPath = null)
        {
            var elapsed = Stopwatch.StartNew();
            var location = new SaveFileLocation(string.IsNullOrEmpty(originalPath) ? path : originalPath);
            var document = new SaveGameDocument(path);
            var previous = _document;
            _document = document;
            Game = document.Game;
            Location = location;
            _playerCharacterName = Game.MainCharacter?.Name;
            previous?.Dispose();

            Opened?.Invoke(this, new EditFileSessionOpenedArgs(Game, Location));
            Logger.Current.Information($"Opened {Game.Definition.Id} save in {elapsed.ElapsedMilliseconds} ms");
        }

        public void Restore()
        {
            _config.Backups.Restore(Location.FilePath);
            Open(Location.FilePath);
        }

        public void Save(SaveFileLocation location)
        {
            if (!CanEdit) throw new InvalidOperationException("This save is open in read-only mode.");

            var header = _document.Header;
            var previous = (header.PlayerCharacterName, header.GameId, header.GameSaveTime, header.Type, header.Name, header.QuickSaveNumber);
            var currentMainCharacterName = Game.MainCharacter?.Name;

            try {
                if (!string.IsNullOrEmpty(_playerCharacterName) && !string.IsNullOrEmpty(currentMainCharacterName)
                    && !string.Equals(_playerCharacterName, currentMainCharacterName, StringComparison.OrdinalIgnoreCase)) {
                    header.PlayerCharacterName = currentMainCharacterName;
                    header.GameId = Guid.NewGuid().ToString("N");
                }
                if (Player is not null && header.GameSaveTime != Player.GameTime) {
                    header.GameSaveTime = Player.GameTime;
                }

                if (!string.Equals(Location.FilePath, location.FilePath, StringComparison.Ordinal)) {
                    if (File.Exists(location.FilePath)) {
                        var oldHeader = SaveGameFile.ReadHeader(location.FilePath)
                            ?? throw new InvalidDataException("The destination save is missing its header.");
                        header.Type = oldHeader.Type;
                        header.Name = oldHeader.Name;
                        header.QuickSaveNumber = oldHeader.QuickSaveNumber;
                    }
                    else {
                        header.Type = SaveFileType.Manual;
                        header.QuickSaveNumber = 0;
                        header.Name = location.Name;
                    }
                }

                var backups = _config.Backups;
                _document.Save(location.FilePath, backups.GetWritePath(location.FilePath), backups.GetFallbackPath(location.FilePath));
            }
            catch {
                (header.PlayerCharacterName, header.GameId, header.GameSaveTime, header.Type, header.Name, header.QuickSaveNumber) = previous;
                throw;
            }

            Location = location.Refresh();
            _playerCharacterName = currentMainCharacterName;
        }
    }
}

using Arcemi.Models;
using System;

namespace Arcemi.SaveGameEditor.Models
{
    public class EditFileSessionOpenedArgs
    {
        public EditFileSessionOpenedArgs(IGameAccessor game, SaveFileLocation location)
        {
            Game = game;
            Location = location;
        }

        public IGameAccessor Game { get; }
        public SaveFileLocation Location { get; }
    }
    public interface IEditFileSession
    {
        event EventHandler<EditFileSessionOpenedArgs> Opened;

        bool IsOpen { get; }
        bool CanEdit { get; }
        PlayerModel Player { get; }
        SaveFileLocation Location { get; }
        string BackupPath { get; }
        IGameAccessor Game { get; }

        void OpenBackup(SaveFileLocation loc);
        void Open(string path, string originalPath = null);
        void Restore();
        void Save(SaveFileLocation location);
    }
}
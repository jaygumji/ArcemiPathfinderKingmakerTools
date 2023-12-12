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
    public interface IEditFileSession : IGameEditFile
    {
        event EventHandler<EditFileSessionOpenedArgs> Opened;

        bool CanEdit { get; }
        SaveFileLocation Location { get; }
        IGameAccessor Game { get; }

        void OpenBackup(SaveFileLocation loc);
        void Open(string path, string originalPath = null);
        void Restore();
        void Save(SaveFileLocation location);
    }
}
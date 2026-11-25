using Arcemi.Models.Warhammer40KDarkHeresy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Arcemi.Models
{
    public sealed class SaveGameDocument : IDisposable
    {
        internal static readonly SaveEntry<JsonPartSaveGameFile> HeaderEntry = new("header.json", SaveGameFile.ReadJson);
        private static readonly SaveEntry<JsonPartSaveGameFile> PartyEntry = new("party.json", SaveGameFile.ReadJson);
        private static readonly SaveEntry<JsonPartSaveGameFile> PlayerEntry = new("player.json", SaveGameFile.ReadJson);

        private readonly SaveGameFile _archive;
        private readonly W40KDHPlayerDocument _owlPlayer;
        private readonly W40KDHPartyDocument _owlParty;
        private readonly Dictionary<string, JsonPartSaveGameFile> _editableEntries = new(StringComparer.OrdinalIgnoreCase);

        public SaveGameDocument(string path)
        {
            _archive = SaveGameFile.Snapshot(path);
            try {
                var headerPart = _archive.Read(HeaderEntry);
                Header = headerPart.GetRoot<HeaderModel>();
                _editableEntries.Add(HeaderEntry.Name, headerPart);
                if (W40KDHGameAccessor.IsOwlSave(_archive, Header, _archive.Contains(PlayerEntry) && _archive.Contains(PartyEntry))) {
                    var owl = W40KDHGameAccessor.Open(_archive, Header);
                    _owlPlayer = owl.Player;
                    _owlParty = owl.OwlParty;
                    Game = owl;
                    return;
                }

                var playerPart = _archive.Read(PlayerEntry);
                var partyPart = _archive.Read(PartyEntry);
                Json = new JsonGameEditFile(Header, playerPart.GetRoot<PlayerModel>(), partyPart.GetRoot<PartyModel>());
                Game = SupportedGames.Detect(Json);
                if (Game.Definition == GameDefinition.NotSet) {
                    throw new InvalidDataException("The game for this save could not be identified.");
                }
                _editableEntries.Add(PlayerEntry.Name, playerPart);
                _editableEntries.Add(PartyEntry.Name, partyPart);
            }
            catch {
                _archive.Dispose();
                throw;
            }
        }

        public HeaderModel Header { get; }
        public IGameEditFile Json { get; }
        public IGameAccessor Game { get; }
        public bool CanSave => Json is not null || _owlPlayer?.CanEdit == true || _owlParty?.CanEdit == true;

        public void Save(string destination, string backupPath, string fallbackBackupPath = null)
        {
            if (!CanSave) throw new InvalidOperationException("This save is open in read-only mode.");

            if (Game is IGameSavePreparation preparation) preparation.BeforeSave();
            var changes = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var (name, part) in _editableEntries) {
                if (part.HasChanges) changes.Add(name, part.ToBytes());
            }
            _owlPlayer?.StageChanges(changes);
            if (_owlParty is not null) {
                _owlParty.StageChanges(changes);
                W40KDHOwlChecksums.StageChanges(_archive, changes);
            }
            _archive.Save(destination, backupPath, fallbackBackupPath, changes);
            foreach (var (name, part) in _editableEntries) {
                if (changes.ContainsKey(name)) part.AcceptChanges();
            }
            _owlPlayer?.AcceptChanges();
            _owlParty?.AcceptChanges();
        }

        public void Dispose() => _archive.Dispose();

        private sealed record JsonGameEditFile(HeaderModel Header, PlayerModel Player, PartyModel Party) : IGameEditFile;
    }
}

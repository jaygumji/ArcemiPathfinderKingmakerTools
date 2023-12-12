using Arcemi.Models;
using Arcemi.SaveGameEditor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Tests.Mocks
{
    public class MockEditFileSession : IEditFileSession
    {
        public bool CanEdit => true;
        public HeaderModel Header { get; set; }
        public PlayerModel Player { get; set; }
        public PartyModel Party { get; set; }
        public SaveFileLocation Location { get; set; }

        public IGameAccessor Game { get; set; }

        public event EventHandler<EditFileSessionOpenedArgs> Opened;

        public static MockEditFileSession FromPartyJson(string content, string mainCharacterId, IGameResourcesProvider resources)
        {
            var partyFile = new JsonPartSaveGameFile(null, JObject.Parse(content));
            var party = partyFile.GetRoot<PartyModel>();
            var playerFile = new JsonPartSaveGameFile(null, JObject.Parse($"{{\"MainCharacterId\":\"{mainCharacterId}\"}}"));
            var session = new MockEditFileSession {
                Player = playerFile.GetRoot<PlayerModel>(),
                Party = partyFile.GetRoot<PartyModel>()
            };
            session.Game = SupportedGames.Detect(session);
            return session;
        }

        public void Open(string path, string originalPath = null)
        {
            throw new NotImplementedException();
        }

        public void OpenBackup(SaveFileLocation loc)
        {
            throw new NotImplementedException();
        }

        public void Restore()
        {
            throw new NotImplementedException();
        }

        public void Save(SaveFileLocation location)
        {
            throw new NotImplementedException();
        }
    }
}

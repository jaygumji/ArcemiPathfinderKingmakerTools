using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Newtonsoft.Json.Linq;

namespace Arcemi.Tests.Mocks
{
    public class MockEditFileSession : IGameEditFile
    {
        public HeaderModel Header { get; set; }
        public PlayerModel Player { get; set; }
        public PartyModel Party { get; set; }

        public IGameAccessor Game { get; set; }

        public static MockEditFileSession FromPartyJson(string content, string mainCharacterId, IGameResourcesProvider resources)
        {
            var partyFile = new JsonPartSaveGameFile(JObject.Parse(content));
            var party = partyFile.GetRoot<PartyModel>();
            var playerFile = new JsonPartSaveGameFile(JObject.Parse(
                $"{{\"MainCharacterId\":\"{mainCharacterId}\",\"m_MainCharacter\":\"{mainCharacterId}\",\"Corruption\":{{}},\"SharedStash\":{{\"$id\":\"1\",\"m_Items\":[]}},\"m_QuestBook\":{{\"$id\":\"2\",\"Facts\":{{\"$id\":\"3\",\"m_Facts\":[]}}}}}}"));
            var session = new MockEditFileSession {
                Player = playerFile.GetRoot<PlayerModel>(),
                Party = partyFile.GetRoot<PartyModel>()
            };
            // These tests currently validate WotR-specific progression behavior.
            // Construct the WotR accessor directly to avoid ambiguous game detection.
            session.Game = new WotrGameAccessor(session);
            return session;
        }
    }
}

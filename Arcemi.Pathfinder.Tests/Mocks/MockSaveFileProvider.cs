using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.SaveGameEditor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Tests.Mocks
{
    public class MockSaveFileProvider : ISaveDataProvider
    {
        public PlayerModel Player { get; set; }
        public PartyModel Party { get; set; }
        public UnitEntityModel PlayerEntity { get; set; }
        public IEnumerable<UnitEntityModel> Characters { get; set; }

        public static MockSaveFileProvider FromPartyJson(string content, string mainCharacterId)
        {
            var partyFile = new JsonPartSaveGameFile(null, JObject.Parse(content), null);
            var party = partyFile.GetRoot<PartyModel>();
            return new MockSaveFileProvider {
                Characters = party.UnitEntities.Where(u => u.Descriptor?.UISettings != null).ToArray(),
                PlayerEntity = party.UnitEntities.FirstOrDefault(u => string.Equals(u.UniqueId, mainCharacterId, StringComparison.Ordinal))
            };
        }
    }
}

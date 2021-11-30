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
        public bool CanEdit => true;
        public PlayerModel Player { get; set; }
        public PartyModel Party { get; set; }
        public UnitEntityModel PlayerEntity { get; set; }
        public IEnumerable<UnitEntityModel> Characters { get; set; }

        public static MockSaveFileProvider FromPartyJson(string content, string mainCharacterId, IGameResourcesProvider resources)
        {
            var partyFile = new JsonPartSaveGameFile(null, JObject.Parse(content), resources);
            var party = partyFile.GetRoot<PartyModel>();
            return new MockSaveFileProvider {
                Characters = party.UnitEntities.Where(u => u.Descriptor?.UISettings != null).ToArray(),
                PlayerEntity = party.UnitEntities.FirstOrDefault(u => string.Equals(u.UniqueId, mainCharacterId, StringComparison.Ordinal))
            };
        }
    }
}

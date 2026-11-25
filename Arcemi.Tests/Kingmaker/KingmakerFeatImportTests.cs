using Arcemi.Models;
using Arcemi.Models.Kingmaker;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace Arcemi.Tests.Kingmaker
{
    public class KingmakerFeatImportTests
    {
        private const string WeaponFocus = "1e1f627d26ad36f43bbd26cc2bf8ac7e";

        [Fact]
        public void ExportedFeatKeepsRankAndParameterAndUsesReceivingCharacter()
        {
            var source = MakeGame("source", "3").MainCharacter;
            var feat = source.Feats.AddByBlueprint(FeatSpec.CreateIdentifier(WeaponFocus, "Longsword"));
            feat.Rank = 2;

            var recipient = MakeGame("recipient", "25").MainCharacter;
            var imported = recipient.Feats.AddByCode(feat.Export());

            Assert.Equal(WeaponFocus, imported.Blueprint);
            Assert.Equal("Longsword", imported.Category);
            Assert.Equal(2, imported.Rank);
            var fact = Assert.Single(recipient.Ref.Descriptor.Progression.Features.Facts);
            var saved = fact.GetAccessor().UnderlyingObject;
            Assert.Equal("25", (string)saved["Owner"]["$ref"]);
            Assert.Equal("25", (string)saved["m_Context"]["m_OwnerDescriptor"]["$ref"]);
            Assert.Equal("recipient", (string)saved["m_Context"]["m_CasterReference"]["m_UniqueId"]);
            Assert.Equal(WeaponFocus, (string)saved["m_Context"]["AssociatedBlueprint"]);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"Blueprint\":null}")]
        [InlineData("{\"Blueprint\":\"abc\",\"Param\":42}")]
        public void InvalidFeatCodeDoesNotInsertAFact(string json)
        {
            var recipient = MakeGame("recipient", "25").MainCharacter;
            Assert.Throws<FormatException>(() => recipient.Feats.AddByCode(CodeExport.ToCode(JObject.Parse(json))));
            Assert.Empty(recipient.Feats);
            Assert.Empty(recipient.Ref.Descriptor.Progression.Features.Facts);
        }

        private static KingmakerGameAccessor MakeGame(string unitId, string descriptorId)
        {
            const string header = """{"$id":"1"}""";
            var player = $$$"""
                {"$id":"1","m_MainCharacter":{"m_UniqueId":"{{{unitId}}}"},
                 "SharedStash":{"$id":"2","m_Items":[]},"m_Dialog":{"$id":"3","ShownCues":[]},
                 "m_QuestBook":{"$id":"4","PersistentQuests":[]}}
                """;
            var party = $$$"""
                {"$id":"1","m_EntityData":[
                    {"$id":"2","UniqueId":"{{{unitId}}}","Descriptor":{
                        "$id":"{{{descriptorId}}}","Blueprint":"4391e8b9afbb0cf43aeba700c089f56d",
                        "Stats":{"$id":"4"},"Progression":{"$id":"5","Classes":[],"Features":{"$id":"6","m_Facts":[]}},
                        "Abilities":{"$id":"7","m_Facts":[]},"Buffs":{"$id":"8","m_Facts":[]},
                        "m_Parts":{"$id":"9","m_Parts":[]},"Resources":{"$id":"10","PersistantResources":[]},
                        "m_Inventory":{"$id":"11","m_Items":[]}
                    }}
                ]}
                """;

            return new KingmakerGameAccessor(new GameFile(
                new JsonPartSaveGameFile(JObject.Parse(header)).GetRoot<HeaderModel>(),
                new JsonPartSaveGameFile(JObject.Parse(player)).GetRoot<PlayerModel>(),
                new JsonPartSaveGameFile(JObject.Parse(party)).GetRoot<PartyModel>()));
        }

        private sealed record GameFile(HeaderModel Header, PlayerModel Player, PartyModel Party) : IGameEditFile;
    }
}

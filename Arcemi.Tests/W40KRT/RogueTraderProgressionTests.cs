using Arcemi.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;

namespace Arcemi.Tests.W40KRT
{
    public class RogueTraderProgressionTests
    {
        [Fact]
        public void ChangingStarterChoicesRollsBackTheirGrantsAndKeepsIndependentAbilities()
        {
            const string origin = "45181a40472441a8904a5282f83693f4";
            const string priest = "d840a5dc947546e0b4ac939287191fd8";
            const string psyker = "1518d1434ed646039215da3fdda6b096";
            const string soldier = "567936698f674003bba05fa02e7130ae";
            const string soldierPath = "06f4f78a9c1a472b85cd79a9a142153d";
            const string soldierAscension = "7354c0ae9bf84c49adf82f95c05d4d5c";
            const string operative = "81c325968f774360a61536404a57c2ec";
            const string operativePath = "1529e5a0e7844bf3bb8d0cc0501264d4";
            const string warHymnFeat = "33638b28fa5e49748f4a7a5eed5ec035";
            const string warHymnAbility = "c647c1c4a1a14c9cb31217ee439164ff";
            using var file = new RogueTraderSave(new JArray(
                Choice(origin, 0, "ff001e095e7240ac99b40ceb2bdadf0a", priest),
                Choice(origin, 0, "f889a299ac7746cfb9b59438fc79018e", soldier),
                Choice(soldierPath, 2, "choice-one", "skill", 1),
                Choice(soldierPath, 2, "choice-two", "skill", 2)), new JArray(
                Feature(priest, "priest"),
                Feature(warHymnFeat, "hymn-feat", Grant("priest")),
                Ability(warHymnAbility, "hymn", Grant("hymn-feat")),
                Ability("shared", "shared", Grant("hymn-feat"), Grant("other")),
                Feature(soldier, "soldier"),
                Feature(soldierPath, "path", rank: 2),
                Feature(soldierAscension, "ascension", Grant("path")),
                Feature("skill", "skill", PathSource(soldierPath, 2, 1), 2, PathSource(soldierPath, 2, 2))
            ), level: 2);

            var unit = file.Document.Game.MainCharacter;
            var occupation = unit.Progression.Selections.Single(s => s.Name == "Occupation");
            occupation.Feature = psyker;
            Assert.DoesNotContain(unit.Feats, f => f.Blueprint == priest || f.Blueprint == warHymnFeat);
            Assert.DoesNotContain(unit.Abilities, a => a.Blueprint == warHymnAbility);
            Assert.Contains(unit.Abilities, a => a.Blueprint == "shared");
            Assert.Single(unit.Progression.Selections, s => s.Name == "Sanctioned Psyker");
            occupation.Feature = priest;
            Assert.DoesNotContain(unit.Progression.Selections, s => s.Name == "Sanctioned Psyker");
            occupation.Feature = psyker;
            Assert.Single(unit.Progression.Selections, s => s.Name == "Sanctioned Psyker");

            unit.Progression.Selections.Single(s => s.Feature == soldier).Feature = operative;
            Assert.Equal(1, unit.Progression.CurrentLevel);
            Assert.Equal(500, unit.Progression.Experience);
            Assert.DoesNotContain(unit.Feats, f => new[] { soldier, soldierPath, soldierAscension, "skill" }.Contains(f.Blueprint));
            Assert.Contains(unit.Feats, f => f.Blueprint == operativePath);
            Assert.DoesNotContain(unit.Progression.Selections, s => s.Level > 1);

            using var reopened = file.Reopen();
            var facts = reopened.Game.MainCharacter.Ref.Facts.Items;
            Assert.All(facts.GroupBy(f => f.UniqueId), group => Assert.Single(group));
            Assert.Single(facts.Single(f => f.Blueprint == "shared").GetAccessor().UnderlyingObject["m_Sources"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ForgeWorldSwitchWritesAnEquippedItemAndItsEffectsAndRemovesThemOnReversal(bool companion)
        {
            const string origin = "45181a40472441a8904a5282f83693f4";
            const string fortress = "b504a419af314de1ab85c86bc8b54907";
            const string forge = "6ba65fc6da1e40cf929d922f5b021a52";
            const string analytics = "96fd88d5d1fe4ba1a35746cde88aaf3c";
            const string locomotion = "49572d6dd2064c718231d60b31abf3f4";
            const string locomotionItem = "b986d0399c6e4165a43596b8101abf8c";
            const string locomotionEffect = "eda2be0dda7841ce9cce6186ef6e0ce1";
            using var file = new RogueTraderSave(new JArray(Choice(origin, 0, "10fefb03369d430e88b65aabaf68deac", fortress)),
                new JArray(Feature(fortress, "fortress")), level: 1, companion: companion);
            var game = file.Document.Game;
            var unit = game.Characters.Last();
            unit.Progression.Selections.Single().Feature = forge;
            Assert.Contains(unit.Feats, f => f.Blueprint == analytics);
            var augment = unit.Progression.Selections.Single(s => s.Name == "Forge World");
            augment.Feature = locomotion;
            Assert.DoesNotContain(unit.Feats, f => f.Blueprint == analytics);
            Assert.Contains(unit.Feats, f => f.Blueprint == locomotionEffect);
            Assert.Single(unit.Body.AdditionalSlots);
            Assert.Empty(game.SharedInventory.Sections.Single().Items);

            using var reopened = file.Reopen();
            var restored = reopened.Game.Characters.Last();
            var slot = Assert.Single(restored.Body.AdditionalSlots);
            var equippedId = slot.Slot.ItemRef;
            var item = reopened.Game.SharedInventory.FindEquipped(equippedId);
            Assert.Equal(locomotionItem, item.Blueprint);
            Assert.Contains(restored.Feats, f => f.Blueprint == locomotionEffect);
            restored.Progression.Selections.Single(s => s.Name == "Homeworld").Feature = fortress;
            Assert.DoesNotContain(restored.Feats, f => f.Blueprint == locomotionEffect || f.Blueprint == locomotion);
            Assert.Empty(restored.Body.AdditionalSlots);
            Assert.Null(reopened.Game.SharedInventory.FindEquipped(equippedId));
        }

        [Fact]
        public void DowngradeKeepsSharedObjectsWhenTheirFirstDefinitionWasInsideARemovedFact()
        {
            const string path = "06f4f78a9c1a472b85cd79a9a142153d";
            var temporary = Feature("temporary", "temporary");
            temporary["Data"] = JObject.Parse("""
                {"Item":{"$id":"20","Blueprint":"retained-item","UniqueId":"item",
                  "Abilities":[{"$id":"21","$type":"Kingmaker.UnitLogic.Abilities.Ability, Code","Blueprint":"retained-ability","UniqueId":"ability"}]}}
                """);
            using var file = new RogueTraderSave(
                new JArray(Choice(path, 2, "choice", "temporary")),
                new JArray(Feature(path, "career", rank: 2), temporary, new JObject { { "$ref", "21" } }),
                level: 2, inventory: new JArray(new JObject { { "$ref", "20" } }));

            file.Document.Game.MainCharacter.Progression.CurrentLevel = 1;
            using var reopened = file.Reopen();
            Assert.Contains(reopened.Game.MainCharacter.Abilities, a => a.Blueprint == "retained-ability");
            Assert.Contains(reopened.Game.SharedInventory.Sections.Single().Items, i => i.Blueprint == "retained-item");
        }

        private static JObject Choice(string path, int level, string id, string blueprint, int rank = 1) =>
            new JObject { { "Path", path }, { "Level", level }, { "Selection", id }, { "Feature", blueprint }, { "Rank", rank } };

        private static JObject Feature(string blueprint, string id, JObject source = null, int rank = 1, JObject second = null) =>
            new JObject {
                { "$type", "Kingmaker.UnitLogic.Feature, Code" }, { "Blueprint", blueprint }, { "UniqueId", id },
                { "Rank", rank }, { "m_Sources", new JArray(new[] { source, second }.Where(s => s != null)) }
            };

        private static JObject Ability(string blueprint, string id, params JObject[] sources) =>
            new JObject {
                { "$type", "Kingmaker.UnitLogic.Abilities.Ability, Code" }, { "Blueprint", blueprint },
                { "UniqueId", id }, { "m_Sources", new JArray(sources) }
            };

        private static JObject Grant(string fact) => new JObject {
            { "m_FactRef", new JObject { { "EntityId", "captain" }, { "FactId", fact } } }
        };

        private static JObject PathSource(string path, int level, int rank) => new JObject {
            { "m_Blueprint", path }, { "m_PathFeatureSource", path }, { "m_PathRank", level }, { "m_FeatureRank", rank }
        };

        private sealed class RogueTraderSave : System.IDisposable
        {
            private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory();

            public RogueTraderSave(JArray selections, JArray facts, int level, JArray inventory = null, bool companion = false)
            {
                var source = Path.Combine(_directory.FullName, "source.zks");
                using (var archive = ZipFile.Open(source, ZipArchiveMode.Create)) {
                    Write(archive, "header.json", """{"m_DlcRewards":[]}""");
                    Write(archive, "player.json", """
                        {"MainCharacter":"captain","StarSystemsState":{},"FractionsReputation":[],"m_QuestBook":{"Facts":{"m_Facts":[]}}}
                        """);
                    var party = JObject.Parse("""
                        {"m_EntityData":[{"UniqueId":"captain","Blueprint":"captain-blueprint","Facts":{"m_Facts":[]},"Parts":{"Container":[
                          {"$type":"Kingmaker.UnitLogic.PartUnitProgression, Code","Experience":500},
                          {"$type":"Kingmaker.UnitLogic.Parts.PartInventory, Code","CollectionConverter":{"m_Items":[]}},
                          {"$type":"Kingmaker.Items.PartUnitBody, Code","m_HandsEquipmentSets":[],"m_QuickSlots":[]}
                        ]}}]}
                        """);
                    var units = (JArray)party["m_EntityData"];
                    var unit = units[0];
                    if (inventory is not null) {
                        unit["Parts"]["Container"][1]["CollectionConverter"]["m_Items"] = inventory;
                    }
                    if (companion) {
                        unit = unit.DeepClone();
                        unit["UniqueId"] = "companion";
                        ((JArray)unit["Parts"]["Container"]).RemoveAt(1);
                        units.Add(unit);
                    }
                    unit["Facts"]["m_Facts"] = facts;
                    var progression = unit["Parts"]["Container"][0];
                    progression["CharacterLevel"] = level;
                    progression["m_Selections"] = selections;
                    Write(archive, "party.json", party.ToString());
                }
                Document = new SaveGameDocument(source);
            }

            public SaveGameDocument Document { get; }

            public SaveGameDocument Reopen()
            {
                var target = Path.Combine(_directory.FullName, "target.zks");
                Document.Save(target, target + ".bck");
                return new SaveGameDocument(target);
            }

            public void Dispose()
            {
                Document.Dispose();
                _directory.Delete(recursive: true);
            }

            private static void Write(ZipArchive archive, string name, string content)
            {
                using var writer = new StreamWriter(archive.CreateEntry(name).Open());
                writer.Write(content);
            }
        }
    }
}

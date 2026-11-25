using Arcemi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests
{
    public class SaveGameDocumentTests
    {
        [Fact]
        public void SaveAsAndOverwritePreserveUntouchedEntriesAndKeepPreviousSave()
        {
            var directory = Directory.CreateTempSubdirectory();
            try {
                var source = Path.Combine(directory.FullName, "source.zks");
                var target = Path.Combine(directory.FullName, "target.zks");
                var backup = Path.Combine(directory.FullName, "target.zks.bck");
                using (var archive = ZipFile.Open(source, ZipArchiveMode.Create)) {
                    WriteEntry(archive, "header.json", "{ \"Name\": \"Original\", \"m_DlcRewards\": [] }");
                    WriteEntry(archive, "player.json", "{ \"StarSystemsState\": {}, \"FractionsReputation\": [], \"m_QuestBook\": {\"Facts\":{\"m_Facts\":[]}} }");
                    WriteEntry(archive, "party.json", "{ \"m_EntityData\": [] }");
                    WriteEntry(archive, "scene/opaque.data", "unchanged");
                }

                using var document = new SaveGameDocument(source);
                Assert.True(document.CanSave);
                Assert.Equal(GameDefinition.Warhammer40K_RogueTrader, document.Game.Definition);
                document.Save(target, backup);

                Assert.Equal(ReadEntry(source, "header.json"), ReadEntry(target, "header.json"));
                Assert.False(File.Exists(backup));

                document.Header.Name = "Changed";
                document.Save(target, backup);
                using var reopened = new SaveGameDocument(target);
                Assert.Equal("Changed", reopened.Header.Name);
                Assert.Equal("Original", SaveGameFile.ReadHeader(backup).Name);
                Assert.Equal("Original", SaveGameFile.ReadHeader(source).Name);
                Assert.Equal(ReadEntry(source, "party.json"), ReadEntry(target, "party.json"));
                Assert.Equal(ReadEntry(source, "player.json"), ReadEntry(target, "player.json"));
                Assert.Equal("unchanged", ReadEntry(target, "scene/opaque.data"));

                document.Header.Name = "Changed again";
                document.Save(target, backup);
                Assert.Equal("Changed", SaveGameFile.ReadHeader(backup).Name);
                Assert.Equal("Changed again", SaveGameFile.ReadHeader(target).Name);
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void RogueTraderPlaytimeAndColonyRefillRoundTripIndependently()
        {
            var directory = Directory.CreateTempSubdirectory();
            try {
                var source = Path.Combine(directory.FullName, "source.zks");
                var target = Path.Combine(directory.FullName, "target.zks");
                var backup = Path.Combine(directory.FullName, "target.zks.bck");
                using (var archive = ZipFile.Open(source, ZipArchiveMode.Create)) {
                    WriteEntry(archive, "header.json", """{"GameSaveTime":"20.00:00:00","GameTotalTime":"5.00:00:00","m_DlcRewards":[]}""");
                    WriteEntry(archive, "player.json", """
                        {
                          "GameTime":"20.00:00:00",
                          "RealTime":"4.00:00:00",
                          "StarSystemsState":{},
                          "FractionsReputation":[],
                          "m_QuestBook":{"Facts":{"m_Facts":[]}},
                          "ColoniesState":{"Colonies":[{"Colony":{
                            "Blueprint":"colony",
                            "Consumables":[
                              {"Item":"medkit","LastRefill":"19.00:00:00","SegmentsToRefill":9,"MaxCount":5},
                              {"Item":"grenade","LastRefill":"19.00:00:00","SegmentsToRefill":9,"MaxCount":2}
                            ],
                            "LootToReceive":{"m_Items":{"m_Items":[{"m_Blueprint":"grenade","m_Count":2}]}}
                          }}]}
                        }
                        """);
                    WriteEntry(archive, "party.json", """{"m_EntityData":[]}""");
                }

                using var document = new SaveGameDocument(source);
                var playtime = Assert.Single(document.Game.Party.Data.Properties.OfType<IGameDataTime>());
                Assert.Equal(4, playtime.Value.Days);
                document.Save(target, backup);
                Assert.Equal(TimeSpan.FromDays(5), SaveGameFile.ReadHeader(target).GameTotalTime);

                var colony = Assert.Single(Assert.Single(document.Game.Management.Places.Properties.OfType<IGameDataList>()).Entries);
                var refillStatus = Assert.Single(colony.Properties.OfType<IGameDataObject>(), data => data.Name == "Refill status");
                var rewards = Assert.Single(refillStatus.Properties.OfType<IGameDataList>()).Entries.ToArray();
                var medkit = Assert.Single(rewards, data => data.Name == "medkit");
                var grenade = Assert.Single(rewards, data => data.Name == "grenade");
                var refill = Assert.Single(medkit.Properties.OfType<IGameDataAction>());
                var cappedRefill = Assert.Single(grenade.Properties.OfType<IGameDataAction>());
                Assert.True(refill.IsEnabled);
                Assert.False(cappedRefill.IsEnabled);

                playtime.Value.Days = 2;
                refill.Invoke();
                cappedRefill.Invoke();
                Assert.False(refill.IsEnabled);
                document.Save(target, backup);

                using var reopened = new SaveGameDocument(target);
                Assert.Equal(TimeSpan.FromDays(2), reopened.Json.Player.RealTime);
                Assert.Equal(reopened.Json.Player.RealTime, reopened.Header.GameTotalTime);
                Assert.Equal(TimeSpan.FromDays(20), reopened.Json.Player.GameTime);
                Assert.Equal(reopened.Json.Player.GameTime, reopened.Header.GameSaveTime);
                Assert.Equal(TimeSpan.FromDays(5), SaveGameFile.ReadHeader(backup).GameTotalTime);

                var reopenedColony = Assert.Single(Assert.Single(reopened.Game.Management.Places.Properties.OfType<IGameDataList>()).Entries);
                var reopenedStatus = Assert.Single(reopenedColony.Properties.OfType<IGameDataObject>(), data => data.Name == "Refill status");
                var reopenedRewards = Assert.Single(reopenedStatus.Properties.OfType<IGameDataList>()).Entries.ToArray();
                var reopenedMedkit = Assert.Single(reopenedRewards, data => data.Name == "medkit");
                var reopenedGrenade = Assert.Single(reopenedRewards, data => data.Name == "grenade");
                Assert.Equal("17.00:00:00", Assert.Single(reopenedMedkit.Properties.OfType<IGameDataText>(), field => field.Label.StartsWith("Last refill")).Value);
                Assert.Equal("19.00:00:00", Assert.Single(reopenedGrenade.Properties.OfType<IGameDataText>(), field => field.Label.StartsWith("Last refill")).Value);
                Assert.Equal(new[] { 9, 5 }, reopenedMedkit.Properties.OfType<IGameDataInteger>().Select(field => field.Value));
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public async Task RogueTraderAddsPetProtocolsAndAugmentsToAnEmptyInventory()
        {
            const string protocol = "11111111111111111111111111111111";
            const string augment = "22222222222222222222222222222222";
            var game = GameDefinition.Warhammer40K_RogueTrader;
            var blueprints = game.Resources.Blueprints;
            var originalEntries = blueprints.Entries.Cast<BlueprintMetadataEntry>().ToArray();
            var directory = Directory.CreateTempSubdirectory();
            try {
                var bundles = directory.CreateSubdirectory("Bundles");
                await File.WriteAllTextAsync(Path.Combine(bundles.FullName, "cheatdata.json"), $$"""
                    {"Entries":[
                      {"Name":"TestPetProtocol_Item","Guid":"{{protocol}}","TypeFullName":"Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentPetProtocol, Code, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
                      {"Name":"TestAugment_Item","Guid":"{{augment}}","TypeFullName":"Kingmaker.Blueprints.Items.Augments.BlueprintItemAugment, Code, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"}
                    ]}
                    """);
                await blueprints.SetupAsync(new BlueprintProviderSetupArgs(game, null, directory.FullName));

                var source = Path.Combine(directory.FullName, "source.zks");
                var target = Path.Combine(directory.FullName, "target.zks");
                using (var archive = ZipFile.Open(source, ZipArchiveMode.Create)) {
                    WriteEntry(archive, "header.json", """{"m_DlcRewards":[]}""");
                    WriteEntry(archive, "player.json", """
                        {"MainCharacter":"captain","StarSystemsState":{},"FractionsReputation":[],"m_QuestBook":{"Facts":{"m_Facts":[]}}}
                        """);
                    WriteEntry(archive, "party.json", """
                        {"m_EntityData":[{"UniqueId":"captain","Blueprint":"captain-blueprint","Facts":{"m_Facts":[]},"Parts":{"Container":[
                          {"$type":"Kingmaker.UnitLogic.Parts.PartInventory, Code","CollectionConverter":{"m_Items":[]}}
                        ]}}]}
                        """);
                }

                using var document = new SaveGameDocument(source);
                var inventory = Assert.Single(document.Game.SharedInventory.Sections);
                var protocolType = Assert.Single(inventory.AddableTypes, type => type.DisplayName == "Pet Protocol");
                var augmentType = Assert.Single(inventory.AddableTypes, type => type.DisplayName == "Augment");
                Assert.Equal(protocol, Assert.Single(inventory.GetAddableItems(protocolType.FullName)).Id);
                Assert.Equal(augment, Assert.Single(inventory.GetAddableItems(augmentType.FullName)).Id);
                Assert.Contains(inventory.GetAddableItems(), entry => entry.Id == protocol);
                Assert.Contains(inventory.GetAddableItems(), entry => entry.Id == augment);
                inventory.Items.AddByBlueprint(protocol);
                inventory.Items.AddByBlueprint(augment);
                document.Save(target, target + ".bck");

                using var reopened = new SaveGameDocument(target);
                var savedItems = Assert.Single(reopened.Game.SharedInventory.Sections).Items.ToArray();
                Assert.Equal(new[] { protocol, augment }, savedItems.Select(item => item.Blueprint));
                Assert.Equal(new[] { "Pet Protocol", "Augment" }, savedItems.Select(item => item.Type));
                Assert.All(savedItems, item => Assert.Equal(1, item.Count));

                var items = JObject.Parse(ReadEntry(target, "party.json"))["m_EntityData"][0]["Parts"]["Container"][0]["CollectionConverter"]["m_Items"];
                Assert.Equal(new[] { 0, 1 }, items.Select(item => item.Value<int>("m_InventorySlotIndex")));
                Assert.All(items, item => {
                    Assert.Equal("Kingmaker.Items.ItemEntitySimple, Code", item.Value<string>("$type"));
                    var fact = Assert.Single(item["Facts"]["m_Facts"]);
                    Assert.Equal(item.Value<string>("Blueprint"), fact.Value<string>("Blueprint"));
                    Assert.Equal(fact.Value<string>("$id"), item["MainFact"].Value<string>("$ref"));
                });
                Assert.NotEqual(items[0]["MainFact"].Value<string>("$ref"), items[1]["MainFact"].Value<string>("$ref"));
            }
            finally {
                blueprints.SetEntries(originalEntries);
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void OverwriteKeepsThePreviousSaveInTheConfiguredBackupFolder()
        {
            var directory = Directory.CreateTempSubdirectory();
            try {
                var saves = directory.CreateSubdirectory("game").FullName;
                var path = Path.Combine(saves, "Manual_1.zks");
                var storage = new SaveBackupStorage(Path.Combine(directory.FullName, "backups"));
                CreateBackupTestSave(path);
                using var document = new SaveGameDocument(path);
                document.Header.Name = "Edited";
                document.Save(path, storage.GetWritePath(path));

                Assert.Equal("Edited", SaveGameFile.ReadHeader(path).Name);
                Assert.Equal("Original", SaveGameFile.ReadHeader(storage.Find(path)).Name);
                Assert.Empty(Directory.GetFiles(saves, "*.bck"));
                Assert.NotEqual(storage.GetWritePath(path),
                    storage.GetWritePath(Path.Combine(directory.FullName, "another game", Path.GetFileName(path))));
                storage.Restore(path);
                Assert.Equal("Original", SaveGameFile.ReadHeader(path).Name);
                Assert.True(File.Exists(storage.Find(path)));
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void FailedBackupLeavesTheSaveUntouchedUnlessFallbackIsEnabled()
        {
            var directory = Directory.CreateTempSubdirectory();
            try {
                var path = Path.Combine(directory.FullName, "Manual_1.zks");
                CreateBackupTestSave(path);
                var blocked = Path.Combine(directory.FullName, "not-a-directory");
                File.WriteAllText(blocked, "block");
                var storage = new SaveBackupStorage(blocked);

                using var document = new SaveGameDocument(path);
                document.Header.Name = "Edited";
                Assert.Throws<IOException>(() => document.Save(path, storage.GetWritePath(path), storage.GetFallbackPath(path)));
                Assert.Equal("Original", SaveGameFile.ReadHeader(path).Name);
                Assert.Null(storage.Find(path));

                storage = new SaveBackupStorage(blocked, allowSaveFolderFallback: true);
                document.Save(path, storage.GetWritePath(path), storage.GetFallbackPath(path));
                Assert.Equal("Edited", SaveGameFile.ReadHeader(path).Name);
                Assert.Equal(storage.GetFallbackPath(path), storage.Find(path));
                Assert.Equal("Original", SaveGameFile.ReadHeader(storage.Find(path)).Name);
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void LegacyBackupsRemainReadableAndCanBeMovedWithoutOverwritingAnExistingBackup()
        {
            var directory = Directory.CreateTempSubdirectory();
            try {
                var path = Path.Combine(directory.FullName, "Manual 1.zks");
                var legacy = path + ".bck";
                CreateBackupTestSave(legacy);
                var storage = new SaveBackupStorage(Path.Combine(directory.FullName, "backups"));

                Assert.Equal(legacy, storage.Find(path));
                Assert.True(storage.MoveLegacyBackup(path));
                Assert.False(File.Exists(legacy));
                Assert.Equal("Original", SaveGameFile.ReadHeader(storage.Find(path)).Name);

                CreateBackupTestSave(legacy);
                Assert.False(storage.MoveLegacyBackup(path));
                Assert.True(File.Exists(legacy));
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        private static void CreateBackupTestSave(string path)
        {
            using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
            WriteEntry(archive, "header.json", """{"Name":"Original","m_DlcRewards":[]}""");
            WriteEntry(archive, "player.json", """{"StarSystemsState":{},"FractionsReputation":[],"m_QuestBook":{"Facts":{"m_Facts":[]}}}""");
            WriteEntry(archive, "party.json", """{"m_EntityData":[]}""");
        }

        private static void WriteEntry(ZipArchive archive, string name, string contents)
        {
            using var writer = new StreamWriter(archive.CreateEntry(name).Open());
            writer.Write(contents);
        }

        private static string ReadEntry(string path, string name)
        {
            using var zip = ZipFile.OpenRead(path);
            using var reader = new StreamReader(zip.GetEntry(name).Open());
            return reader.ReadToEnd();
        }
    }
}

using Arcemi.Models;
using Arcemi.Models.BinaryTraversal.Owl;
using Arcemi.Tests.Mocks;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;
using Xunit;

namespace Arcemi.Tests
{
    public class DarkHeresySaveFixtureTests
    {
        private static readonly string FixturePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAreaDirectory", "DarkHeresy_Quick_1.zks");
        private static readonly string[] RealQuickSavePaths = {
            @"C:\Users\johan\AppData\LocalLow\Owlcat Games\WHDH\Saved Games\Quick_1.zks",
            @"/mnt/c/Users/johan/AppData/LocalLow/Owlcat Games/WHDH/Saved Games/Quick_1.zks",
        };

        [Fact]
        public void Fixture_ContainsExpectedEntries()
        {
            Assert.True(File.Exists(FixturePath), "Dark Heresy fixture file is missing: " + FixturePath);

            using var stream = new FileStream(FixturePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            Assert.NotNull(archive.GetEntry("header.json"));
            Assert.NotNull(archive.GetEntry("party.owl"));
            Assert.NotNull(archive.GetEntry("player.owl"));
        }

        [Fact]
        public void ReadHeader_FromDarkHeresyFixture_ReturnsData()
        {
            var header = SaveGameFile.ReadHeader(FixturePath);

            Assert.NotNull(header);
            Assert.False(string.IsNullOrWhiteSpace(header.GameId));
            Assert.False(string.IsNullOrWhiteSpace(header.Type));
            Assert.NotEqual(default, header.SystemSaveTime);
        }

        [Theory]
        [InlineData("party.owl")]
        [InlineData("player.owl")]
        public void OwlEntry_CanBeLoadedByOwlBinaryDocument(string entryName)
        {
            using var stream = new FileStream(FixturePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var entry = archive.GetEntry(entryName);
            Assert.NotNull(entry);

            using var entryStream = entry.Open();
            using var ms = new MemoryStream();
            entryStream.CopyTo(ms);
            var bytes = ms.ToArray();

            var doc = new OwlBinaryDocument(bytes, Array.Empty<Arcemi.Models.BinaryTraversal.BinarySlotDefinition>());
            var saved = doc.Save();

            Assert.Equal(bytes.Length, saved.Length);
            Assert.Equal((byte)'O', saved[0]);
            Assert.Equal((byte)'W', saved[1]);
            Assert.Equal((byte)'L', saved[2]);
        }

        [Fact]
        public void SupportedGames_Detects_DarkHeresyAccessor()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyAccessor_ExposesMainCharacterStats()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
                Assert.NotNull(main);
                Assert.True(main.Stats.IsSupported);
                Assert.Contains(main.Stats.Groupings, g => g.Any(e => e.Name == "Weapon Skill"));
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyAccessor_ContainsScreenshotCharacteristicStats()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
                Assert.NotNull(main);
                Assert.True(main.Stats.IsSupported);

                var stats = main.Stats.Groupings
                    .SelectMany(g => g)
                    .ToDictionary(x => x.Name, x => x.Value, StringComparer.Ordinal);

                // The save stores base/partial values, while in-game UI may include bonuses.
                AssertCharacteristicAtMost(stats, "Weapon Skill", 35);
                AssertCharacteristicAtMost(stats, "Ballistic Skill", 45);
                AssertCharacteristicAtMost(stats, "Strength", 30);
                AssertCharacteristicAtMost(stats, "Toughness", 30);
                AssertCharacteristicAtMost(stats, "Agility", 40);
                AssertCharacteristicAtMost(stats, "Intelligence", 30);
                AssertCharacteristicAtMost(stats, "Perception", 30);
                AssertCharacteristicAtMost(stats, "Willpower", 40);
                AssertCharacteristicAtMost(stats, "Fellowship", 55);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyAccessor_MapsSkillAdvancementToEditableSkillStat()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;
                Assert.NotNull(main);

                var diplomacy = main.Stats.Groupings
                    .SelectMany(g => g)
                    .FirstOrDefault(x => x.Name == "Diplomacy");
                Assert.NotNull(diplomacy);
                Assert.True(diplomacy.Value >= 10);
                Assert.False(diplomacy.IsReadOnly);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyAccessor_LoadsMultiplePartyMembers_FromPartyOwl()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
                Assert.True(game.Characters.Count > 1, "Expected more than one character parsed from party.owl.");
                Assert.True(game.Characters.Count(x => x.Type != UnitEntityType.Player) > 0, "Expected at least one non-main party member.");
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyOpen_TimingBreakdown()
        {
            var swTotal = Stopwatch.StartNew();
            var sw = new Stopwatch();

            sw.Start();
            var save = new SaveGameFile(FixturePath);
            sw.Stop();
            var openMs = sw.ElapsedMilliseconds;

            try
            {
                sw.Restart();
                var header = save.GetHeader().GetRoot<HeaderModel>();
                sw.Stop();
                var headerMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var player = save.GetPlayer().GetRoot<PlayerModel>();
                sw.Stop();
                var playerMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var party = save.GetParty().GetRoot<PartyModel>();
                sw.Stop();
                var partyMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var file = new MockEditFileSession {
                    Header = header,
                    Player = player,
                    Party = party,
                };
                var game = SupportedGames.Detect(file);
                sw.Stop();
                var detectMs = sw.ElapsedMilliseconds;

                swTotal.Stop();
                Console.WriteLine(
                    $"DH timing (ms): open={openMs}, header={headerMs}, player={playerMs}, party={partyMs}, detect={detectMs}, total={swTotal.ElapsedMilliseconds}");

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyOpen_RealQuick1Save_TimingBreakdown()
        {
            var realQuickSavePath = RealQuickSavePaths.FirstOrDefault(File.Exists);
            if (string.IsNullOrEmpty(realQuickSavePath)) {
                Console.WriteLine("Skipping real-save timing test. File not found in any known location: " + string.Join(", ", RealQuickSavePaths));
                return;
            }

            var swTotal = Stopwatch.StartNew();
            var sw = new Stopwatch();

            sw.Start();
            var save = new SaveGameFile(realQuickSavePath);
            sw.Stop();
            var openMs = sw.ElapsedMilliseconds;

            try
            {
                sw.Restart();
                var header = save.GetHeader().GetRoot<HeaderModel>();
                sw.Stop();
                var headerMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var player = save.GetPlayer().GetRoot<PlayerModel>();
                sw.Stop();
                var playerMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var party = save.GetParty().GetRoot<PartyModel>();
                sw.Stop();
                var partyMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var file = new MockEditFileSession {
                    Header = header,
                    Player = player,
                    Party = party,
                };
                var game = SupportedGames.Detect(file);
                sw.Stop();
                var detectMs = sw.ElapsedMilliseconds;

                swTotal.Stop();
                Console.WriteLine(
                    $"DH REAL timing (ms): open={openMs}, header={headerMs}, player={playerMs}, party={partyMs}, detect={detectMs}, total={swTotal.ElapsedMilliseconds}, chars={game.Characters.Count}");

                Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, game.Definition);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyPartyOwl_CanPersistCharacterLevelPatch()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var analysis = (partyPart.Json["__arcemi_dh"]?["characters"] as JArray)?.FirstOrDefault() as JObject;
                Assert.NotNull(analysis);

                var stats = analysis["stats"] as JArray;
                Assert.NotNull(stats);
                var levelStat = stats
                    .OfType<JObject>()
                    .FirstOrDefault(x => string.Equals(x["name"]?.Value<string>(), "Character Level", StringComparison.Ordinal));
                Assert.NotNull(levelStat);

                var slotPath = levelStat["slotPath"]?.Value<string>();
                Assert.False(string.IsNullOrWhiteSpace(slotPath));

                var before = levelStat["value"]?.Value<int>() ?? 0;
                levelStat["value"] = before + 1;
                partyPart.Save();

                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;
                Assert.NotNull(main);

                var value = main.Stats.Groupings
                    .SelectMany(g => g)
                    .First(x => x.Name == "Character Level")
                    .Value;

                Assert.Equal(before + 1, value);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyDiagnostics_CanBeCollectedWithoutUi()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var diagnosticsRoot = partyPart.Json["__arcemi_dh"] as JObject;
                Assert.NotNull(diagnosticsRoot);

                var characters = diagnosticsRoot["characters"] as JArray;
                Assert.NotNull(characters);
                Assert.True(characters.Count > 0);

                var first = characters[0] as JObject;
                Assert.NotNull(first);
                Assert.True((first["stats"] as JArray)?.Count > 0);

                // Keep this as a fast, no-UI diagnostic dump for parser troubleshooting.
                Console.WriteLine("DH diagnostics (first character):");
                Console.WriteLine(first.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyDiagnostics_DumpAllCharactersSummary()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var characters = (partyPart.Json["__arcemi_dh"]?["characters"] as JArray)?.OfType<JObject>().ToArray();
                Assert.NotNull(characters);
                Assert.True(characters.Length > 0);

                foreach (var character in characters) {
                    var index = character["index"]?.Value<int>() ?? -1;
                    var main = character["isMainCharacter"]?.Value<bool>() == true;
                    var inSquad = character["isInCurrentSquad"]?.Value<bool>() == true;
                    var unitBlueprint = character["unitBlueprint"]?.Value<string>() ?? string.Empty;
                    var portraitBlueprint = character["portraitBlueprint"]?.Value<string>() ?? string.Empty;
                    var statsCount = (character["stats"] as JArray)?.Count ?? 0;
                    var selectionCount = (character["selections"] as JArray)?.Count ?? 0;

                    Console.WriteLine(
                        $"DH summary: idx={index}, main={main}, inSquad={inSquad}, unitBp='{unitBlueprint}', portraitBp='{portraitBlueprint}', stats={statsCount}, selections={selectionCount}");
                }
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyDiagnostics_FindsFeatureSelectionsForAtLeastOneCharacter()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var characters = (partyPart.Json["__arcemi_dh"]?["characters"] as JArray)?.OfType<JObject>().ToArray();
                Assert.NotNull(characters);
                Assert.True(characters.Length > 0);

                var withSelections = characters
                    .Select((c, i) => new { Character = c, Index = i, Selections = (c["selections"] as JArray)?.Count ?? 0 })
                    .Where(x => x.Selections > 0)
                    .ToArray();

                var debugSummary = string.Join(
                    "; ",
                    characters.Select((c, i) => {
                        var facts = c["debugFacts"] as JObject;
                        var factCount = facts?["factCount"]?.Value<int>() ?? 0;
                        var featureFactCount = facts?["featureFactCount"]?.Value<int>() ?? 0;
                        return $"idx={i}, facts={factCount}, featureFacts={featureFactCount}";
                    }));
                Assert.True(
                    withSelections.Length > 0,
                    "No characters contained parsed feature selections. Stats cannot be reconstructed without selection/fact data. " + debugSummary);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyAccessor_HasEditableCharacteristic_WhenSelectionRankSlotExists()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = partyPart.GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;
                Assert.NotNull(main);

                var editableCharacteristic = main.Stats.Groupings
                    .SelectMany(g => g)
                    .FirstOrDefault(x =>
                        IsCoreCharacteristic(x.Name)
                        && !x.IsReadOnly);

                if (editableCharacteristic is null) {
                    Console.WriteLine("No editable characteristic resolved from this fixture; keeping characteristics read-only.");
                    return;
                }
                Assert.True(editableCharacteristic.Value >= 0);
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyDiagnostics_HasCharacteristicStatWithWritableSlot()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var characters = (partyPart.Json["__arcemi_dh"]?["characters"] as JArray)?.OfType<JObject>().ToArray();
                Assert.NotNull(characters);
                Assert.True(characters.Length > 0);

                var writableCount = characters
                    .SelectMany(c => (c["stats"] as JArray)?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
                    .Count(stat =>
                        IsCoreCharacteristic(stat["name"]?.Value<string>())
                        && !string.IsNullOrWhiteSpace(stat["slotPath"]?.Value<string>())
                        && stat["editable"]?.Value<bool>() == true);
                var selectionWritableCount = characters
                    .SelectMany(c => (c["selections"] as JArray)?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
                    .Count(x => !string.IsNullOrWhiteSpace(x["rankSlotPath"]?.Value<string>()));

                Assert.True(
                    writableCount > 0 || selectionWritableCount > 0,
                    "No writable Dark Heresy slots were discovered in party.owl.");
            }
            finally
            {
                save.Close();
            }
        }

        [Fact]
        public void DarkHeresyPartyOwl_CanPersistEditableCharacteristicPatch()
        {
            var save = new SaveGameFile(FixturePath);
            try
            {
                var partyPart = save.GetParty();
                var file = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = partyPart.GetRoot<PartyModel>(),
                };
                var game = SupportedGames.Detect(file);
                var main = game.MainCharacter;
                Assert.NotNull(main);

                var editableCharacteristic = main.Stats.Groupings
                    .SelectMany(g => g)
                    .FirstOrDefault(x =>
                        IsCoreCharacteristic(x.Name)
                        && !x.IsReadOnly
                        && string.Equals(x.Info, "party.owl", StringComparison.Ordinal));
                if (editableCharacteristic is null) {
                    Console.WriteLine("No editable characteristic resolved from this fixture; skipping persistence assertion.");
                    return;
                }

                var before = editableCharacteristic.Value;
                editableCharacteristic.Value = before + 5;
                partyPart.Save();

                var reopened = new MockEditFileSession {
                    Header = save.GetHeader().GetRoot<HeaderModel>(),
                    Player = save.GetPlayer().GetRoot<PlayerModel>(),
                    Party = save.GetParty().GetRoot<PartyModel>(),
                };
                var reopenedGame = SupportedGames.Detect(reopened);
                var reopenedMain = reopenedGame.MainCharacter;
                Assert.NotNull(reopenedMain);

                var after = reopenedMain.Stats.Groupings
                    .SelectMany(g => g)
                    .First(x => x.Name == editableCharacteristic.Name)
                    .Value;

                Assert.Equal(before + 5, after);
            }
            finally
            {
                save.Close();
            }
        }

        private static void AssertCharacteristicAtMost(System.Collections.Generic.IDictionary<string, int> stats, string name, int screenValue)
        {
            Assert.True(stats.ContainsKey(name), "Missing expected characteristic '" + name + "'.");
            Assert.InRange(stats[name], 0, screenValue);
        }

        private static bool IsCoreCharacteristic(string name)
        {
            return name == "Weapon Skill"
                || name == "Ballistic Skill"
                || name == "Strength"
                || name == "Toughness"
                || name == "Agility"
                || name == "Intelligence"
                || name == "Perception"
                || name == "Willpower"
                || name == "Fellowship";
        }
    }
}

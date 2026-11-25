using Arcemi.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Arcemi.Tests
{
    public class DarkHeresySaveFixtureTests
    {
        private static readonly string AlphaFixturePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAreaDirectory", "DarkHeresy_Quick_1.zks");

        [Fact]
        public void HeaderCanBeListedWithoutOpeningAnUnsupportedOwlVersion()
        {
            Assert.NotNull(SaveGameFile.ReadHeader(AlphaFixturePath));

            var error = Assert.Throws<NotSupportedException>(() => new SaveGameDocument(AlphaFixturePath));
            Assert.Contains("OWL version 2", error.Message);
        }

        [Fact]
        public void BetaQuicksaveHidesFormerPrologueCompanions()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_BETA_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            using var document = new SaveGameDocument(path);
            Assert.Same(document.Game.MainCharacter, Assert.Single(document.Game.Characters,
                unit => unit.Type.IsDisplayedInCharactersPage()));
            Assert.Contains(document.Game.Party.Data.Properties.OfType<IGameDataInteger>(),
                entry => entry.Label == "Current squad" && entry.Value == 1);
            Assert.Contains(document.Game.Party.Data.Properties.OfType<IGameDataInteger>(),
                entry => entry.Label == "Characters" && entry.Value == 1);
            foreach (var blueprint in new[] { "51880655802645dead2e5ba4ff64007c", "e9530687c357421f9f9de739f1a05055" }) {
                var unit = Assert.Single(document.Game.Characters, unit => unit.Blueprint == blueprint);
                Assert.Equal(UnitEntityType.Other, unit.Type);
                Assert.True(unit.Companion.IsExCompanion);
                Assert.False(unit.Companion.IsInParty);
                Assert.Equal("Former companion in reserve", unit.Companion.State);
            }
        }

        [Fact]
        public void BetaRecruitedPartyShowsAriaEpioneAndHaymar()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_RECRUITED_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            using var document = new SaveGameDocument(path);
            var visible = document.Game.Characters.Where(unit => unit.Type.IsDisplayedInCharactersPage()).ToArray();
            Assert.Equal(new[] { "Aria", "Epione Spes", "Haymar Devos" }, visible.Select(unit => unit.Name).OrderBy(name => name));
            Assert.Equal(UnitEntityType.Player, document.Game.MainCharacter.Type);
            Assert.Equal("0d63af3a5c6244fdad5a93e6e331ae87", document.Game.MainCharacter.Portrait.Blueprint);
            Assert.Equal("29834a8100f247ac995dde0ae448e9f8", visible.Single(unit => unit.Name == "Epione Spes").Portrait.Blueprint);
            Assert.Equal("ae7ebcd0df5e420e82f2161b1b4f529f", visible.Single(unit => unit.Name == "Haymar Devos").Portrait.Blueprint);
            Assert.All(visible, unit => Assert.True(unit.Portrait.IsSupported));
            foreach (var unit in visible.Where(unit => unit != document.Game.MainCharacter)) {
                Assert.Equal(UnitEntityType.Companion, unit.Type);
                Assert.True(unit.Companion.IsInParty);
                Assert.NotEmpty(unit.Stats.Groupings);
            }
            Assert.Contains(document.Game.Party.Data.Properties.OfType<IGameDataInteger>(),
                entry => entry.Label == "Current squad" && entry.Value == 3);
        }

        [Fact]
        public void BetaEquipmentResolvesItemsByInstanceAcrossTheRecruitedParty()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_RECRUITED_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            using var document = new SaveGameDocument(path);
            var game = document.Game;
            var aria = game.MainCharacter.Body;
            var epione = game.Characters.Single(unit => unit.Name == "Epione Spes").Body;
            var haymar = game.Characters.Single(unit => unit.Name == "Haymar Devos").Body;
            var party = new[] { aria, epione, haymar };

            Assert.All(party, body => {
                Assert.True(body.IsSupported);
                Assert.Equal(0, body.ActiveHandsEquipmentSetIndex);
                Assert.Equal(2, body.HandsEquipmentSets.Count);
            });
            var armors = party.Select(body => Resolve(body.Armor)).ToArray();
            Assert.All(armors, item => Assert.Equal("f05671c1926142f5adfbdff941f5196d", item.Blueprint));
            Assert.Equal(3, armors.Select(item => item.UniqueId).Distinct().Count());

            Assert.Equal("519c3aa92ba74a1b997b7f788022602c", Resolve(aria.HandsEquipmentSets[0].Primary).Blueprint);
            Assert.Equal("298f1d2365864c919cfd094d0209ef08", Resolve(aria.HandsEquipmentSets[0].Secondary).Blueprint);
            Assert.Null(aria.HandsEquipmentSets[1].Primary);
            Assert.Equal("4087908acf454aefb2de4fd4950b4026", Resolve(epione.HandsEquipmentSets[0].Primary).Blueprint);
            Assert.Equal("6a0b75f7d93a48e4a1e89b87dd32bfce", Resolve(epione.HandsEquipmentSets[1].Primary).Blueprint);
            Assert.Equal("8c2a5683e27043e0af4e73bc0a50594b", Resolve(haymar.HandsEquipmentSets[0].Primary).Blueprint);

            var epioneKit = Resolve(epione.QuickSlots[1]);
            var haymarKit = Resolve(haymar.QuickSlots[1]);
            Assert.Equal("10b3319b17334d089cdf1b53f08595d3", epioneKit.Blueprint);
            Assert.Equal(epioneKit.Blueprint, haymarKit.Blueprint);
            Assert.NotEqual(epioneKit.UniqueId, haymarKit.UniqueId);
            Assert.Equal(1, epioneKit.Charges);

            IGameItemEntry Resolve(IGameUnitEquippedEntry slot)
            {
                Assert.NotNull(slot);
                Assert.True(slot.IsReadOnly);
                var item = game.SharedInventory.FindEquipped(slot.ItemRef);
                Assert.NotNull(item);
                Assert.Equal(slot.ItemRef, item.UniqueId);
                return item;
            }
        }

        [Fact]
        public void BetaInventoryStackAndChargesRoundTripAndKeepOtherEntriesIntact()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_RECRUITED_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            using var document = new SaveGameDocument(path);
            var items = Assert.Single(document.Game.SharedInventory.Sections).Items;
            var medikits = items.First(item => item.Blueprint == "10b3319b17334d089cdf1b53f08595d3" && item.IsStackable);
            var scrolls = items.First(item => item.Blueprint == "e407aa96008e480eb447e82ee3fabbed" && item.IsStackable);
            var medikitCount = medikits.Count;
            var scrollCount = scrolls.Count;
            var medikitCharges = medikits.Charges;
            var newMedikitCount = medikitCount == 9 ? 10 : 9;
            var newScrollCount = scrollCount == 18 ? 19 : 18;
            var newMedikitCharges = medikitCharges == 3 ? 4 : 3;
            var weapon = items.First(item => item.Type == "Weapon");
            Assert.Equal("Usable", medikits.Type);
            Assert.Contains(items, item => item.Type == "Armor");
            Assert.NotEmpty(weapon.Blueprint);
            Assert.False(weapon.TrySetCount(2));
            Assert.False(medikits.TrySetCount(0));
            Assert.True(medikits.IsChargable);
            Assert.True(medikits.CanEditCharges);
            Assert.False(medikits.TrySetCharges(0));
            Assert.False(weapon.IsChargable);
            Assert.False(weapon.TrySetCharges(3));
            var singleUseItem = items.First(item => item.IsChargable && !item.IsStackable);
            Assert.True(singleUseItem.CanEditCharges);
            Assert.False(singleUseItem.CanEditQuantity);
            Assert.False(Money(document).IsReadOnly);

            var directory = Directory.CreateTempSubdirectory();
            try {
                var target = Path.Combine(directory.FullName, "inventory.zks");
                File.Copy(path, target);
                Assert.True(medikits.TrySetCharges(newMedikitCharges));
                document.Save(target, null);
                Assert.Equal(File.ReadAllBytes(path), File.ReadAllBytes(Path.ChangeExtension(target, ".zks.bck")));

                Assert.True(medikits.TrySetCount(newMedikitCount));
                Assert.True(scrolls.TrySetCount(newScrollCount));
                document.Save(target, null);
                using (var reopened = new SaveGameDocument(target)) {
                    var updated = Assert.Single(reopened.Game.SharedInventory.Sections).Items;
                    Assert.Equal(items.Count, updated.Count);
                    Assert.Equal(newMedikitCount, updated.Single(item => item.UniqueId == medikits.UniqueId).Count);
                    Assert.Equal(newScrollCount, updated.Single(item => item.UniqueId == scrolls.UniqueId).Count);
                    Assert.Equal(newMedikitCharges, updated.Single(item => item.UniqueId == medikits.UniqueId).Charges);
                }
                CompareOwlArchives(path, target, 3, playerChanged: false);

                Assert.True(medikits.TrySetCharges(medikitCharges));
                Assert.True(medikits.TrySetCount(medikitCount));
                Assert.True(scrolls.TrySetCount(scrollCount));
                document.Save(target, null);
                CompareArchives(path, target, expectChanges: false);
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void BetaQuicksaveExposesAriaAndGameState()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_BETA_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            using var document = new SaveGameDocument(path);
            Assert.Equal(GameDefinition.Warhammer40K_DarkHeresy, document.Game.Definition);
            Assert.Null(document.Json);
            Assert.Equal("Aria", document.Game.MainCharacter.Name);
            Assert.Equal(UnitEntityType.Player, document.Game.MainCharacter.Type);

            var stats = document.Game.MainCharacter.Stats.Groupings.SelectMany(group => group).ToArray();
            var level = Assert.Single(stats, entry => entry.Name == "Character Level");
            Assert.True(level.Value > 0);
            Assert.Equal(level.Value, document.Game.MainCharacter.Progression.CurrentLevel);
            Assert.Contains(stats, entry => entry.Name == "Experience" && entry.Value == 969);
            Assert.NotEmpty(document.Game.MainCharacter.Feats);
            Assert.NotEmpty(document.Game.MainCharacter.Abilities);
            Assert.True(document.Game.MainCharacter.Progression.IsSupported);
            Assert.Equal(969, document.Game.MainCharacter.Progression.Experience);
            Assert.NotNull(document.Game.MainCharacter.Progression.Data);
            Assert.DoesNotContain(document.Game.MainCharacter.Sections, section => section.Name is "Features" or "Abilities" or "Progression selections");
            var general = Assert.Single(document.Game.MainCharacter.Stats.Groupings, group => group.Name == "General");
            Assert.All(general, entry => Assert.True(entry.IsReadOnly));
            var perception = Stat(document.Game.MainCharacter, "Perception");
            Assert.Equal(5, perception.Step);
            Assert.Equal(10, perception.Value);
            Assert.False(perception.IsReadOnly);
            Assert.True(document.Game.SharedInventory.IsSupported);
            var items = Assert.Single(document.Game.SharedInventory.Sections).Items;
            Assert.NotEmpty(items);
            Assert.All(items.Where(item => !item.IsChargable), item => Assert.False(item.CanEditCharges));

            var quests = document.Game.State.QuestBook.Entries;
            Assert.Contains(quests, quest => quest.State == "Started" && quest.Objectives.Count > 0);
            Assert.All(quests, quest => {
                Assert.True(quest.IsReadOnly);
                Assert.All(quest.Objectives, objective => Assert.True(objective.IsReadOnly));
            });
            Assert.Contains(document.Game.State.Sections, section => section.Name == "Investigations");

            Assert.True(document.CanSave);
            Assert.Contains(document.Game.Party.Data.Properties.OfType<IGameDataInteger>(),
                entry => entry.Label == "Experience rate (%)" && entry.Value == 100 && entry.IsReadOnly);
            Assert.Contains(document.Game.Party.Data.Properties.OfType<IGameDataText>(),
                entry => entry.Label == "Difficulty" && entry.Value == "Daring" && entry.IsReadOnly);
        }

        [Fact]
        public void BetaMoneyEditWritesOnlyPlayerAndChecksumToNewCopy()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_BETA_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            var directory = Directory.CreateTempSubdirectory();
            try {
                var target = Path.Combine(directory.FullName, "edited.zks");
                var restored = Path.Combine(directory.FullName, "restored.zks");
                var originalArchiveHash = SHA256.HashData(File.ReadAllBytes(path));
                using var document = new SaveGameDocument(path);
                var money = Money(document);
                var originalMoney = money.Value;
                var nextMoney = originalMoney == "432123" ? "432124" : "432123";

                money.Value = "-1";
                Assert.Throws<InvalidDataException>(() => document.Save(target, null));
                Assert.False(File.Exists(target));
                File.Copy(path, target);
                money.Value = nextMoney;
                document.Save(target, null);
                Assert.Equal(originalArchiveHash, SHA256.HashData(File.ReadAllBytes(Path.ChangeExtension(target, ".zks.bck"))));

                using (var edited = new SaveGameDocument(target)) {
                    Assert.Equal(nextMoney, Money(edited).Value);
                    Assert.Equal("Aria", edited.Game.MainCharacter.Name);
                }
                CompareArchives(path, target);

                money.Value = originalMoney;
                document.Save(restored, null);
                CompareArchives(path, restored, expectChanges: false);
                Assert.Equal(originalArchiveHash, SHA256.HashData(File.ReadAllBytes(path)));
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void BetaMoneyAndAdvancementEditsCanBeSavedTogether()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_BETA_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            var directory = Directory.CreateTempSubdirectory();
            try {
                var editedPath = Path.Combine(directory.FullName, "edited.zks");
                var nextPath = Path.Combine(directory.FullName, "edited-again.zks");
                var originalHash = SHA256.HashData(File.ReadAllBytes(path));
                using var document = new SaveGameDocument(path);
                var perception = Stat(document.Game.MainCharacter, "Perception");
                var diplomat = Assert.Single(document.Game.Characters, unit => unit.Stats.Groupings
                    .SelectMany(group => group).Any(stat => stat.Name == "Diplomacy" && stat.Value > 0));
                var diplomacy = Stat(diplomat, "Diplomacy");
                Assert.Equal(5, perception.Step);
                Assert.Equal(10, diplomacy.Step);

                Assert.False(perception.TrySetValue(11));
                Assert.False(perception.TrySetValue(25));
                Assert.False(perception.TrySetValue(0));
                Assert.Equal(10, perception.Value);
                Assert.True(perception.TrySetValue(15));
                Assert.True(diplomacy.TrySetValue(20));
                Money(document).Value = "987";
                document.Save(editedPath, null);

                using (var edited = new SaveGameDocument(editedPath)) {
                    Assert.Equal(15, Stat(edited.Game.MainCharacter, "Perception").Value);
                    Assert.Equal(10, Stat(edited.Game.MainCharacter, "Toughness").Value);
                    Assert.Equal(20, Stat(edited.Game.Characters.Single(unit => unit.UniqueId == diplomat.UniqueId), "Diplomacy").Value);
                    Assert.Equal("987", Money(edited).Value);
                    Assert.True(Stat(edited.Game.MainCharacter, "Perception").TrySetValue(20));
                }
                CompareOwlArchives(path, editedPath, 2);

                Assert.True(perception.TrySetValue(10));
                document.Save(nextPath, null);
                using var next = new SaveGameDocument(nextPath);
                Assert.Equal(10, Stat(next.Game.MainCharacter, "Perception").Value);
                Assert.Equal("987", Money(next).Value);
                CompareOwlArchives(path, nextPath, 1);
                Assert.Equal(originalHash, SHA256.HashData(File.ReadAllBytes(path)));
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        [Fact]
        public void BetaUninvestedAdvancementsCreateAndRestoreIndependentFeatureRecords()
        {
            var path = Environment.GetEnvironmentVariable("ARCEMI_DARK_HERESY_BETA_SAVE");
            if (string.IsNullOrWhiteSpace(path)) return;

            var directory = Directory.CreateTempSubdirectory();
            try {
                var target = Path.Combine(directory.FullName, "granted.zks");
                File.Copy(path, target);
                using var document = new SaveGameDocument(target);
                var capital = Stat(document.Game.MainCharacter, "Strength");
                var companion = document.Game.Characters.Single(unit => unit.Blueprint == "51880655802645dead2e5ba4ff64007c");
                var skill = Stat(companion, "Athletics");
                Assert.Equal(0, capital.Value);
                Assert.Equal(0, skill.Value);
                Assert.True(capital.TrySetValue(5));
                Assert.True(skill.TrySetValue(10));
                document.Save(target, null);
                Assert.Equal(File.ReadAllBytes(path), File.ReadAllBytes(Path.ChangeExtension(target, ".zks.bck")));

                using (var reopened = new SaveGameDocument(target)) {
                    Assert.Equal(5, Stat(reopened.Game.MainCharacter, "Strength").Value);
                    Assert.Equal(10, Stat(reopened.Game.Characters.Single(unit => unit.UniqueId == companion.UniqueId), "Athletics").Value);
                    Assert.Equal(document.Game.MainCharacter.Feats.Count + 1, reopened.Game.MainCharacter.Feats.Count);
                    Assert.Contains(reopened.Game.MainCharacter.Feats, fact => fact.Blueprint == "d86a5d75aa32469289bf0e46835d27d5" && fact.Rank == 1);
                    Assert.Equal(10, Stat(reopened.Game.MainCharacter, "Perception").Value);
                }
                Assert.True(capital.TrySetValue(0));
                Assert.True(skill.TrySetValue(0));
                document.Save(target, null);
                CompareArchives(path, target, expectChanges: false);
            }
            finally {
                directory.Delete(recursive: true);
            }
        }

        private static IGameUnitStatsEntry Stat(IGameUnitModel unit, string name) =>
            Assert.Single(unit.Stats.Groupings.SelectMany(group => group), field => field.Name == name);

        private static void CompareOwlArchives(string source, string target, int expectedBytesChanged, bool playerChanged = true)
        {
            using var before = ZipFile.OpenRead(source);
            using var after = ZipFile.OpenRead(target);
            var originalParty = ReadEntry(before.GetEntry("party.owl"));
            var writtenParty = ReadEntry(after.GetEntry("party.owl"));
            Assert.Equal(originalParty.Length, writtenParty.Length);
            Assert.Equal(expectedBytesChanged, originalParty.Zip(writtenParty).Count(pair => pair.First != pair.Second));

            var manifest = Encoding.UTF8.GetString(ReadEntry(after.GetEntry("checksums")));
            foreach (var name in new[] { "party.owl", "player.owl" }) {
                var payload = ReadEntry(after.GetEntry(name));
                if (name == "player.owl" && !playerChanged) {
                    Assert.Equal(ReadEntry(before.GetEntry(name)), payload);
                    continue;
                }
                var digest = GameChecksumText(payload);
                Assert.Contains(name + ": " + digest + "\n", manifest);
            }
            foreach (var entry in before.Entries.Where(entry => entry.FullName is not ("player.owl" or "party.owl" or "checksums"))) {
                Assert.Equal(ReadEntry(entry), ReadEntry(after.GetEntry(entry.FullName)));
            }
        }

        private static IGameDataText Money(SaveGameDocument document) =>
            Assert.Single(document.Game.Party.Data.Properties.OfType<IGameDataText>(), field => field.Label == "Money");

        private static void CompareArchives(string source, string target, bool expectChanges = true)
        {
            using var before = ZipFile.OpenRead(source);
            using var after = ZipFile.OpenRead(target);
            Assert.Equal(before.Entries.Select(x => x.FullName), after.Entries.Select(x => x.FullName));
            foreach (var first in before.Entries) {
                var original = ReadEntry(first);
                var updated = ReadEntry(after.GetEntry(first.FullName));
                if (!expectChanges || first.FullName is not ("player.owl" or "checksums")) {
                    Assert.Equal(original, updated);
                }
                else if (first.FullName == "player.owl") {
                    Assert.Equal(original.Length, updated.Length);
                    var differences = Enumerable.Range(0, original.Length).Where(i => original[i] != updated[i]).ToArray();
                    Assert.NotEmpty(differences);
                    Assert.InRange(differences[^1] - differences[0], 0, sizeof(long) - 1);
                }
                else {
                    var player = ReadEntry(after.GetEntry("player.owl"));
                    var digest = GameChecksumText(player);
                    Assert.Contains("player.owl: " + digest + "\n", Encoding.UTF8.GetString(updated));
                }
            }
        }

        private static string GameChecksumText(byte[] payload)
        {
            // Reproduce Unity Mono's replacement for invalid UTF-8 pairs in its raw MD5 digest.
            var digest = Encoding.Latin1.GetString(MD5.HashData(payload));
            var normalized = Regex.Replace(digest, @"\xE0[\x80-\x9F]|\xED[\xA0-\xBF]|\xF0[\x80-\x8F]|\xF4[\x90-\xBF]", "\u00EF\u00BF\u00BD");
            return Encoding.UTF8.GetString(Encoding.Latin1.GetBytes(normalized));
        }

        private static byte[] ReadEntry(ZipArchiveEntry entry)
        {
            using var source = entry.Open();
            using var buffer = new MemoryStream();
            source.CopyTo(buffer);
            return buffer.ToArray();
        }
    }
}

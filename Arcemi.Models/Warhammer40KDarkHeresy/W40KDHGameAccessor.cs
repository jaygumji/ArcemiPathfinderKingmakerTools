using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    public class W40KDHGameAccessor : IGameAccessor
    {
        public static bool Detect(IGameEditFile file)
        {
            if (file?.Header is null) return false;
            var headerAccessor = file.Header.GetAccessor();
            if (!string.Equals(headerAccessor.Value<string>("Format"), "OwlPack", StringComparison.OrdinalIgnoreCase)) return false;
            // Rogue Trader has this state root in player.json.
            if (file.Player?.GetAccessor()?.HasProperty("StarSystemsState") ?? false) return false;
            return true;
        }

        public W40KDHGameAccessor(IGameEditFile file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            OperationEvents = new GameOperationEvents(this);
            var analyzedCharacters = W40KDHPartyAnalysis.ReadFromJson(file.Party?.GetAccessor()?.UnderlyingObject);
            EnrichAdvancementStats(analyzedCharacters);
            Party = new W40KDHGamePartyModel(analyzedCharacters);
            Characters = new GameModelCollection<IGameUnitModel>(
                analyzedCharacters.Select((c, i) => (IGameUnitModel)new W40KDHGameUnitModel(
                    this,
                    c,
                    i,
                    c.Stats)).ToArray(),
                GameModelCollectionWriter<IGameUnitModel>.ReadOnly);
            MainCharacter = Characters.FirstOrDefault(x => x.UniqueId.Eq(MainCharacterId)) ?? Characters.FirstOrDefault();
            SharedInventory = new W40KDHUnsupportedInventoryModel();
            SharedStash = new W40KDHUnsupportedInventoryModel();
            Management = new W40KDHUnsupportedManagementModel();
            State = new W40KDHUnsupportedStateModel();
        }

        public IGameEditFile File { get; }
        public GameDefinition Definition => GameDefinition.Warhammer40K_DarkHeresy;
        public string MainCharacterId
        {
            get => MainCharacter?.UniqueId;
            set => MainCharacter = Characters.FirstOrDefault(c => c.UniqueId.Eq(value)) ?? MainCharacter;
        }

        public IGamePartyModel Party { get; }
        public IGameUnitModel MainCharacter { get; private set; }
        public IGameModelCollection<IGameUnitModel> Characters { get; }
        public IGameInventoryModel SharedInventory { get; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; }
        public IGameStateModel State { get; }
        public IGameOperationEvents OperationEvents { get; }

        public void BeforeSave()
        {
        }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit) => null;

        public void SetMainCharacter(IGameUnitModel unit)
        {
            MainCharacter = unit;
        }

        private void EnrichAdvancementStats(IReadOnlyList<W40KDHAnalyzedCharacter> characters)
        {
            if (characters is null || characters.Count == 0) return;

            foreach (var character in characters) {
                var allStats = character.Stats.ToDictionary(x => x.Name, StringComparer.Ordinal);
                EnsureMappedSelectionSlots(character);
                var grouped = character.Selections
                    .Select(selection => (Selection: selection, Mapping: MapSelection(selection)))
                    .Where(x => x.Mapping is not null)
                    .GroupBy(x => x.Mapping.StatName, StringComparer.Ordinal);

                foreach (var group in grouped) {
                    var mapped = group.ToArray();
                    var total = mapped.Sum(x => x.Selection.Rank * x.Mapping.PointsPerRank);
                    var editable = mapped.FirstOrDefault(x => x.Selection.RankOffset.HasValue);
                    var stat = allStats.TryGetValue(group.Key, out var existing)
                        ? existing
                        : CreateMappedStat(character, group.Key, total, mapped);

                    var hasDirectSlot = stat.SlotPath.HasValue();
                    if (!hasDirectSlot) {
                        stat.Value = total;
                        stat.IsEditable = editable.Selection?.RankOffset is int;
                        stat.SlotScale = editable.Mapping?.PointsPerRank ?? 1;
                        stat.SlotBase = editable.Mapping is object
                            ? total - (editable.Selection.Rank * editable.Mapping.PointsPerRank)
                            : 0;
                        stat.SlotPath = editable.Selection?.RankOffset is int rankOffset
                            ? BuildSelectionRankSlotPath(character.Index, rankOffset)
                            : null;
                    }
                    if (stat.JsonRef is object) {
                        stat.JsonRef["value"] = stat.Value;
                        stat.JsonRef["editable"] = stat.IsEditable;
                        stat.JsonRef["source"] = "party.owl";
                        stat.JsonRef["slotPath"] = stat.SlotPath ?? string.Empty;
                        stat.JsonRef["slotScale"] = stat.SlotScale;
                        stat.JsonRef["slotBase"] = stat.SlotBase;
                    }
                }
            }
        }

        private static W40KDHAnalyzedStat CreateMappedStat(
            W40KDHAnalyzedCharacter character,
            string statName,
            int value,
            IEnumerable<(W40KDHAnalyzedSelection Selection, AdvancementMapping Mapping)> mappedSelections)
        {
            var editableSelection = mappedSelections.FirstOrDefault(x => x.Selection.RankOffset.HasValue);
            var stat = new W40KDHAnalyzedStat(statName, value, isEditable: editableSelection.Selection?.RankOffset is int, source: "party.owl") {
                SlotScale = editableSelection.Mapping?.PointsPerRank ?? 1,
                SlotBase = editableSelection.Mapping is object
                    ? value - (editableSelection.Selection.Rank * editableSelection.Mapping.PointsPerRank)
                    : 0,
                SlotPath = editableSelection.Selection?.RankOffset is int rankOffset
                    ? BuildSelectionRankSlotPath(character.Index, rankOffset)
                    : null,
            };

            var statObj = new JObject {
                { "name", stat.Name },
                { "value", stat.Value },
                { "editable", stat.IsEditable },
                { "source", stat.Source },
                { "slotPath", stat.SlotPath ?? string.Empty },
                { "slotScale", stat.SlotScale },
                { "slotBase", stat.SlotBase },
            };
            stat.JsonRef = statObj;

            if (TryGetCharacterStatsArray(character, out var statsArray)) {
                statsArray.Add(statObj);
            }
            character.Stats.Add(stat);
            return stat;
        }

        private static void EnsureMappedSelectionSlots(W40KDHAnalyzedCharacter character)
        {
            if (character?.Selections is null || character.Selections.Count == 0) return;

            var mappedSlots = new HashSet<W40KDHAnalyzedSelection>();
            var mappedStatNames = new HashSet<string>(
                character.Selections
                    .Select(selection => (Selection: selection, Mapping: MapSelection(selection)))
                    .Where(x => x.Mapping is object)
                    .Select(x => {
                        mappedSlots.Add(x.Selection);
                        return x.Mapping.StatName;
                    }),
                StringComparer.Ordinal);
            var candidateSlots = character.Selections
                .Where(x => x is object
                    && x.RankOffset.HasValue
                    && x.FeatureBlueprintOffset.HasValue
                    && x.FeatureBlueprintLength.HasValue
                    && x.FeatureBlueprintLength.Value >= 32)
                .OrderBy(x => x.RankOffset.Value)
                .ToList();

            foreach (var mapping in AllAdvancementMappings) {
                if (mappedStatNames.Contains(mapping.StatName)) continue;
                var slot = character.Selections.FirstOrDefault(IsFreeSelectionSlot)
                    ?? candidateSlots.FirstOrDefault(x => !mappedSlots.Contains(x));
                if (slot is null) continue;

                slot.FeatureBlueprint = mapping.BlueprintId;
                slot.Rank = 0;
                if (slot.JsonRef is object) {
                    slot.JsonRef["featureBlueprint"] = slot.FeatureBlueprint;
                    slot.JsonRef["rank"] = slot.Rank;
                }
                mappedStatNames.Add(mapping.StatName);
                mappedSlots.Add(slot);
            }
        }

        private static bool IsFreeSelectionSlot(W40KDHAnalyzedSelection selection)
        {
            if (selection is null) return false;
            return selection.RankOffset.HasValue
                && selection.FeatureBlueprintOffset.HasValue
                && selection.FeatureBlueprintLength.HasValue
                && string.IsNullOrWhiteSpace(selection.FeatureBlueprint);
        }

        private static string BuildSelectionRankSlotPath(int characterIndex, int rankOffset)
        {
            return string.Concat("chars[", characterIndex, "].stats.SelectionRank.i32@", rankOffset);
        }

        private static bool TryGetCharacterStatsArray(W40KDHAnalyzedCharacter character, out JArray statsArray)
        {
            statsArray = character?.JsonRef?["stats"] as JArray;
            return statsArray is object;
        }

        private static AdvancementMapping MapSelection(W40KDHAnalyzedSelection selection)
        {
            if (selection?.FeatureBlueprint is null) return null;
            if (AdvancementMappingsByBlueprintId.TryGetValue(selection.FeatureBlueprint, out var byId)) {
                return byId;
            }

            if (!TryGetBlueprintMetadata(selection.FeatureBlueprint, out var typeFullName, out var name)) {
                return null;
            }
            if (!typeFullName.Contains("BlueprintAttributeAdvancement", StringComparison.Ordinal)
                && !typeFullName.Contains("BlueprintSkillAdvancement", StringComparison.Ordinal)) return null;

            foreach (var mapping in AllAdvancementMappings) {
                if (name.Contains(mapping.Token, StringComparison.OrdinalIgnoreCase)) {
                    return mapping;
                }
            }

            return null;
        }

        private static bool TryGetBlueprintMetadata(string blueprintId, out string typeFullName, out string name)
        {
            var resources = GameDefinition.Warhammer40K_DarkHeresy.Resources;
            if (resources?.Blueprints is object
                && resources.Blueprints.TryGetType(blueprintId, out typeFullName)
                && resources.Blueprints.TryGetName(blueprintId, out name)) {
                return true;
            }

            if (CheatDataLookup.Value.TryGetValue(blueprintId, out var entry)) {
                typeFullName = entry.TypeFullName;
                name = entry.Name;
                return true;
            }

            typeFullName = null;
            name = null;
            return false;
        }

        private static readonly Lazy<IReadOnlyDictionary<string, CheatDataEntry>> CheatDataLookup
            = new Lazy<IReadOnlyDictionary<string, CheatDataEntry>>(LoadCheatDataLookup);

        private static IReadOnlyDictionary<string, CheatDataEntry> LoadCheatDataLookup()
        {
            var lookup = new Dictionary<string, CheatDataEntry>(StringComparer.Ordinal);
            foreach (var path in GetPotentialCheatDataPaths()) {
                if (!System.IO.File.Exists(path)) continue;
                try {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var streamReader = new StreamReader(stream);
                    using var reader = new JsonTextReader(streamReader);
                    var serializer = new JsonSerializer();
                    var root = serializer.Deserialize<JObject>(reader);
                    var entries = root?["Entries"] as JArray;
                    if (entries is null) continue;
                    foreach (var entry in entries.OfType<JObject>()) {
                        var guid = entry["Guid"]?.Value<string>();
                        if (string.IsNullOrWhiteSpace(guid)) continue;
                        if (lookup.ContainsKey(guid)) continue;
                        lookup.Add(guid, new CheatDataEntry(
                            entry["TypeFullName"]?.Value<string>() ?? string.Empty,
                            entry["Name"]?.Value<string>() ?? string.Empty));
                    }
                }
                catch {
                    // Ignore parse failures for fallback lookup paths.
                }
                if (lookup.Count > 0) break;
            }
            return lookup;
        }

        private static IEnumerable<string> GetPotentialCheatDataPaths()
        {
            yield return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "Blueprints_W40KDH.json");
            yield return Path.Combine(Directory.GetCurrentDirectory(), "_Defs", "Blueprints_W40KDH.json");
            yield return @"D:\Programs\Steam\steamapps\common\Warhammer 40,000 Dark Heresy Playtest\Bundles\cheatdata.json";
            yield return "/mnt/d/Programs/Steam/steamapps/common/Warhammer 40,000 Dark Heresy Playtest/Bundles/cheatdata.json";
        }

        private static readonly AdvancementMapping[] AttributeMappings = {
            new AdvancementMapping("WeaponSkill", "Weapon Skill", 5, "9fe1b142bcfa4203b2949ca121fb4b19"),
            new AdvancementMapping("BallisticSkill", "Ballistic Skill", 5, "ef35544ab8a040bd9461189652b179d4"),
            new AdvancementMapping("Strength", "Strength", 5, "d86a5d75aa32469289bf0e46835d27d5"),
            new AdvancementMapping("Toughness", "Toughness", 5, "1ea6d519b60c4d4993a386dab0f94253"),
            new AdvancementMapping("Agility", "Agility", 5, "23921e40f1954693ae36538629767030"),
            new AdvancementMapping("Intelligence", "Intelligence", 5, "f60d1a2d62234c018e3e0882563a058c"),
            new AdvancementMapping("Perception", "Perception", 5, "01c01d1bae2a4ce0baefd44cc3c1a641"),
            new AdvancementMapping("Willpower", "Willpower", 5, "fbf0a0ed1b754f569512494e24e054b1"),
            new AdvancementMapping("Willpower", "Willpower", 5, "41cd7c199e734921a3e3b5a1fcc362f6"),
            new AdvancementMapping("Fellowship", "Fellowship", 5, "b4ced0141f374f3d811177fc5fb08ae3"),
        };

        private static readonly AdvancementMapping[] SkillMappings = {
            new AdvancementMapping("Athletics", "Athletics", 10, "f53f3695352f4a25968ee22fc00eaed8"),
            new AdvancementMapping("Tenacity", "Tenacity", 10, "5c3fde6ac014419ba2ecb8777f41c804"),
            new AdvancementMapping("Mobility", "Mobility", 10, "1fdaf03d04124a559941ac77248b1ebc"),
            new AdvancementMapping("Resistance", "Resistance", 10, "aaf536a5947847ba9d9a8c67843a9a7f"),
            new AdvancementMapping("Demolition", "Demolition", 10, "28000dbc252d4994996faa4b5f7a9bb4"),
            new AdvancementMapping("Reflexes", "Reflexes", 10, "aefa5ca4b2fe495daf95ccfc10380dec"),
            new AdvancementMapping("SleightOfHand", "Sleight Of Hand", 10, "92421ec45ea34033b74fd1fb6f730851"),
            new AdvancementMapping("LoreHeresy", "Lore Heresy", 10, "4580edfdbca44ea2ace9e04c433841d6"),
            new AdvancementMapping("LoreXenos", "Lore Xenos", 10, "1fb90cb8a7f74acfa1ecadfa4390eefd"),
            new AdvancementMapping("LoreWarp", "Lore Warp", 10, "a00e0a152e964bb8bd6774f754c2860d"),
            new AdvancementMapping("TechUse", "Tech Use", 10, "83fed3e54f9446a19138f40630ac1443"),
            new AdvancementMapping("Interrogation", "Interrogation", 10, "359a62f25c204be797ec4c280be466b1"),
            new AdvancementMapping("Mettle", "Mettle", 10, "9954ba4665664665b180f842893594f2"),
            new AdvancementMapping("Awareness", "Awareness", 10, "963d7ad946374a9c9033d853091ed55e"),
            new AdvancementMapping("Wits", "Wits", 10, "91f3f090b17f4cccb413cfb950379999"),
            new AdvancementMapping("Intimidation", "Intimidation", 10, "7321c1acd38c4a3f8984313c223f47e9"),
            new AdvancementMapping("Diplomacy", "Diplomacy", 10, "4c77d5c9e5f2479ea918ecfa55462113"),
            new AdvancementMapping("Medicae", "Medicae", 10, "a60de9ca50844f9aa07a9499e017b707"),
            new AdvancementMapping("LoreImperium", "Lore Imperium", 10, "7ebcd4987ba9414bb191e5639a464bf7"),
            new AdvancementMapping("LoreXenos", "Lore Xenos", 10, "28bf8c1a5e904b4ebc3ce8a11689b888"),
        };

        private static readonly AdvancementMapping[] AllAdvancementMappings = AttributeMappings.Concat(SkillMappings).ToArray();

        private static readonly IReadOnlyDictionary<string, AdvancementMapping> AdvancementMappingsByBlueprintId
            = new Dictionary<string, AdvancementMapping>(StringComparer.Ordinal) {
                ["9fe1b142bcfa4203b2949ca121fb4b19"] = AttributeMappings[0],
                ["ef35544ab8a040bd9461189652b179d4"] = AttributeMappings[1],
                ["d86a5d75aa32469289bf0e46835d27d5"] = AttributeMappings[2],
                ["1ea6d519b60c4d4993a386dab0f94253"] = AttributeMappings[3],
                ["23921e40f1954693ae36538629767030"] = AttributeMappings[4],
                ["f60d1a2d62234c018e3e0882563a058c"] = AttributeMappings[5],
                ["01c01d1bae2a4ce0baefd44cc3c1a641"] = AttributeMappings[6],
                ["fbf0a0ed1b754f569512494e24e054b1"] = AttributeMappings[7],
                ["41cd7c199e734921a3e3b5a1fcc362f6"] = AttributeMappings[8],
                ["b4ced0141f374f3d811177fc5fb08ae3"] = AttributeMappings[9],
                ["f53f3695352f4a25968ee22fc00eaed8"] = SkillMappings[0],
                ["5c3fde6ac014419ba2ecb8777f41c804"] = SkillMappings[1],
                ["1fdaf03d04124a559941ac77248b1ebc"] = SkillMappings[2],
                ["aaf536a5947847ba9d9a8c67843a9a7f"] = SkillMappings[3],
                ["28000dbc252d4994996faa4b5f7a9bb4"] = SkillMappings[4],
                ["aefa5ca4b2fe495daf95ccfc10380dec"] = SkillMappings[5],
                ["92421ec45ea34033b74fd1fb6f730851"] = SkillMappings[6],
                ["4580edfdbca44ea2ace9e04c433841d6"] = SkillMappings[7],
                ["1fb90cb8a7f74acfa1ecadfa4390eefd"] = SkillMappings[8],
                ["a00e0a152e964bb8bd6774f754c2860d"] = SkillMappings[9],
                ["83fed3e54f9446a19138f40630ac1443"] = SkillMappings[10],
                ["359a62f25c204be797ec4c280be466b1"] = SkillMappings[11],
                ["9954ba4665664665b180f842893594f2"] = SkillMappings[12],
                ["963d7ad946374a9c9033d853091ed55e"] = SkillMappings[13],
                ["91f3f090b17f4cccb413cfb950379999"] = SkillMappings[14],
                ["7321c1acd38c4a3f8984313c223f47e9"] = SkillMappings[15],
                ["4c77d5c9e5f2479ea918ecfa55462113"] = SkillMappings[16],
                ["a60de9ca50844f9aa07a9499e017b707"] = SkillMappings[17],
                ["7ebcd4987ba9414bb191e5639a464bf7"] = SkillMappings[18],
                ["28bf8c1a5e904b4ebc3ce8a11689b888"] = SkillMappings[19],
            };

        private sealed class AdvancementMapping
        {
            public AdvancementMapping(string token, string statName, int pointsPerRank, string blueprintId)
            {
                Token = token;
                StatName = statName;
                PointsPerRank = pointsPerRank;
                BlueprintId = blueprintId;
            }

            public string Token { get; }
            public string StatName { get; }
            public int PointsPerRank { get; }
            public string BlueprintId { get; }
        }

        private sealed class CheatDataEntry
        {
            public CheatDataEntry(string typeFullName, string name)
            {
                TypeFullName = typeFullName;
                Name = name;
            }

            public string TypeFullName { get; }
            public string Name { get; }
        }
    }

    internal class W40KDHGamePartyModel : IGamePartyModel
    {
        public W40KDHGamePartyModel(IReadOnlyList<W40KDHAnalyzedCharacter> characters)
        {
            IsSupported = true;
            Data = GameDataModels.Object("Party", new IGameData[] {
                GameDataModels.Integer("Characters", new { Count = characters?.Count ?? 0 }, x => x.Count),
                GameDataModels.Integer("Current Squad", new { Count = characters?.Count(x => x.IsInCurrentSquad) ?? 0 }, x => x.Count),
            });
        }

        public bool IsSupported { get; }
        public IGameDataObject Data { get; }
    }

    internal sealed class W40KDHGameUnitModel : IGameUnitModel
    {
        private readonly W40KDHGameAccessor _game;
        private readonly W40KDHAnalyzedCharacter _analyzedCharacter;
        private readonly int _index;
        private readonly IReadOnlyList<W40KDHAnalyzedStat> _analyzedStats;

        public W40KDHGameUnitModel(
            W40KDHGameAccessor game,
            W40KDHAnalyzedCharacter analyzedCharacter,
            int index,
            IReadOnlyList<W40KDHAnalyzedStat> analyzedStats)
        {
            _game = game;
            _analyzedCharacter = analyzedCharacter ?? throw new ArgumentNullException(nameof(analyzedCharacter));
            _index = index;
            _analyzedStats = analyzedStats ?? Array.Empty<W40KDHAnalyzedStat>();
            Portrait = new W40KDHGameUnitPortraitModel(game, _analyzedCharacter);
            Companion = new W40KDHUnsupportedCompanionModel();
            Alignment = new W40KDHUnsupportedAlignmentModel();
            Asks = new W40KDHUnsupportedAsksModel();
            Race = new W40KDHUnsupportedRaceModel();
            Progression = new W40KDHUnsupportedProgressionModel();
            Stats = new W40KDHGameUnitStatsModel(this, _analyzedStats);
            Appearance = new W40KDHUnsupportedAppearanceModel();
            Body = new W40KDHUnsupportedBodyModel();
            SpellCaster = new W40KDHUnsupportedSpellCasterModel();
            Feats = GameModelCollection<IGameUnitFeatEntry>.Empty;
            Abilities = GameModelCollection<IGameUnitAbilityEntry>.Empty;
            Buffs = GameModelCollection<IGameUnitBuffEntry>.Empty;
            UniqueBuffs = GameModelCollection<IGameUnitBuffEntry>.Empty;
            Sections = Array.Empty<IGameDataObject>();
        }

        public bool IsSupported => true;
        public string UniqueId => _analyzedCharacter.UnitBlueprint.OrIfEmpty(_analyzedCharacter.PortraitBlueprint).OrIfEmpty("dh-unit-" + _index);
        public string Blueprint => _analyzedCharacter.PortraitBlueprint.OrIfEmpty(_analyzedCharacter.UnitBlueprint);
        public string Name => _analyzedCharacter.DisplayName
            .OrIfEmpty(_game.Definition.Resources.GetCharacterName(Blueprint))
            .OrIfEmpty(_analyzedCharacter.IsMainCharacter ? "Main Character" : "Companion " + (_index + 1));
        public string CustomName
        {
            get => _analyzedCharacter.DisplayName;
            set { }
        }
        public string DefaultName => Name;
        public UnitEntityType Type => _analyzedCharacter.IsMainCharacter
            ? UnitEntityType.Player
            : UnitEntityType.Companion;
        public bool IsMainCharacter => _analyzedCharacter.IsMainCharacter;
        internal W40KDHGameAccessor Game => _game;
        public UnitEntityModel Ref => null;
        public IGameUnitPortraitModel Portrait { get; }
        public IGameUnitCompanionModel Companion { get; }
        public IGameDataObject Overview { get; } = GameDataModels.Object("Overview", Array.Empty<IGameData>());
        public IGameUnitAlignmentModel Alignment { get; }
        public IGameUnitAsksModel Asks { get; }
        public IGameUnitRaceModel Race { get; }
        public IGameUnitProgressionModel Progression { get; }
        public IGameUnitStatsModel Stats { get; }
        public IGameUnitAppearanceModel Appearance { get; }
        public IGameUnitBodyModel Body { get; }
        public IGameUnitSpellCasterModel SpellCaster { get; }
        public IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        public IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        public IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        public IGameModelCollection<IGameUnitBuffEntry> UniqueBuffs { get; }
        public IReadOnlyList<IGameDataObject> Sections { get; }

        public void ReplacePartyMemberWith(IGameUnitModel unit) { }
        public void AddToRetinue() { }
    }

    internal sealed class W40KDHGameUnitPortraitModel : IGameUnitPortraitModel
    {
        private readonly W40KDHGameAccessor _game;
        private readonly W40KDHAnalyzedCharacter _analyzedCharacter;

        public W40KDHGameUnitPortraitModel(W40KDHGameAccessor game, W40KDHAnalyzedCharacter analyzedCharacter)
        {
            _game = game;
            _analyzedCharacter = analyzedCharacter;
        }

        public bool IsSupported => !string.IsNullOrWhiteSpace(Blueprint);
        public string Blueprint => _analyzedCharacter?.PortraitBlueprint;
        public string Path => _game.Definition.Resources.GetPortraitsUri(Blueprint);

        public void Set(Portrait portrait)
        {
            // Editing portrait requires a mapped OWL write slot.
        }
    }

    internal sealed class W40KDHUnsupportedInventoryModel : IGameInventoryModel
    {
        public bool IsSupported => false;
        public IReadOnlyList<IGameItemSection> Sections { get; } = Array.Empty<IGameItemSection>();
        public IGameItemEntry FindEquipped(object itemRef) => null;
    }

    internal sealed class W40KDHUnsupportedManagementModel : IGameManagementModel
    {
        public bool IsSupported => false;
        public string DisplayName => "Management";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Members", "Member");
        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; } = GameModelCollection<IGameManagementMemberModelEntry>.Empty;
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = Array.Empty<IGameManagementArmyModelEntry>();
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = Array.Empty<IGameManagementTaskModelEntry>();
        public IGameDataObject Overview => null;
        public IGameDataObject Resources => null;
        public IGameDataObject Places => null;
    }

    internal sealed class W40KDHUnsupportedStateModel : IGameStateModel
    {
        public bool IsSupported => false;
        public IGameStateQuestBook QuestBook => null;
        public IReadOnlyList<IGameDataObject> Sections { get; } = Array.Empty<IGameDataObject>();
    }

    internal sealed class W40KDHUnsupportedCompanionModel : IGameUnitCompanionModel
    {
        public bool IsSupported => false;
        public IReadOnlyList<GameEnumValue> AllStates { get; } = Array.Empty<GameEnumValue>();
        public string State { get => string.Empty; set { } }
        public bool IsReadOnly => true;
        public bool IsDialogEnabled => false;
        public bool IsInParty => false;
        public bool IsExCompanion => false;
    }

    internal sealed class W40KDHUnsupportedAlignmentModel : IGameUnitAlignmentModel
    {
        public bool IsSupported => false;
        public string DisplayName => string.Empty;
        public int X { get => 0; set { } }
        public int Y { get => 0; set { } }
        public Alignment Direction => Alignment.TrueNeutral;
        public string LockedAlignmentMask { get => string.Empty; set { } }
        public bool IsAlignmentMaskSupported => false;
        public IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; } = GameModelCollection<IGameUnitAlignmentHistoryEntryModel>.Empty;
    }

    internal sealed class W40KDHUnsupportedAsksModel : IGameUnitAsksModel
    {
        public bool IsSupported => false;
        public string Custom { get => string.Empty; set { } }
    }

    internal sealed class W40KDHUnsupportedRaceModel : IGameUnitRaceModel
    {
        public bool IsSupported => false;
        public string DisplayName => string.Empty;
    }

    internal sealed class W40KDHUnsupportedProgressionModel : IGameUnitProgressionModel
    {
        public bool IsSupported => false;
        public int Experience { get => 0; set { } }
        public int CurrentLevel { get => 0; set { } }
        public bool IsLevelReadOnly => true;
        public bool IsSelectionsRepairable => false;
        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; } = GameModelCollection<IGameUnitSelectionProgressionEntry>.Empty;
        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();
        public IGameDataObject Data => null;
        public void RepairSelections() { }
    }

    internal sealed class W40KDHGameUnitStatsModel : IGameUnitStatsModel
    {
        public W40KDHGameUnitStatsModel(W40KDHGameUnitModel unit, IReadOnlyList<W40KDHAnalyzedStat> analyzedStats)
        {
            _unit = unit;
            _analyzedStats = analyzedStats ?? Array.Empty<W40KDHAnalyzedStat>();

            var entries = new List<IGameUnitStatsEntry>();

            foreach (var stat in _analyzedStats) {
                if (stat.Name.Eq("Player Rank")) {
                    entries.Add(new W40KDHGameUnitStatsEntry(
                        stat.Name,
                        () => _unit.Game.File.Header.GetAccessor().Value<int>("PlayerCharacterRank"),
                        v => _unit.Game.File.Header.GetAccessor().Value(v, "PlayerCharacterRank"),
                        "Header"));
                    continue;
                }

                var captured = stat;
                entries.Add(new W40KDHGameUnitStatsEntry(
                    captured.Name,
                    () => captured.Value,
                    captured.IsEditable ? v => SetStatValue(captured, v) : null,
                    captured.Source));
            }

            Groupings = entries.Count == 0
                ? Array.Empty<GameStatsGrouping>()
                : new[] { new GameStatsGrouping("General", "", entries) };
        }

        private readonly W40KDHGameUnitModel _unit;
        private readonly IReadOnlyList<W40KDHAnalyzedStat> _analyzedStats;

        private static void SetStatValue(W40KDHAnalyzedStat stat, int value)
        {
            if (stat is null) return;
            if (stat.SlotScale > 1) {
                var delta = value - stat.SlotBase;
                var rank = (int)Math.Round(delta / (double)stat.SlotScale, MidpointRounding.AwayFromZero);
                if (rank < 0) rank = 0;
                value = stat.SlotBase + (rank * stat.SlotScale);
            }
            stat.Value = value;
            if (stat.JsonRef is object) {
                stat.JsonRef["value"] = value;
            }
        }

        public bool IsSupported => Groupings.Count > 0;
        public IReadOnlyList<GameStatsGrouping> Groupings { get; }
    }

    internal sealed class W40KDHGameUnitStatsEntry : IGameUnitStatsEntry
    {
        private readonly Func<int> _getter;
        private readonly Action<int> _setter;

        public W40KDHGameUnitStatsEntry(string name, Func<int> getter, Action<int> setter, string info)
        {
            Name = name;
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter;
            Info = info;
        }

        public string Name { get; }
        public int Value { get => _getter(); set => _setter?.Invoke(value); }
        public string Info { get; }
        public bool IsReadOnly => _setter is null;
    }

    internal sealed class W40KDHUnsupportedAppearanceModel : IGameUnitAppearanceModel
    {
        public bool IsSupported => false;
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool CanRestore => false;
        public bool CanDelete => false;
        public void Restore() { }
        public void Delete() { }
    }

    internal sealed class W40KDHUnsupportedBodyModel : IGameUnitBodyModel
    {
        public bool IsSupported => false;
        public IReadOnlyList<IGameUnitHandsEquippedEntry> HandsEquipmentSets { get; } = Array.Empty<IGameUnitHandsEquippedEntry>();
        public IReadOnlyList<IGameUnitEquippedEntry> QuickSlots { get; } = Array.Empty<IGameUnitEquippedEntry>();
        public IGameUnitEquippedEntry Armor => null;
        public IGameUnitEquippedEntry Shirt => null;
        public IGameUnitEquippedEntry Belt => null;
        public IGameUnitEquippedEntry Head => null;
        public IGameUnitEquippedEntry Feet => null;
        public IGameUnitEquippedEntry Gloves => null;
        public IGameUnitEquippedEntry Neck => null;
        public IGameUnitEquippedEntry Glasses => null;
        public IGameUnitEquippedEntry Ring1 => null;
        public IGameUnitEquippedEntry Ring2 => null;
        public IGameUnitEquippedEntry Wrist => null;
        public IGameUnitEquippedEntry Shoulders => null;
    }

    internal sealed class W40KDHUnsupportedSpellCasterModel : IGameUnitSpellCasterModel
    {
        public bool IsSupported => false;
        public IGameUnitSpellCasterBonusSpellModel BonusSpells { get; } = new W40KDHUnsupportedBonusSpellModel();
        public IGameModelCollection<IGameUnitSpellBookEntry> SpellBooks { get; } = GameModelCollection<IGameUnitSpellBookEntry>.Empty;
    }

    internal sealed class W40KDHUnsupportedBonusSpellModel : IGameUnitSpellCasterBonusSpellModel
    {
        public bool IsSupported => false;
        public bool IsUnlocked => false;
        public IEnumerable<SpellIndexAccessor> Accessors => Array.Empty<SpellIndexAccessor>();
        public void Unlock() { }
    }
}

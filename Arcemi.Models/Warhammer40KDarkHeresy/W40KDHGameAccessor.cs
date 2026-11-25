using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    public sealed class W40KDHGameAccessor : IGameAccessor
    {
        internal W40KDHPlayerDocument Player { get; }
        internal W40KDHPartyDocument OwlParty { get; }

        internal static bool IsOwlSave(SaveGameFile archive, HeaderModel header, bool hasJsonLayout) =>
            archive.Contains(W40KDHPartyDocument.PartyEntry)
            || archive.Contains(W40KDHPlayerDocument.PlayerEntry)
            || (!hasJsonLayout && string.Equals(header.GetAccessor().Value<string>("Format"), "OwlPack", StringComparison.OrdinalIgnoreCase));

        internal static W40KDHGameAccessor Open(SaveGameFile archive, HeaderModel header)
        {
            var party = new W40KDHPartyDocument(archive);
            var player = archive.Contains(W40KDHPlayerDocument.PlayerEntry) ? new W40KDHPlayerDocument(archive) : null;
            return new W40KDHGameAccessor(header, party, player);
        }

        private W40KDHGameAccessor(HeaderModel header, W40KDHPartyDocument party, W40KDHPlayerDocument player)
        {
            Player = player;
            OwlParty = party;
            SharedInventory = party.Inventory;
            State = new W40KDHGameStateModel(player?.Data, Definition.Resources);
            SharedStash = player?.Inventory ?? new W40KDHInventoryModel("Shared stash", null, Definition.Resources, writable: false);
            var units = party.Data.Units;
            OperationEvents = new GameOperationEvents(this);
            if (units.Count(x => x.IsMainCharacter) != 1) {
                throw new InvalidDataException("The OWL v3 party does not identify exactly one main character.");
            }

            Party = new W40KDHGamePartyModel(units, player);
            Characters = new GameModelCollection<IGameUnitModel>(
                units.Select(unit => (IGameUnitModel)new W40KDHGameUnitModel(this, unit, party.GetAdvancements(unit), header.PlayerCharacterName)).ToArray(),
                GameModelCollectionWriter<IGameUnitModel>.ReadOnly);
            MainCharacter = Characters.Single(unit => unit.Type == UnitEntityType.Player);
        }

        public GameDefinition Definition => GameDefinition.Warhammer40K_DarkHeresy;
        public string MainCharacterId
        {
            get => MainCharacter.UniqueId;
            set => throw new NotSupportedException();
        }

        public IGamePartyModel Party { get; }
        public IGameUnitModel MainCharacter { get; }
        public IGameModelCollection<IGameUnitModel> Characters { get; }
        public IGameInventoryModel SharedInventory { get; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; } = new W40KDHUnsupportedManagementModel();
        public IGameStateModel State { get; }
        public IGameOperationEvents OperationEvents { get; }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit) => null;
        public void SetMainCharacter(IGameUnitModel unit) => throw new NotSupportedException();
    }

    internal class W40KDHGamePartyModel : IGamePartyModel
    {
        public W40KDHGamePartyModel(IReadOnlyList<W40KDHParsedUnitStats> characters, W40KDHPlayerDocument player)
        {
            IsSupported = true;
            var entries = new List<IGameData> {
                GameDataModels.Integer("Characters", characters, units => units.Count(unit => unit.IsMainCharacter || unit.IsCompanion)),
                GameDataModels.Integer("Current squad", characters, units => units.Count(unit => unit.IsMainCharacter || unit.IsInParty))
            };
            if (player?.Data.Money is long) {
                entries.Add(GameDataModels.Text("Money", player, value => value.MoneyText,
                    player.CanEditMoney ? (value, text) => value.MoneyText = text : null));
            }
            if (player?.Data is { } data) {
                if (data.Chapter.HasValue) entries.Add(GameDataModels.Integer("Chapter", data, value => value.Chapter.Value));
                if (data.ExperienceRate.HasValue) entries.Add(GameDataModels.Integer("Experience rate (%)", data, value => value.ExperienceRate.Value));
                if (data.Difficulty.HasValue()) entries.Add(GameDataModels.Text("Difficulty", data, value => value.Difficulty));
                if (data.RespecAllowed is bool respec) entries.Add(GameDataModels.Boolean("Respec allowed", respec, value => value));
                if (data.OnlyOneSave is bool oneSave) entries.Add(GameDataModels.Boolean("Only one save", oneSave, value => value));
                if (data.SkillCheckModifier.HasValue) {
                    entries.Add(GameDataModels.Integer("Skill check modifier", data, value => value.SkillCheckModifier.Value));
                }
            }
            Data = GameDataModels.Object("Party", entries);
        }

        public bool IsSupported { get; }
        public IGameDataObject Data { get; }
    }

    internal sealed class W40KDHGameUnitModel : IGameUnitModel
    {
        private readonly W40KDHParsedUnitStats _unit;

        public W40KDHGameUnitModel(W40KDHGameAccessor game, W40KDHParsedUnitStats unit, IReadOnlyList<W40KDHAdvancementStat> advancements, string headerName)
        {
            _unit = unit;
            CustomName = unit.IsMainCharacter ? unit.CustomName.OrIfEmpty(headerName) : unit.CustomName;
            var blueprintName = unit.UnitBlueprint.HasValue() ? game.Definition.Resources.GetCharacterName(unit.UnitBlueprint) : null;
            DefaultName = blueprintName.OrIfEmpty(unit.IsMainCharacter ? "Main Character" : unit.IsCompanion ? $"Companion {unit.EntityIndex}" : $"Unit {unit.EntityIndex}");
            Portrait = new W40KDHGameUnitPortraitModel(game.Definition.Resources, unit);
            Companion = new W40KDHCompanionModel(unit);
            Alignment = new W40KDHUnsupportedAlignmentModel();
            Asks = new W40KDHUnsupportedAsksModel();
            Race = new W40KDHUnsupportedRaceModel();
            Progression = new W40KDHProgressionModel(unit, game.Definition.Resources.Blueprints);
            Stats = new W40KDHGameUnitStatsModel(unit, advancements);
            Appearance = new W40KDHUnsupportedAppearanceModel();
            Body = unit.Body;
            SpellCaster = new W40KDHUnsupportedSpellCasterModel();
            var blueprints = game.Definition.Resources.Blueprints;
            Feats = new GameModelCollection<IGameUnitFeatEntry>(unit.ProgressionSelections.Where(fact => fact.IsFeatureFact)
                .Select(fact => (IGameUnitFeatEntry)new W40KDHFeatureEntry(fact, blueprints.GetNameOrBlueprint(fact.FeatureBlueprint),
                    advancements.FirstOrDefault(stat => stat.Owns(fact)))).ToArray(), GameModelCollectionWriter<IGameUnitFeatEntry>.ReadOnly);
            Abilities = new GameModelCollection<IGameUnitAbilityEntry>(unit.FactStatuses.Where(fact => fact.Category == "Abilities")
                .Select(fact => (IGameUnitAbilityEntry)new W40KDHAbilityEntry(
                    blueprints.GetNameOrBlueprint(fact.Blueprint), fact.Blueprint, fact.IsActive)).ToArray(),
                GameModelCollectionWriter<IGameUnitAbilityEntry>.ReadOnly);
            Buffs = new GameModelCollection<IGameUnitBuffEntry>(unit.FactStatuses.Where(fact => fact.Category == "Buffs")
                .Select(fact => (IGameUnitBuffEntry)new W40KDHBuffEntry(
                    blueprints.GetNameOrBlueprint(fact.Blueprint), fact.Blueprint, fact.IsActive)).ToArray(),
                GameModelCollectionWriter<IGameUnitBuffEntry>.ReadOnly);
            UniqueBuffs = GameModelCollection<IGameUnitBuffEntry>.Empty;
            Sections = Array.Empty<IGameDataObject>();
        }

        public bool IsSupported => true;
        public string UniqueId => _unit.EntityId.HasValue() ? _unit.EntityId : $"owl-party-{_unit.EntityIndex}";
        public string Blueprint => _unit.UnitBlueprint;
        public string Name => CustomName.OrIfEmpty(DefaultName);
        public string CustomName { get; }
        public bool CanRename => false;
        string IGameUnitModel.CustomName { get => CustomName; set => throw new NotSupportedException(); }
        public string DefaultName { get; }
        public UnitEntityType Type => _unit.IsMainCharacter
            ? UnitEntityType.Player
            : _unit.IsCompanion ? UnitEntityType.Companion : UnitEntityType.Other;
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

        public void ReplacePartyMemberWith(IGameUnitModel unit) => throw new NotSupportedException();
        public void AddToRetinue() => throw new NotSupportedException();
    }

    internal sealed class W40KDHGameUnitPortraitModel : IGameUnitPortraitModel
    {
        private readonly IGameResourcesProvider _resources;
        private readonly W40KDHParsedUnitStats _unit;
        private readonly string _defaultBlueprint;

        public W40KDHGameUnitPortraitModel(IGameResourcesProvider resources, W40KDHParsedUnitStats unit)
        {
            _resources = resources;
            _unit = unit;
            if (unit.UnitBlueprint.HasValue() && resources.Mappings.Characters.TryGetValue(unit.UnitBlueprint, out var mapping)) {
                _defaultBlueprint = mapping.Portrait;
            }
        }

        public bool IsSupported => Blueprint.HasValue();
        public bool CanSet => false;
        public string Blueprint => _unit.PortraitBlueprint.OrIfEmpty(_defaultBlueprint);
        public string Path => _resources.AppData is null ? null : _resources.GetPortraitsUri(Blueprint);

        public void Set(Portrait portrait) => throw new NotSupportedException();
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

    internal sealed class W40KDHCompanionModel : IGameUnitCompanionModel
    {
        private readonly W40KDHParsedUnitStats _unit;

        public W40KDHCompanionModel(W40KDHParsedUnitStats unit)
        {
            _unit = unit;
            AllStates = new[] { new GameEnumValue(State, State) };
        }

        public bool IsSupported => _unit.CompanionState.HasValue() && !_unit.IsMainCharacter;
        public IReadOnlyList<GameEnumValue> AllStates { get; }
        public string State
        {
            get => _unit.CompanionState switch {
                "InParty" => "In party",
                "InPartyDetached" => "Detached from party",
                "Remote" => "Outside party",
                "ExCompanion" when _unit.FormerCompanionState == "InReserve" => "Former companion in reserve",
                "ExCompanion" => "Former companion",
                _ => "Unknown"
            };
            set => throw new NotSupportedException();
        }
        public bool IsReadOnly => true;
        public bool IsDialogEnabled => false;
        public bool IsInParty => _unit.IsInParty;
        public bool IsExCompanion => _unit.CompanionState == "ExCompanion";
    }

    internal sealed class W40KDHUnsupportedAlignmentModel : IGameUnitAlignmentModel
    {
        public bool IsSupported => false;
        public string DisplayName => string.Empty;
        public int X { get => 0; set => throw new NotSupportedException(); }
        public int Y { get => 0; set => throw new NotSupportedException(); }
        public Alignment Direction => Alignment.TrueNeutral;
        public string LockedAlignmentMask { get => string.Empty; set => throw new NotSupportedException(); }
        public bool IsAlignmentMaskSupported => false;
        public IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; } = GameModelCollection<IGameUnitAlignmentHistoryEntryModel>.Empty;
    }

    internal sealed class W40KDHUnsupportedAsksModel : IGameUnitAsksModel
    {
        public bool IsSupported => false;
        public string Custom { get => string.Empty; set => throw new NotSupportedException(); }
    }

    internal sealed class W40KDHUnsupportedRaceModel : IGameUnitRaceModel
    {
        public bool IsSupported => false;
        public string DisplayName => string.Empty;
    }

    internal sealed class W40KDHGameUnitStatsModel : IGameUnitStatsModel
    {
        public W40KDHGameUnitStatsModel(W40KDHParsedUnitStats unit, IReadOnlyList<W40KDHAdvancementStat> advancements)
        {
            var general = new List<IGameUnitStatsEntry>();
            if (unit.CharacterLevel is int level) general.Add(new Stat("Character Level", level));
            if (unit.Experience is int experience) general.Add(new Stat("Experience", experience));
            if (unit.MissingHpFraction is float missing && float.IsFinite(missing)) {
                general.Add(new Stat("Health %", (int)Math.Round(Math.Clamp(1 - missing, 0f, 1f) * 100)));
            }

            var groupings = new List<GameStatsGrouping>();
            if (general.Count > 0) groupings.Add(new GameStatsGrouping("General", "Source", general));
            if (unit.Characteristics.Count > 0) {
                var characteristics = unit.Characteristics.Select(pair =>
                    (IGameUnitStatsEntry)new Stat(pair.Key, pair.Value.Value)).ToArray();
                groupings.Add(new GameStatsGrouping("Characteristics", "Source", characteristics));
            }
            var characteristicsAdvancements = advancements.Where(stat => stat.Step == 5).ToArray();
            var skillAdvancements = advancements.Where(stat => stat.Step == 10).ToArray();
            if (characteristicsAdvancements.Length > 0) {
                groupings.Add(new GameStatsGrouping("Characteristic advancement bonuses", "Feature rank", characteristicsAdvancements));
            }
            if (skillAdvancements.Length > 0) {
                groupings.Add(new GameStatsGrouping("Skill advancement bonuses", "Feature rank", skillAdvancements));
            }
            Groupings = groupings;
        }

        public bool IsSupported => Groupings.Count > 0;
        public IReadOnlyList<GameStatsGrouping> Groupings { get; }

        private sealed record Stat(string Name, int Value) : IGameUnitStatsEntry
        {
            int IGameUnitStatsEntry.Value { get => Value; set => throw new NotSupportedException(); }
            public string Info => "party.owl";
            public bool IsReadOnly => true;
        }
    }

    internal sealed class W40KDHUnsupportedAppearanceModel : IGameUnitAppearanceModel
    {
        public bool IsSupported => false;
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool CanRestore => false;
        public bool CanDelete => false;
        public void Restore() => throw new NotSupportedException();
        public void Delete() => throw new NotSupportedException();
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

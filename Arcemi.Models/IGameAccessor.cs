using System;
using System.Collections;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameEditFile
    {
        HeaderModel Header { get; }
        PlayerModel Player { get; }
        PartyModel Party { get; }
    }
    public static class SupportedGames
    {
        public static IReadOnlyList<GameDefinition> All { get; } = new[] {
            GameDefinition.Warhammer40K_RogueTrader,
            GameDefinition.Pathfinder_WrathOfTheRighteous,
            GameDefinition.Pathfinder_Kingmaker
        };
        public static IGameAccessor Detect(IGameEditFile file)
        {
            if (PathfinderWotr.WotrGameAccessor.Detect(file)) return new PathfinderWotr.WotrGameAccessor(file);
            if (Kingmaker.KingmakerGameAccessor.Detect(file)) return new Kingmaker.KingmakerGameAccessor(file);
            if (Warhammer40KRogueTrader.W40KRTGameAccessor.Detect(file)) return new Warhammer40KRogueTrader.W40KRTGameAccessor(file);
            return new NotSetGameAccessor();
        }
    }
    public interface IGameAccessor
    {
        GameDefinition Definition { get; }
        string MainCharacterId { get; set; }
        IGamePartyModel Party { get; }
        IGameUnitModel MainCharacter { get; }
        IGameModelCollection<IGameUnitModel> Characters { get; }
        IGameInventoryModel SharedInventory { get; }
        IGameInventoryModel SharedStash { get; }
        IGameManagementModel Management { get; }
        IGameStateModel State { get; }

        void BeforeSave();

        IGameUnitModel GetOwnerOf(IGameUnitModel unit);
        void SetMainCharacter(IGameUnitModel unit);
    }

    public interface IGamePartyModel : IGameModel
    {
        IGameDataObject Data { get; }
    }

    public interface IGameStateModel : IGameModel
    {
        IGameStateQuestBook QuestBook { get; }
        IReadOnlyList<IGameDataObject> Sections { get; }
    }

    public interface IGameStateQuestBook
    {
        IReadOnlyList<IGameStateQuestEntry> Entries { get; }
    }

    public interface IGameStateQuestEntry
    {
        string Name { get; }
        string State { get; set; }
        bool IsCompleted { get; }
        IReadOnlyList<DataOption> StateOptions { get; }
        IReadOnlyList<IGameStateQuestObjectiveEntry> Objectives { get; }
    }

    public interface IGameStateQuestObjectiveEntry
    {
        string Name { get; }
        string State { get; set; }
        IReadOnlyList<DataOption> StateOptions { get; }
    }

    public class NotSetGameAccessor : IGameAccessor
    {
        public GameDefinition Definition => GameDefinition.NotSet;
        public IGameUnitModel MainCharacter { get; }
        public IGameModelCollection<IGameUnitModel> Characters { get; } = new GameModelCollection<IGameUnitModel, UnitEntityModel>(null, null);
        public IGameInventoryModel SharedInventory { get; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; }
        public IGamePartyModel Party { get; }
        public IGameStateModel State { get; }
        public string MainCharacterId { get; set; }

        public void BeforeSave()
        {
            throw new NotSupportedException("Unknown game, unable to save");
        }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit)
        {
            throw new NotSupportedException("Unknown game, unable to get owner");
        }

        public void SetMainCharacter(IGameUnitModel unit)
        {
            throw new NotSupportedException("Unknown game, unable to set main character");
        }
    }

    public interface IGameInventoryModel : IGameModel
    {
        IReadOnlyList<IGameItemSection> Sections { get; }
        IGameItemEntry FindEquipped(object itemRef);
    }
    public interface IGameItemSection
    {
        string Name { get; }
        IGameModelCollection<IGameItemEntry> Items { get; }
        IReadOnlyList<BlueprintType> AddableTypes { get; }
        IEnumerable<IBlueprintMetadataEntry> GetAddableItems(string typeFullName = null);
    }
    public interface IGameItemEntry
    {
        string Name { get; }
        string Blueprint { get; }
        string UniqueId { get; }
        string Type { get; }
        string Description { get; }
        int Index { get; }
        bool IsChargable { get; }
        int Charges { get; set; }
        bool IsStackable { get; }
        int Count { get; set; }
        bool CanEdit { get; }
        string IconUrl { get; }
        bool IsLocked { get; set; }
    }

    public interface IGameUnitModel : IGameModel
    {
        string UniqueId { get; }
        string Name { get; }
        string CustomName { get; set; }
        string DefaultName { get; }
        UnitEntityType Type { get; }
        UnitEntityModel Ref { get; }
        IGameUnitPortraitModel Portrait { get; }
        IGameUnitCompanionModel Companion { get; }
        IGameDataObject Overview { get; }
        IGameUnitAlignmentModel Alignment { get; }
        IGameUnitAsksModel Asks { get; }
        IGameUnitRaceModel Race { get; }
        IGameUnitProgressionModel Progression { get; }
        IGameUnitStatsModel Stats { get; }
        IGameUnitAppearanceModel Appearance { get; }
        IGameUnitBodyModel Body { get; }
        IGameUnitSpellCasterModel SpellCaster { get; }
        IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        IReadOnlyList<IGameDataObject> Sections { get; }

        void ReplacePartyMemberWith(IGameUnitModel unit);
        void AddToRetinue();
    }

    public interface IGameUnitSpellCasterModel : IGameModel
    {
        IGameUnitSpellCasterBonusSpellModel BonusSpells { get; }
        IGameModelCollection<IGameUnitSpellBookEntry> SpellBooks { get; }
    }

    public interface IGameUnitSpellCasterBonusSpellModel : IGameModel
    {
        bool IsUnlocked { get; }
        IEnumerable<SpellIndexAccessor> Accessors { get; }
        void Unlock();
    }

    public interface IGameUnitSpellBookEntry
    {
        string Name { get; }
        string Type { get; }
        int Level { get; set; }
        bool IsModifierSupported { get; }
        string ModifierName { get; }
        int Modifier { get; set; }

        IGameSpellSlotCollection<IGameSpellEntry> KnownSpells { get; }
        IGameSpellSlotCollection<IGameSpellEntry> SpecialSpells { get; }
        IGameSpellSlotCollection<IGameCustomSpellEntry> CustomSpells { get; }
        IGameSpellSlotCollection<IGameMemorizedSpellEntry> MemorizedSpells { get; }

        IGameMagicInfusionSpellSlotCollection MagicInfusions { get; }

        ListValueAccessor<string> SpecialLists { get; }
        ListValueAccessor<string> OppositionSchools { get; }
        IEnumerable<SpellIndexAccessor> SpontaneousSlots { get; }

        void EnableCustomSpells();
    }

    public interface IGameMagicInfusionSpellSlotCollection : IEnumerable<IGameMagicInfusionSpellEntry>
    {
        bool CanAdd { get; }
        IReadOnlyList<IGameMagicInfusionSpellEntry> Slots { get; }

        IGameMagicInfusionSpellEntry Add();
        void Remove(IGameMagicInfusionSpellEntry entry);

        void SetSpellLevel(IGameMagicInfusionSpellEntry spell, int level);
    }

    public interface IGameSpellEntry
    {
        string Name { get; }
        string Blueprint { get; }
    }

    public interface IGameMagicInfusionSpellEntry
    {
        int SlotId { get; }
        string Spell1Blueprint { get; set; }
        string Spell1Name { get; }
        string Spell2Blueprint { get; set; }
        string Spell2Name { get; }
        int SpellLevel { get; set; }

        string SpellSchool { get; set; }
        IEnumerable<DataOption> SpellSchools { get; }

        string SavingThrowType { get; set; }
        IEnumerable<DataOption> SavingThrowTypes { get; }

        string SpellTargetType { get; set; }
        IEnumerable<DataOption> SpellTargetTypes { get; }

        bool IsTouch { get; set; }
        string DeliverBlueprint { get; set; }
        string AdditionalAoeBlueprint { get; set; }
    }

    public interface IGameCustomSpellEntry : IGameSpellEntry
    {
        int DecorationColor { get; set; }
        int DecorationBorder { get; set; }
        int SpellLevelCost { get; set; }
        int HeightenLevel { get; set; }
        MetamagicCollection Metamagic { get; }
    }
    public interface IGameMemorizedSpellEntry : IGameSpellEntry
    {
        bool IsAvailable { get; set; }
        IGameSpellEntry Reference { get; }
    }

    public interface IGameSpellSlotCollection<T> where T : IGameSpellEntry
    {
        IReadOnlyList<T> this[int index] { get; }
        int Count { get; }
        bool CanAddNew { get; }
        bool CanAddReference { get; }
        bool CanRemove { get; }
        T AddNew(int index, string blueprint);
        T AddReference(int index, IGameSpellEntry reference);
        bool Remove(T item);
    }

    public interface IGameUnitBodyModel : IGameModel
    {
        IReadOnlyList<IGameUnitHandsEquippedEntry> HandsEquipmentSets { get; }
        IReadOnlyList<IGameUnitEquippedEntry> QuickSlots { get; }

        IGameUnitEquippedEntry Armor { get; }
        IGameUnitEquippedEntry Shirt { get; }
        IGameUnitEquippedEntry Belt { get; }
        IGameUnitEquippedEntry Head { get; }
        IGameUnitEquippedEntry Feet { get; }
        IGameUnitEquippedEntry Gloves { get; }
        IGameUnitEquippedEntry Neck { get; }
        IGameUnitEquippedEntry Glasses { get; }
        IGameUnitEquippedEntry Ring1 { get; }
        IGameUnitEquippedEntry Ring2 { get; }
        IGameUnitEquippedEntry Wrist { get; }
        IGameUnitEquippedEntry Shoulders { get; }
    }

    public interface IGameUnitHandsEquippedEntry
    {
        IGameUnitEquippedEntry Primary { get; }
        IGameUnitEquippedEntry Secondary { get; }
    }
    public interface IGameUnitEquippedEntry
    {
        object ItemRef { get; }
    }

    public interface IGameUnitFactEntry
    {
        string DisplayName { get; }
        string Blueprint { get; }
    }

    public interface IGameUnitAppearanceModel : IGameModel
    {
        IReadOnlyList<IGameUnitDollModel> Dolls { get; }
    }

    public interface IGameUnitDollModel : IGameModel
    {
        string Name { get; }
        string Gender { get; set; }
        IReadOnlyList<GenderOption> GenderOptions { get; }
        string Race { get; set; }
        IReadOnlyList<BlueprintOption> RaceOptions { get; }

        int ClothesPrimaryIndex { get; set; }
        int ClothesSecondaryIndex { get; set; }
        ListValueAccessor<string> EquipmentEntityIds { get; }
        IReadOnlyList<GameKeyValueEntry<int>> EntityRampIdices { get; }
        void AddEntityRampIdices(GameKeyValueEntry<int> item);
        void RemoveEntityRampIdices(GameKeyValueEntry<int> item);
        IReadOnlyList<GameKeyValueEntry<int>> EntitySecondaryRampIdices { get; }
        void AddEntitySecondaryRampIdices(GameKeyValueEntry<int> item);
        void RemoveEntitySecondaryRampIdices(GameKeyValueEntry<int> item);

        string Export();
        void Import(string code);
    }

    public interface IGameUnitBuffEntry
    {
        string DisplayName { get; }
        string Blueprint { get; }
        bool IsActive { get; set; }
        TimeParts Duration { get; }
    }

    public interface IGameUnitAbilityEntry
    {
        string DisplayName { get; }
        string Blueprint { get; }
        bool IsActive { get; set; }
    }

    public interface IGameUnitFeatEntry
    {
        string DisplayName { get; }
        string Tooltip { get; }
        string Blueprint { get; }
        string Category { get; }
        bool IsRanked { get; }
        int Rank { get; set; }

        /// <summary>
        /// Last level this feat was updated
        /// </summary>
        int SourceLevel { get; }

        string Export();
    }

    public interface IGameUnitStatsModel : IGameModel
    {
        IReadOnlyList<GameStatsGrouping> Groupings { get; }
    }

    public class GameStatsGrouping : IReadOnlyList<IGameUnitStatsEntry>
    {
        private readonly IReadOnlyList<IGameUnitStatsEntry> _entries;
        public string Name { get; }
        public string InfoHeader { get; }

        public int Count => _entries.Count;
        public IGameUnitStatsEntry this[int index] => _entries[index];
        public GameStatsGrouping(string name, string infoHeader, IReadOnlyList<IGameUnitStatsEntry> entries)
        {
            Name = name;
            InfoHeader = infoHeader;
            _entries = entries;
        }
        public IEnumerator<IGameUnitStatsEntry> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IGameUnitStatsEntry
    {
        string Name { get; }
        int Value { get; set; }
        string Info { get; }
    }

    public interface IGameUnitProgressionModel : IGameModel
    {
        int Experience { get; set; }
        int CurrentLevel { get; set; }
        bool IsLevelReadOnly { get; }
        bool IsSelectionsRepairable { get; }

        IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; }
        IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; }
        IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; }

        IGameDataObject Data { get; }

        void RepairSelections();
    }

    public interface IGameUnitSelectionProgressionEntry
    {
        string Name { get; }
        string Feature { get; set; }
        int Level { get; }
        bool IsCreation { get; }
        IReadOnlyList<BlueprintOption> Options { get; }
        bool IsReadOnly { get; }
    }

    public interface IGameUnitClassProgressionEntry
    {
        string Name { get; }
        string SpecializationName { get; }
        int Level { get; }
    }

    public interface IGameUnitUltimateProgressionEntry
    {
        string Name { get; }
        int Level { get; set; }
        int MinLevel { get; }
        int MaxLevel { get; }
    }

    public interface IGameUnitAsksModel : IGameModel
    {
        string Custom { get; set; }
    }

    public interface IGameUnitRaceModel : IGameModel
    {
        string DisplayName { get; }
    }

    public interface IGameModel
    {
        bool IsSupported { get; }
    }

    public interface IGameUnitCompanionModel : IGameModel
    {
        IReadOnlyList<GameEnumValue> AllStates { get; }
        string State { get; set; }
        bool IsReadOnly { get; }
        bool IsDialogEnabled { get; }
        bool IsInParty { get; }
        bool IsExCompanion { get; }
    }

    public interface IGameUnitAlignmentModel : IGameModel
    {
        string DisplayName { get; }
        int X { get; set; }
        int Y { get; set; }
        Alignment Direction { get; }
        string LockedAlignmentMask { get; set; }
        bool IsAlignmentMaskSupported { get; }
        IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; }
    }

    public interface IGameUnitAlignmentHistoryEntryModel
    {
        string Direction { get; }
        string Provider { get; }
        int X { get; }
        int Y { get; }
    }

    public interface IGameUnitPortraitModel : IGameModel
    {
        string Blueprint { get; }
        string Path { get; }

        void Set(Portrait portrait);
    }

    public enum UnitEntityType
    {
        Player,
        Pet,
        Mercenary,
        Companion,
        Starship,
        Other
    }

    public static class UnitEntityTypeExtensions
    {
        public static bool IsCharacter(this UnitEntityType entityType)
        {
            switch (entityType) {
                case UnitEntityType.Player:
                case UnitEntityType.Mercenary:
                case UnitEntityType.Companion:
                case UnitEntityType.Pet:
                    return true;
            }
            return false;
        }

        public static bool IsDisplayedInCharactersPage(this UnitEntityType entityType)
        {
            return IsCharacter(entityType)
                || entityType == UnitEntityType.Starship;
        }
    }
}

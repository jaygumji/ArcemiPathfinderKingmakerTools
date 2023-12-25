using Newtonsoft.Json.Linq;
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
            GameDefinition.Pathfinder_WrathOfTheRighteous
        };
        public static IGameAccessor Detect(IGameEditFile file)
        {
            if (PathfinderWotr.WotrGameAccessor.Detect(file)) return new PathfinderWotr.WotrGameAccessor(file);
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
        IReadOnlyList<IGamePartyResourceEntry> Resources { get; }
    }

    public interface IGamePartyResourceEntry
    {
        string Name { get; }
        int Value { get; set; }
        bool IsSmall { get; }
        bool IsReadOnly { get; }
    }

    public interface IGameStateModel : IGameModel
    {
        
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
        string Type { get; }
        string Description { get; }
        int Index { get; }
        bool IsChargable { get; }
        int Charges { get; set; }
        bool IsStackable { get; }
        int Count { get; set; }
        bool CanEdit { get; }
        string IconUrl { get; }
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
        IGameUnitAlignmentModel Alignment { get; }
        IGameUnitAsksModel Asks { get; }
        IGameUnitRaceModel Race { get; }
        IGameUnitProgressionModel Progression { get; }
        IGameUnitStatsModel Stats { get; }
        IGameUnitAppearanceModel Appearance { get; }
        IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        IReadOnlyList<IGameUnitFactSection> FactSections { get; }
    }

    public interface IGameUnitFactSection
    {
        string Name { get; }
        IGameModelCollection<IGameUnitFactEntry> Items { get; }
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

        void RepairSelections();
    }

    public interface IGameUnitSelectionProgressionEntry
    {
        string Name { get; }
        string Feature { get; set; }
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
    }

    public interface IGameUnitAlignmentModel : IGameModel
    {
        string DisplayName { get; }
        int X { get; set; }
        int Y { get; set; }
        Alignment Direction { get; }
        string LockedAlignmentMask { get; set; }
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

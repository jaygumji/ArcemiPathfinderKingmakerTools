using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameManagementModel : IGameModel
    {
        string DisplayName { get; }
        ModelTypeName MemberTypeName { get; }
        ModelTypeName PlacesTypeName { get; }
        bool IsOverviewEnabled { get; }
        bool IsMembersEnabled { get; }
        bool IsArmiesEnabled { get; }
        bool IsTasksEnabled { get; }
        bool IsPlacesEnabled { get; }
        IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        IGameModelCollection<IGameManagementPlaceModelEntry> Places { get; }
    }

    public interface IGameManagementPlaceModelEntry
    {
        string Name { get; }
        string Blueprint { get; }
        IReadOnlyList<GameManagementPlaceModelDataGrouping> DataGroupings { get; }
    }

    public class GameManagementPlaceModelDataGrouping
    {
        public string Name { get; }
        public IReadOnlyList<IGameManagementPlaceModelDataGroupingEntry> Entries { get; }

        public GameManagementPlaceModelDataGrouping(string name, IReadOnlyList<IGameManagementPlaceModelDataGroupingEntry> entries)
        {
            Name = name;
            Entries = entries;
        }
    }

    public interface IGameManagementPlaceModelDataGroupingEntry
    {
        string Label { get; }
    }

    public interface IGameManagementPlaceModelDataGroupingOptionsEntry : IGameManagementPlaceModelDataGroupingEntry
    {
        IReadOnlyList<DataOption> Options { get; }
        string Value { get; set; }
    }

    public interface IGameManagementPlaceModelDataGroupingIntegerEntry : IGameManagementPlaceModelDataGroupingEntry
    {
        int Value { get; set; }
        int MinValue { get; }
        int MaxValue { get; }
        int Modifiers { get; }
    }

    public interface IGameManagementTaskModelEntry
    {
    }

    public interface IGameManagementArmyModelEntry
    {
    }

    public interface IGameManagementMemberModelEntry
    {
        string Name { get; }
        string UniqueId { get; }
        string Blueprint { get; }
        string PortraitPath { get; }
        int Experience { get; set; }
        int Level { get; set; }
        int CurrentMana { get; set; }
    }
}
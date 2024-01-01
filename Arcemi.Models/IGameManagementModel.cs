using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameManagementModel : IGameModel
    {
        string DisplayName { get; }
        ModelTypeName MemberTypeName { get; }
        bool IsOverviewEnabled { get; }
        bool IsMembersEnabled { get; }
        bool IsArmiesEnabled { get; }
        bool IsTasksEnabled { get; }
        IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        IGameDataObject Resources { get; }
        IGameDataObject Places { get; }
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
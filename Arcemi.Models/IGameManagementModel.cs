using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameManagementModel : IGameModel
    {
        string DisplayName { get; }
        string MemberTypeName { get; }
        string PlacesTypeName { get; }
        bool IsArmiesEnabled { get; }
        bool IsPlacesEnabled { get; }
        IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        IGameModelCollection<IGameManagementPlaceModelEntry> Places { get; }
    }

    public interface IGameManagementPlaceModelEntry
    {
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
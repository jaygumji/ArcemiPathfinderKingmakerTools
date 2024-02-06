using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameManagementModel : IGameModel
    {
        string DisplayName { get; }
        ModelTypeName MemberTypeName { get; }
        IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        IGameDataObject Overview { get; }
        IGameDataObject Resources { get; }
        IGameDataObject Places { get; }
    }

    public interface IGameManagementTaskModelEntry
    {
        string Name { get; }
        string Description { get; }
        string Type { get; }
        int StartedOn { get; set; }
    }

    public interface IGameManagementArmyModelEntry
    {
    }

    public interface IGameManagementMemberModelEntry
    {
        string Name { get; }
        string Blueprint { get; }
        string PortraitPath { get; }
        IGameDataObject Overview { get; }
    }
}
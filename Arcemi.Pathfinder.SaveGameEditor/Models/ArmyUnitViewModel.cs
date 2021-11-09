using Arcemi.Pathfinder.Kingmaker;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class ArmyUnitViewModel
    {
        public ArmyUnitViewModel(PlayerArmySquadModel squad, ArmyUnitDataMapping mapping, int positionIndex)
        {
            Squad = squad;
            Mapping = mapping;
            PositionIndex = positionIndex;
        }

        public PlayerArmySquadModel Squad { get; }
        public ArmyUnitDataMapping Mapping { get; }
        public int PositionIndex { get; }
        public string Name => Mapping.Name.OrIfEmpty(Squad.DisplayName);
    }
}

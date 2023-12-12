using Arcemi.Models;

namespace Arcemi.SaveGameEditor.Models
{
    public class ArmyUnitViewModel
    {
        private readonly IGameResourcesProvider _res;

        public ArmyUnitViewModel(IGameResourcesProvider res, PlayerArmySquadModel squad, ArmyUnitDataMapping mapping, int positionIndex)
        {
            _res = res;
            Squad = squad;
            Mapping = mapping;
            PositionIndex = positionIndex;
        }

        public PlayerArmySquadModel Squad { get; }
        public ArmyUnitDataMapping Mapping { get; }
        public int PositionIndex { get; }
        public string Name => Mapping.Name.OrIfEmpty(_res.GetArmyUnitName(Squad.Unit));
    }
}

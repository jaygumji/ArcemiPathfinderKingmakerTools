using System.Diagnostics;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitRaceModel : IGameUnitRaceModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameUnitRaceModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public UnitEntityModel Unit { get; }
        public string DisplayName => Res.GetRaceName(Unit.Descriptor.Progression.Race);
        public bool IsSupported => true;
    }
}
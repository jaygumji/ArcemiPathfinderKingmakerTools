namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitRaceModel : IGameUnitRaceModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitRaceModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public UnitEntityModel Unit { get; }
        public string DisplayName => Res.GetRaceName(Unit.Descriptor.Progression.Race);
        public bool IsSupported => true;
    }
}
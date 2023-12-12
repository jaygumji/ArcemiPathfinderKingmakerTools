namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitAsksModel : IGameUnitAsksModel
    {
        public WotrGameUnitAsksModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public string Custom { get => Unit.Descriptor.CustomAsks; set => Unit.Descriptor.CustomAsks = value; }

        public bool IsSupported => true;

        public UnitEntityModel Unit { get; }
    }
}
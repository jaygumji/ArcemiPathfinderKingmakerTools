namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAsksModel : IGameUnitAsksModel
    {
        public KingmakerGameUnitAsksModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public string Custom { get => Unit.Descriptor.CustomAsks; set => Unit.Descriptor.CustomAsks = value; }

        public bool IsSupported => true;

        public UnitEntityModel Unit { get; }
    }
}
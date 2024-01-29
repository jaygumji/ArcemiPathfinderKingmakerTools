namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAlignmentModel : IGameUnitAlignmentModel
    {
        public bool IsSupported => false; // (Unit?.Descriptor?.Alignment?.Vector) is object;
        public string DisplayName => Unit.Descriptor.Alignment.Vector.DisplayName;
        public int X { get => Unit.Descriptor.Alignment.Vector.X; set => Unit.Descriptor.Alignment.Vector.X = value; }
        public int Y { get => Unit.Descriptor.Alignment.Vector.Y; set => Unit.Descriptor.Alignment.Vector.Y = value; }
        public Alignment Direction => Unit.Descriptor.Alignment.Vector.Direction;
        public string LockedAlignmentMask { get => Unit.Descriptor.Alignment.LockedAlignmentMask; set => Unit.Descriptor.Alignment.LockedAlignmentMask = value; }
        public IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; }

        public KingmakerGameUnitAlignmentModel(UnitEntityModel unit)
        {
            Unit = unit;
            //History = new GameModelCollection<IGameUnitAlignmentHistoryEntryModel, AlignmentHistoryModel>(unit.Descriptor?.Alignment?.History, a => new KingmakerGameUnitAlignmentHistoryEntryModel(a));
            History = GameModelCollection<IGameUnitAlignmentHistoryEntryModel>.Empty;
        }

        public UnitEntityModel Unit { get; }
    }
}
using System;

namespace Arcemi.Models.Kingmaker
{
    public static class KingmakerAlignmentScale
    {
        public static int ToView(double? value)
        {
            if (value == null) return 0;
            return (int)Math.Round(value.Value * 100.0);
        }

        public static double ToModel(int value)
        {
            return value / 100.0;
        }
    }
    internal class KingmakerGameUnitAlignmentModel : IGameUnitAlignmentModel
    {
        public VectorModel Vector => A.Object<VectorModel>();
        public bool IsSupported => Ref is object;
        public string DisplayName => Direction.ToString().AsDisplayable();

        public int X { get => KingmakerAlignmentScale.ToView(Vector?.X); set { if (Vector is object) { Vector.X = KingmakerAlignmentScale.ToModel(value); } } }
        public int Y { get => KingmakerAlignmentScale.ToView(Vector?.Y); set { if (Vector is object) { Vector.Y = KingmakerAlignmentScale.ToModel(value); } } }
        public Alignment Direction => Vector is object ? AlignmentExtensions.Detect(X, Y) : Alignment.None;
        public bool IsAlignmentMaskSupported => false;
        public string LockedAlignmentMask { get; set; }
        public IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; }

        public KingmakerGameUnitAlignmentModel(UnitEntityModel unit)
        {
            Unit = unit;
            Ref = unit?.Descriptor?.GetAccessor().Object<RefModel>("Alignment");
            A = Ref?.GetAccessor();
            History = new GameModelCollection<IGameUnitAlignmentHistoryEntryModel, RefModel>(A?.List<RefModel>("m_History"), a => new KingmakerGameUnitAlignmentHistoryEntryModel(a), writer: new KingmakerGameUnitAlignmentHistoryCollectionWriter());
        }

        public UnitEntityModel Unit { get; }
        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }
    }
}
namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAlignmentHistoryEntryModel : IGameUnitAlignmentHistoryEntryModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public string Direction { get => A.Value<string>(); set => A.Value(value); }
        public string Provider => Res.Blueprints.GetNameOrBlueprint(A.Value<string>());
        private VectorModel Position => A.Object<VectorModel>();
        public int X => KingmakerAlignmentScale.ToView(Position?.X);
        public int Y => KingmakerAlignmentScale.ToView(Position?.Y);

        public KingmakerGameUnitAlignmentHistoryEntryModel(RefModel @ref)
        {
            Ref = @ref;
            A = Ref.GetAccessor();
        }

        public void Set(double x, double y)
        {
            Position.X = x;
            Position.Y = y;
            Direction = AlignmentExtensions.Detect(X, Y).ToString();
        }

        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }
    }
}
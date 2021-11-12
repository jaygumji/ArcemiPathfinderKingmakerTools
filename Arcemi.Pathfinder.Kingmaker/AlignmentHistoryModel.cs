namespace Arcemi.Pathfinder.Kingmaker
{
    public class AlignmentHistoryModel : RefModel
    {
        public AlignmentHistoryModel(ModelDataAccessor accessor) : base(accessor)
        {
            Vector = new VectorView(() => Position, v => Position = v);
        }

        public VectorView Vector { get; }
        public string Position { get => A.Value<string>(); set => A.Value(value); }
        public string Direction { get => A.Value<string>(); set => A.Value(value); }
        public string Provider { get => A.Value<string>(); set => A.Value(value); }
        public string ProviderDisplayName => A.Res.Blueprints.GetNameOrBlueprint(Provider);
    }
}
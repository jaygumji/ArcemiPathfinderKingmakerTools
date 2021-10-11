namespace Arcemi.Pathfinder.Kingmaker
{
    public class AlignmentHistoryModel : RefModel
    {
        public AlignmentHistoryModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Position { get => A.Value<string>(); set => A.Value(value); }
        public string Direction { get => A.Value<string>(); set => A.Value(value); }
        public string Provider { get => A.Value<string>(); set => A.Value(value); }
    }
}
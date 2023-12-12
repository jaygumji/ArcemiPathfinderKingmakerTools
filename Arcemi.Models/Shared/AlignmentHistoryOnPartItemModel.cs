namespace Arcemi.Models
{
    public class AlignmentHistoryOnPartItemModel : RefModel
    {
        public AlignmentHistoryOnPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public VectorModel Position => A.Object<VectorModel>();
        public string Direction { get => A.Value<string>(); set => A.Value(value); }
        public string Provider { get => A.Value<string>(); set => A.Value(value); }
    }
}
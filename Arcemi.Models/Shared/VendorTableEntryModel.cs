namespace Arcemi.Models
{
    public class VendorTableEntryModel : RefModel
    {
        public VendorTableEntryModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Count { get => A.Value<int>(); set => A.Value(value); }
        public string Item { get => A.Value<string>(); set => A.Value(value); }
    }
}
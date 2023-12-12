namespace Arcemi.Models
{
    public class AbilityResourceModel : RefModel
    {
        public AbilityResourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public int Amount { get => A.Value<int>(); set => A.Value(value); }
        public int RetainCount { get => A.Value<int>(); set => A.Value(value); }
    }
}
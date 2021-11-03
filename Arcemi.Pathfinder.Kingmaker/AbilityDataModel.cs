namespace Arcemi.Pathfinder.Kingmaker
{
    public class AbilityDataModel : RefModel
    {
        public AbilityDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
    }
}
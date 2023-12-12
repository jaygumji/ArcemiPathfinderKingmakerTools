namespace Arcemi.Models
{
    public class FeatureParamModel : Model
    {
        public FeatureParamModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string WeaponCategory { get => A.Value<string>(); set => A.Value(value); }
        public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
    }
}
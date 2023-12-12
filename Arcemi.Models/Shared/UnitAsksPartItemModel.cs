namespace Arcemi.Models
{
    public class UnitAsksPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartUnitAsks, Code";

        public UnitAsksPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string CustomAsks { get => A.Value<string>(); set => A.Value(value); }
        public string OverrideAsks { get => A.Value<string>(); set => A.Value(value); }
    }
}
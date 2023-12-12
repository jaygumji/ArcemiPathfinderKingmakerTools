namespace Arcemi.Models
{
    public class UnitDescriptionPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartUnitDescription, Code";

        public UnitDescriptionPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string CustomGender { get => A.Value<string>(); set => A.Value(value); }
        public string CustomName { get => A.Value<string>(); set => A.Value(value); }
        public bool ForceUseClassEquipment { get => A.Value<bool>(); set => A.Value(value); }
    }
}
namespace Arcemi.Models
{
    public class UnitCompanionPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartCompanion, Code";

        public UnitCompanionPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string State { get => A.Value<string>(); set => A.Value(value); }
    }
}
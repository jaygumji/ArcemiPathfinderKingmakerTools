namespace Arcemi.Models
{
    public class UnitViewSettingsPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartUnitViewSettings, Code";

        public UnitViewSettingsPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public DollDataModel Doll => A.Object<DollDataModel>();
    }
}
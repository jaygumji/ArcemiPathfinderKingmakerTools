namespace Arcemi.Models
{
    public class UnitUISettingsPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UI.Models.UnitSettings.PartUnitUISettings, Code";

        public UnitUISettingsPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool m_ShowHelm { get => A.Value<bool>(); set => A.Value(value); }
        public bool m_ShowBackpack { get => A.Value<bool>(); set => A.Value(value); }
        public string m_Portrait { get => A.Value<string>(); set => A.Value(value); }
        public CustomPortraitModel m_CustomPortrait => A.Object<CustomPortraitModel>();
    }
}
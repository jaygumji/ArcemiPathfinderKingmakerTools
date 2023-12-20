namespace Arcemi.Models
{
    public class UnitViewSettingsPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartUnitViewSettings, Code";

        public UnitViewSettingsPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public W40KRTDollModel Doll => A.Object<W40KRTDollModel>();
    }
    public class W40KRTDollModel : RefModel
    {
        public W40KRTDollModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<string> EquipmentEntityIds => A.ListValue<string>();
        public ListAccessor<KeyValuePairModel<int>> EntityRampIdices => A.List<KeyValuePairModel<int>>();
        public ListAccessor<KeyValuePairModel<int>> EntitySecondaryRampIdices => A.List<KeyValuePairModel<int>>();
        public string Gender { get => A.Value<string>(); set => A.Value(value); }
        public string RacePreset { get => A.Value<string>(); set => A.Value(value); }
        public int ClothesPrimaryIndex { get => A.Value<int>(); set => A.Value(value); }
        public int ClothesSecondaryIndex { get => A.Value<int>(); set => A.Value(value); }
    }

}
namespace Arcemi.Models
{
    public class DollDataModel : RefModel
    {
        public DollDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<string> EquipmentEntityIds => A.ListValue<string>();
        public DictionaryOfValueAccessor<int> EntityRampIdices => A.DictionaryOfValue<int>();
        public DictionaryOfValueAccessor<int> EntitySecondaryRampIdices => A.DictionaryOfValue<int>();
        public string Gender { get => A.Value<string>(); set => A.Value(value); }
        public string RacePreset { get => A.Value<string>(); set => A.Value(value); }
        public int ClothesPrimaryIndex { get => A.Value<int>(); set => A.Value(value); }
        public int ClothesSecondaryIndex { get => A.Value<int>(); set => A.Value(value); }
    }
}
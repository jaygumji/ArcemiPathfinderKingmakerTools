namespace Arcemi.Pathfinder.Kingmaker
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
        public string RacePresetName => A.Res.Blueprints.GetNameOrBlueprint(RacePreset);
        public int ClothesPrimaryIndex { get => A.Value<int>(); set => A.Value(value); }
        public int ClothesSecondaryIndex { get => A.Value<int>(); set => A.Value(value); }

        public string Export()
        {
            return A.Export();
        }

        public void Import(string script)
        {
            var obj = A.CreateImportView(script, a => new DollDataModel(a));
            EquipmentEntityIds.Clear();
            EquipmentEntityIds.AddRange(obj.EquipmentEntityIds);

            EntityRampIdices.Clear();
            EntityRampIdices.AddRange(obj.EntityRampIdices);

            EntitySecondaryRampIdices.Clear();
            EntitySecondaryRampIdices.AddRange(obj.EntitySecondaryRampIdices);

            ClothesPrimaryIndex = obj.ClothesPrimaryIndex;
            ClothesSecondaryIndex = obj.ClothesSecondaryIndex;

            RacePreset = obj.RacePreset;
            Gender = obj.Gender;
            //A.Import(script);
        }
    }
}
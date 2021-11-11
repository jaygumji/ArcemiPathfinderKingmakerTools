namespace Arcemi.Pathfinder.Kingmaker
{
    public class CustomSpellModel : RefModel
    {
        public CustomSpellModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public int DecorationColorNumber { get => A.Value<int>(); set => A.Value(value); }
        public int DecorationBorderNumber { get => A.Value<int>(); set => A.Value(value); }
        public CustomSpellMetamagicDataModel MetamagicData => A.Object(factory: a => new CustomSpellMetamagicDataModel(a));
    }
}
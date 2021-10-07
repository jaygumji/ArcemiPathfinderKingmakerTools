namespace Arcemi.Pathfinder.Kingmaker
{
    public class CharacterResourceModel : RefModel
    {
        public CharacterResourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public int Amount { get => A.Value<int>(); set => A.Value(value); }
        public int RetainCount { get => A.Value<int>(); set => A.Value(value); }
    }
}
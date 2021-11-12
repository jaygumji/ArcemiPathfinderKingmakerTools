namespace Arcemi.Pathfinder.Kingmaker
{
    public class SpellModel : RefModel
    {
        public SpellModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
    }
}
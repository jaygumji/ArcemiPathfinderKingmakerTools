namespace Arcemi.Pathfinder.Kingmaker
{
    public class FlagModel : Model
    {
        public FlagModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name => A.Res.Blueprints.GetNameOrBlueprint(Key);
        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public int Value { get => A.Value<int>(); set => A.Value(value); }
    }
}
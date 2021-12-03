namespace Arcemi.Pathfinder.Kingmaker
{
    public class ParentContextModel : Model
    {
        public ParentContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssociatedBlueprint { get => A.Value<string>(); set => A.Value(value); }
    }
}
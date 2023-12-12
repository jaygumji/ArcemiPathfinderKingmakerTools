namespace Arcemi.Models
{
    public class ParentContextModel : Model
    {
        public ParentContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public string AssociatedBlueprint { get => A.Value<string>(); set => A.Value(value); }
        public string OwnerRef { get => A.Value<string>("m_OwnerRef"); set => A.Value(value, "m_OwnerRef"); }
    }
}
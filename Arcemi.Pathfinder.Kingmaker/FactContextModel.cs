namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactContextModel : RefModel
    {
        public FactContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssociatedBlueprint { get => A.Value<string>(); set => A.Value(value); }
        public string OwnerRef { get => A.Value<string>("m_OwnerRef"); set => A.Value(value, "m_OwnerRef"); }
        public string SourceItemRef { get => A.Value<string>("m_SourceItemRef"); set => A.Value(value, "m_SourceItemRef"); }

        public FactContextModel ParentContext => A.Object(factory: a => new FactContextModel(a));

        //public CharacterModel OwnerDescriptor => A.Object<CharacterModel>("m_OwnerDescriptor");

        //public IReadOnlyList<int> Ranks => A.ListValue<int>("m_Ranks");
        //public IReadOnlyList<int> SharedValues => A.ListValue<int>("m_SharedValues");
        //public string SpellDescriptor { get => A.Value<string>(); set => A.Value(value); }
        //public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
        //public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }
    }
}
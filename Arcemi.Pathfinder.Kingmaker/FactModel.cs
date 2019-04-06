namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactModel : RefModel
    {

        public FactModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public int Rank { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public bool IgnorePrerequisites { get => A.Value<bool>(); set => A.Value(value); }
        public bool Initialized { get => A.Value<bool>(); set => A.Value(value); }
        public bool Active { get => A.Value<bool>(); set => A.Value(value); }
        public CharacterModel Owner { get => A.Object<CharacterModel>(); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); }
    }
}
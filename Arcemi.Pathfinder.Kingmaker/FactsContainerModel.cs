namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactsContainerModel : RefModel
    {
        public FactsContainerModel(ModelDataAccessor accessor) : base(accessor) { }

        public ListAccessor<FactItemModel> Items => A.List("m_Facts", FactItemModel.Factory);
    }

    public class FactItemModel : RefModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string DisplayName => Mappings.GetFactName(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); }

        public static FactItemModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, "Kingmaker.UnitLogic.Feature, Assembly-CSharp", System.StringComparison.Ordinal)) {
                return new FeatureFactItemModel(accessor);
            }
            return new FactItemModel(accessor);
        }
    }

    public class FeatureFactItemModel : FactItemModel
    {
        public FeatureFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public int Rank { get => A.Value<int>(); set => A.Value(value); }
        public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));
    }
}
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class FeatureFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Feature, Assembly-CSharp";
        public FeatureFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public bool IsRanked => RankToSource?.Count > 0;
        public int Rank { get => A.Value<int>() > 0 ? A.Value<int>() : IsRanked ? 1 : 0; set => A.Value(value); }
        public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add(nameof(RankToSource), new JArray());
            FactItemModel.Prepare(refs, obj);
        }
    }
}
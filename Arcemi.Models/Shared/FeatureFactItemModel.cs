using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class FeatureFactItemModel : FactItemModel
    {
        public const string TypeRef = "Models.UnitLogic.Feature, Assembly-CSharp";
        public FeatureFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public override string DisplayName(IGameResourcesProvider res)
        {
            if (Param != null) {
                if (!string.IsNullOrEmpty(Param.WeaponCategory)) {
                    return string.Concat(base.DisplayName(res), " - ", Param.WeaponCategory.AsDisplayable());
                }
                if (!string.IsNullOrEmpty(Param.SpellSchool)) {
                    return string.Concat(base.DisplayName(res), " - ", Param.SpellSchool.AsDisplayable());
                }
            }
            return base.DisplayName(res);
        }

        public bool IsRanked => RankToSource?.Count > 0;
        public int Rank { get => A.Value<int>() > 0 ? A.Value<int>() : IsRanked ? 1 : 0; set => A.Value(value); }
        public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public bool IgnorePrerequisites { get => A.Value<bool>(); set => A.Value(value); }
        public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));
        public FeatureParamModel Param => A.Object(factory: a => new FeatureParamModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add("m_RankToSource", new JArray());
            FactItemModel.Prepare(refs, obj);
        }

        public override string Export()
        {
            return A.ExportCode(o => {
                o.RemovePath("m_Context", "m_OwnerRef");
                o.Remove("Source");
                o.Remove("SourceLevel");
                o.Remove("UniqueId");
                o.Remove("AttachTime");
                o.Remove("IgnorePrerequisites");
                o.Remove("IsActive");
            });
        }
    }
}
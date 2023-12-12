using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class W40KRTFeatFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Feature, Code";
        public W40KRTFeatFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public override FactContextModel Context { get => A.Object("m_Context", a => new W40KRTFeatFactContextModel(a)); set => base.Context = value; }
        public W40KRTFeatFactContextModel Context2 => (W40KRTFeatFactContextModel)Context;

        public bool IsRanked => Context2?.Ranks?.Count > 0;
        public int Rank { get => A.Value<int>() > 0 ? A.Value<int>() : IsRanked ? 1 : 0; set => A.Value(value); }
        //public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        //public string Source { get => A.Value<string>(); set => A.Value(value); }
        //public bool IgnorePrerequisites { get => A.Value<bool>(); set => A.Value(value); }
        //public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));
        //public FeatureParamModel Param => A.Object(factory: a => new FeatureParamModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }

        public override string Export()
        {
            return A.ExportCode(o => {
                o.RemovePath("m_Context", "m_OwnerRef");
                o.RemovePath("m_Context", "m_CasterRef");
                o.RemovePath("m_Context", "m_ParentContext");
                o.RemovePath("m_Context", "Direction");
                o.Remove("UniqueId");
                o.Remove("IgnorePrerequisites");
                o.Remove("DisabledBecauseOfPrerequisites");
                o.Remove("IsTemporary");
                o.Remove("IsActive");
            });
        }
    }

    public class W40KRTFeatFactContextModel : FactContextModel
    {
        public W40KRTFeatFactContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string CasterRef { get => A.Value<string>("m_CasterRef"); set => A.Value(value, "m_CasterRef"); }
        public ListValueAccessor<int> Ranks => A.ListValue<int>("m_Ranks");
        public ListValueAccessor<int> SharedValues => A.ListValue<int>("m_SharedValues");
        public string SpellDescriptor { get => A.Value<string>(); set => A.Value(value); }
        public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
        public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }
        //"m_MainTarget": null,
        //"m_SourceAbility": null,
        //"m_ShadowFactorPercents": 0,
        //"m_ShadowDisbelieveData": null,
        //"ParentContext": null,
    }
}
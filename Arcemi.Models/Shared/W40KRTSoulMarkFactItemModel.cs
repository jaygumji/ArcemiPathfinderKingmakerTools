using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class W40KRTSoulMarkFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.SoulMark, Code";

        public W40KRTSoulMarkFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Rank { get => A.Value<int>(); set => A.Value(value); }
        public ListAccessor<W40KRTSoulMarkSourceModel> Sources => A.List<W40KRTSoulMarkSourceModel>("m_Sources");

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }

    public class W40KRTSoulMarkSourceModel : RefModel
    {
        public W40KRTSoulMarkSourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName(IGameResourcesProvider res) => res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public int PathRank { get => A.Value<int>("m_PathRank"); set => A.Value(value, "m_PathRank"); }
    }

    public class W40KRTSoulMarkSourceCollectionWriter : GameModelCollectionWriter<IGameDataObject, W40KRTSoulMarkSourceModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("m_Blueprint", args.Blueprint);
            args.Obj.Add("m_PathRank", 1);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameDataObject> current)
        {
            var ids = new HashSet<string>(current.Select(x => ((W40KRTSoulMarkSourceModel)x.Ref).Blueprint), System.StringComparer.Ordinal);
            var projects = Res.Blueprints.GetEntries(Warhammer40KRogueTrader.W40KRTBlueprintTypeProvider.ColonyProject).Where(p => p.Name.Original.ILike("soulmark"));
            return Res.Blueprints.GetEntries(Warhammer40KRogueTrader.W40KRTBlueprintTypeProvider.Answer).Concat(projects)
                .Where(x => !ids.Contains(x.Id))
                .OrderBy(x => x.DisplayName)
                .ToArray();
        }
    }
}
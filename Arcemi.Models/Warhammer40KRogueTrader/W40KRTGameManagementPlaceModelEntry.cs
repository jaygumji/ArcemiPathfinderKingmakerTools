using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTColonyModel : RefModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTColonyModel(ModelDataAccessor accessor) : base(accessor)
        {
            CA = A.Object<RefModel>("Colony").GetAccessor();
            Name = Res.Blueprints.GetNameOrBlueprint(Blueprint);
            ColonyFoundationTime = new TimeParts(() => CA.Value<TimeSpan>("ColonyFoundationTime"), v => CA.Value(v, "ColonyFoundationTime"));
            LastEventTime = new TimeParts(() => CA.Value<TimeSpan>("m_LastEventTime"), v => CA.Value(v, "m_LastEventTime"));
        }

        public RefModel Ref { get; }
        public ModelDataAccessor CA { get; }
        public string Name { get; }
        public TimeParts ColonyFoundationTime { get; }
        public TimeParts LastEventTime { get; }
        public string Planet => A.Value<string>();
        public string Area => A.Value<string>();
        public string Blueprint => CA.Value<string>();
        public IGameDataObject Data { get; }
    }

    internal class W40KRTGameDataListOfProject : IGameDataList
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameDataListOfProject(ListAccessor<RefModel> refModels)
        {
            Entries = new GameModelCollection<IGameDataObject, RefModel>(refModels, a => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(a.GetAccessor().Value<string>("Blueprint")), new IGameData[] {
                GameDataModels.Time("Start time", new TimeParts(() => a.GetAccessor().Value<TimeSpan>("StartTime"), v => a.GetAccessor().Value(v, "StartTime"))),
                GameDataModels.Boolean("Finished", a, x => x.GetAccessor().Value<bool>("IsFinished"), (x, v) => x.GetAccessor().Value(v, "IsFinished")),
            }, @ref: a), writer: new W40KRTGameManagementColonyProjectCollectionWriter());
        }
        public string ItemName => "Project";
        public IGameModelCollection<IGameDataObject> Entries { get; }
        public GameDataListMode Mode => GameDataListMode.Full;
    }

    internal class W40KRTGameManagementColonyProjectCollectionWriter : GameModelCollectionWriter<IGameDataObject, RefModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("Blueprint", args.Blueprint);
            args.Obj.Add("StartTime", "00:00:00");
            args.Obj.Add("IsFinished", true);
            args.Obj.Add("UsedResourcesFromPool", new JArray());
            args.Obj.Add("ProducedResourcesWithoutModifiers", new JArray());
            args.Obj.Add("ResourceShortage", new JArray());
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameDataObject> current)
        {
            var currentIds = new HashSet<string>(current.Select(x => x.Ref.GetAccessor().Value<string>("Blueprint")), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(W40KRTBlueprintTypeProvider.ColonyProject).Where(e => !currentIds.Contains(e.Id)).ToArray();
        }
    }
}
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameManagementModel : IGameManagementModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameManagementModel(PlayerModel player)
        {
            Player = player;
            var csa = player.GetAccessor().Object<RefModel>("ColoniesState").GetAccessor();
            Resources = GameDataModels.Object("Resources", new IGameData[] {
                new W40KRTGameManagementPlaceIntDataEntry(csa.Object<RefModel>("MinerProductivity"), "Miner Productivity"),
                GameDataModels.List("Miner", csa.List<RefModel>("Miners"), rm => {
                    var sso = Res.Blueprints.GetNameOrBlueprint(rm.GetAccessor().Value<string>("Sso"));
                    var resource = Res.Blueprints.GetNameOrBlueprint(rm.GetAccessor().Value<string>("Resource"));
                    return GameDataModels.Object($"{resource} - {sso}", new IGameData[] {
                        GameDataModels.Integer("Count", rm, a => a.GetAccessor().Value<int>("InitialCount"), (a, v) => a.GetAccessor().Value(v, "InitialCount"))
                    });
                }, mode: GameDataListMode.Rows),
                GameDataModels.Object("Other", new [] {
                    new GameDataListOfKeyValueOfInt(Res, csa.List<KeyValuePairModel<int>>("ResourcesNotFromColonies"), "Resource", W40KRTBlueprintTypeProvider.Resource),
                }, isCollapsable: true)
            });

            Places = GameDataModels.Object("Colonies", new IGameData[] {
                GameDataModels.List("Colony", csa.List<W40KRTColonyModel>("Colonies"), m => {
                    return GameDataModels.Object(m.Name, new IGameData[] {
                        GameDataModels.Time("Foundation Time", m.ColonyFoundationTime),
                        GameDataModels.Time("Last Event Time", m.LastEventTime),
                        new W40KRTGameManagementPlaceIntDataEntry(m.CA.Object<RefModel>("Contentment"), "Complacency"),
                        new W40KRTGameManagementPlaceIntDataEntry(m.CA.Object<RefModel>("Efficiency"), "Efficiency"),
                        new W40KRTGameManagementPlaceIntDataEntry(m.CA.Object<RefModel>("Security"), "Security"),
                        GameDataModels.Object("Resources", new IGameData[] {
                            GameDataModels.Object("Initial", new [] {
                                new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("InitialResources"), "Resource", W40KRTBlueprintTypeProvider.Resource),
                            }, isCollapsable: true),
                            GameDataModels.Object("Available", new [] {
                                new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("AvailableProducedResources"), "Resource", W40KRTBlueprintTypeProvider.Resource),
                            }, isCollapsable: true),
                            GameDataModels.Object("Used", new [] {
                                new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("UsedProducedResources"), "Resource", W40KRTBlueprintTypeProvider.Resource)
                            }, isCollapsable: true),
                            //new GameDataListGrouping(new [] {
                            //    new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("AvailableProducedResources"), "Initial", W40KRTBlueprintTypeProvider.Resource),
                            //    new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("AvailableProducedResources"), "Available", W40KRTBlueprintTypeProvider.Resource),
                            //    new GameDataListOfKeyValueOfInt(Res, m.CA.List<KeyValuePairModel<int>>("AvailableProducedResources"), "Used", W40KRTBlueprintTypeProvider.Resource)
                            //})
                        }),
                        GameDataModels.Object("Projects", new [] {
                            new W40KRTGameDataListOfProject(m.CA.List<RefModel>("Projects"))
                        })
                    }, m);
                }, writer: new W40KRTGameManagementPlaceModelEntryWriter())
            });
        }

        public PlayerModel Player { get; }

        public string DisplayName => "Dynasty";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Crew", "Crew");
        public ModelTypeName PlacesTypeName { get; } = new ModelTypeName("Colonies", "Colony");

        public bool IsOverviewEnabled => false;
        public bool IsMembersEnabled => false;
        public bool IsArmiesEnabled => false;
        public bool IsTasksEnabled => false;

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; } = GameModelCollection<IGameManagementMemberModelEntry>.Empty;
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = GameModelCollection<IGameManagementArmyModelEntry>.Empty;
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = GameModelCollection<IGameManagementTaskModelEntry>.Empty;
        public IGameDataObject Resources { get; }
        public IGameDataObject Places { get; }

        public bool IsSupported => true;
    }
}
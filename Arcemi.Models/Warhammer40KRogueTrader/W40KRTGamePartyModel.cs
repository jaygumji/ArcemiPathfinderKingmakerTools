using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyModel : IGamePartyModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGamePartyModel(PlayerModel player, HeaderModel header)
        {
            Player = player;
            var A = player.GetAccessor();
            Data = GameDataModels.Object(new IGameData[] {
                GameDataModels.Object("Party resources", new IGameData[] {
                    A.Object<W40KRTScrapResourceEntry>("Scrap"),
                    A.Object<W40KRTProfitFactorResourceEntry>("ProfitFactor"),
                    new W40KRTNavigatorResourceEntry(player.GetAccessor().Object<RefModel>("WarpTravelState")),
                    new W40KRTRespecsResourceEntry(player),
                }),
                GameDataModels.Object("Reputations", A.List<KeyValuePairModel<int>>("FractionsReputation").Where(x => !x.Key.IEq("None")).Select(x => new W40KRTGamePartyFactionResourceEntry(x)).ToArray()),
                GameDataModels.Object("DLC Rewards", new[] {
                    GameDataModels.List(null, header.GetAccessor().List<RefModel>("m_DlcRewards"), x => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(x.GetAccessor().Value<string>("guid")), new IGameData[] {
                    }), writer: new W40KRTDlcRewardsCollectionWriter(player), mode: GameDataListMode.Rows)
                }, isCollapsable: true),
            });
        }

        public PlayerModel Player { get; }
        public IGameDataObject Data { get; }
        public bool IsSupported => true;
    }
}
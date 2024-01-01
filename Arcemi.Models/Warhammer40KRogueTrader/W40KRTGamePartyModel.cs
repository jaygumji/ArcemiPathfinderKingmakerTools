using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyModel : IGamePartyModel
    {
        public W40KRTGamePartyModel(PlayerModel player)
        {
            Player = player;
            var A = player.GetAccessor();
            Data = GameDataModels.Object(new IGameData[] {
                GameDataModels.Object(new IGameData[] {
                    A.Object<W40KRTScrapResourceEntry>("Scrap"),
                    A.Object<W40KRTProfitFactorResourceEntry>("ProfitFactor"),
                    new W40KRTNavigatorResourceEntry(player.GetAccessor().Object<RefModel>("WarpTravelState")),
                    new W40KRTRespecsResourceEntry(player),
                }),
                GameDataModels.Object("Reputations", A.List<KeyValuePairModel<int>>("FractionsReputation").Where(x => !x.Key.IEq("None")).Select(x => new W40KRTGamePartyFactionResourceEntry(x)).ToArray())
            });
        }

        public PlayerModel Player { get; }
        public IGameDataObject Data { get; }
        public bool IsSupported => true;
    }
}
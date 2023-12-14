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
            Resources = new IGamePartyResourceEntry[] {
                A.Object<W40KRTScrapResourceEntry>("Scrap"),
                A.Object<W40KRTProfitFactorResourceEntry>("ProfitFactor"),
            }.Concat(A.List<KeyValuePairModel<int>>("FractionsReputation").Where(x => !x.Key.IEq("None")).Select(x => new W40KRTGamePartyFactionResourceEntry(x))
            ).ToArray();
        }

        public PlayerModel Player { get; }
        public IReadOnlyList<IGamePartyResourceEntry> Resources { get; }
        public bool IsSupported => true;
    }
}
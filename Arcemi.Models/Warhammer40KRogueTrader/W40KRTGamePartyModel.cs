using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyModel : IGamePartyModel
    {
        public W40KRTGamePartyModel(PlayerModel player)
        {
            Player = player;
            Resources = Array.Empty<IGamePartyResourceEntry>();
        }

        public PlayerModel Player { get; }
        public IReadOnlyList<IGamePartyResourceEntry> Resources { get; }
        public bool IsSupported => true;
    }
}
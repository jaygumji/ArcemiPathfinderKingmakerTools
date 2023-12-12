using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameManagementModel : IGameManagementModel
    {
        public W40KRTGameManagementModel(PlayerModel player)
        {
            Player = player;
        }

        public PlayerModel Player { get; }

        public string DisplayName => throw new System.NotImplementedException();

        public string MemberTypeName => throw new System.NotImplementedException();

        public string PlacesTypeName => throw new System.NotImplementedException();

        public bool IsArmiesEnabled => throw new System.NotImplementedException();

        public bool IsPlacesEnabled => throw new System.NotImplementedException();

        public IGameModelCollection<IGameManagementMemberModelEntry> Members => throw new System.NotImplementedException();

        public IReadOnlyList<IGameManagementArmyModelEntry> Armies => throw new System.NotImplementedException();

        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks => throw new System.NotImplementedException();

        public IGameModelCollection<IGameManagementPlaceModelEntry> Places => throw new System.NotImplementedException();

        public bool IsSupported => false;
    }
}
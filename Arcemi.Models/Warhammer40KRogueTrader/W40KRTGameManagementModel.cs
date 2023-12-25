using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameManagementModel : IGameManagementModel
    {
        public W40KRTGameManagementModel(PlayerModel player)
        {
            Player = player;
            var coloniesState = player.GetAccessor().Object<RefModel>("ColoniesState");
            Places = new GameModelCollection<IGameManagementPlaceModelEntry, RefModel>(coloniesState.GetAccessor().List<RefModel>("Colonies"),
                m => new W40KRTGameManagementPlaceModelEntry(m), writer: new W40KRTGameManagementPlaceModelEntryWriter());
        }

        public PlayerModel Player { get; }

        public string DisplayName => "Dynasty";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Crew", "Crew");
        public ModelTypeName PlacesTypeName { get; } = new ModelTypeName("Colonies", "Colony");

        public bool IsOverviewEnabled => false;
        public bool IsMembersEnabled => false;
        public bool IsArmiesEnabled => false;
        public bool IsTasksEnabled => false;
        public bool IsPlacesEnabled => true;

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; } = GameModelCollection<IGameManagementMemberModelEntry>.Empty;
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = GameModelCollection<IGameManagementArmyModelEntry>.Empty;
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = GameModelCollection<IGameManagementTaskModelEntry>.Empty;
        public IGameModelCollection<IGameManagementPlaceModelEntry> Places { get; }

        public bool IsSupported => true;
    }
}
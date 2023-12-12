using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementModel : IGameManagementModel
    {
        public WotrGameManagementModel(PlayerModel player)
        {
            // Where(l => l.IsFactionCrusaders()) ?? Array.Empty<PlayerLeaderModel>()
            Ref = player;
            Members = new GameModelCollection<IGameManagementMemberModelEntry, PlayerLeaderModel>(player.LeadersManager?.Leaders, a => new WotrGameManagementMemberModelEntry(a), l => l.IsFactionCrusaders());
            Armies = player?.GlobalMaps?.SelectMany(m => m.Armies).Where(a => a.Data.IsFactionCrusaders()).Select(a => new WotrGameManagementArmyModelEntry(a)).ToArray();
            Tasks = player.Kingdom?.Leaders?.Where(l => l.AssignedTask is object).Select(l => new WotrGameManagementTaskModelEntry(l.AssignedTask)).ToArray();
            Places = new GameModelCollection<IGameManagementPlaceModelEntry, SettlementStateModel>(player.Kingdom?.SettlementsManager?.SettlementStates, a => new WotrGameManagementPlaceModelEntry(a));
        }

        public PlayerModel Ref { get; }

        public string DisplayName => "Crusade";
        public string MemberTypeName => "Generals";
        public string PlacesTypeName => "Settlements";

        public bool IsArmiesEnabled => true;
        public bool IsPlacesEnabled => true;

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        public IGameModelCollection<IGameManagementPlaceModelEntry> Places { get; }

        public bool IsSupported => Ref.Kingdom is object;
    }
}
using System;
using System.Collections.Generic;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameManagementModel : IGameManagementModel
    {
        public KingmakerGameManagementModel(PlayerModel player)
        {
            Ref = player;
        }

        public string DisplayName => "Kingdom";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Leaders", "Leader");
        public bool IsOverviewEnabled => false;
        public bool IsMembersEnabled => false;
        public bool IsArmiesEnabled => false;
        public bool IsTasksEnabled => false;

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; } = GameModelCollection<IGameManagementMemberModelEntry>.Empty;
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = Array.Empty<IGameManagementArmyModelEntry>();
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = Array.Empty<IGameManagementTaskModelEntry>();

        public IGameDataObject Resources => null;
        public IGameDataObject Places => null;

        public bool IsSupported => false;

        public PlayerModel Ref { get; }
    }
}
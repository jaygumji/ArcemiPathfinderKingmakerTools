using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameManagementMemberModelEntry : IGameManagementMemberModelEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        private readonly IReadOnlyList<IGameUnitModel> _units;

        public KingmakerGameManagementMemberModelEntry(PlayerKingdomLeaderModel @ref, IReadOnlyList<IGameUnitModel> units)
        {
            Ref = @ref;
            _units = units;
            Overview = GameDataModels.Object(new IGameData[] {
                GameDataModels.Text("Blueprint", @ref, r => r.LeaderSelection?.Blueprint),
                GameDataModels.Text("Type", @ref, r => r.Type)
            });
        }
        public string Name
        {
            get {
                var leaderName = Res.GetLeaderName(Blueprint);
                if (leaderName.HasValue()) return leaderName;
                return string.Concat("<", Ref.Type.AsDisplayable(), ">");
            }
        }
        public string Blueprint => Ref.LeaderSelection?.Blueprint;
        public string PortraitPath
        {
            get {
                if (string.IsNullOrEmpty(Blueprint)) return Res.AppData.Portraits.GetUnknownUri();

                if (Res.TryGetLeader(Blueprint, out var leader)) return Res.GetPortraitsUri(leader.Portrait);
                var unit = _units.FirstOrDefault(u => u.Ref.Descriptor.Blueprint.Eq(Blueprint));
                if (unit is object) return unit.Portrait.Path;
                var id = Res.GetCharacterPotraitIdentifier(Blueprint);
                return Res.GetPortraitsUri(id);
            }
        }
        public IGameDataObject Overview { get; }

        public PlayerKingdomLeaderModel Ref { get; }
    }
}
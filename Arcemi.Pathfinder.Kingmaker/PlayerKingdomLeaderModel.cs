using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerKingdomLeaderModel : RefModel
    {
        private Portraits _portraits;

        public PlayerKingdomLeaderModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public string Role => Type?.AsDisplayable();
        public string Name => Mappings.GetLeaderName(LeaderSelection);
        public bool IsAssigned => !string.IsNullOrEmpty(LeaderSelection);
        public string PortraitPath {
            get {
                if (string.IsNullOrEmpty(LeaderSelection)) {
                    return _portraits.GetUnknownUri();
                }
                return _portraits.GetPortraitsUri(Mappings.GetPortraitId(LeaderSelection));
            }
        }
        public string LeaderSelection { get => A.Value<string>(); set => A.Value(value); }
        public IReadOnlyList<string> PossibleLeaders => A.ListValue<string>();
        public PlayerKingdomTaskModel AssignedTask => A.Object<PlayerKingdomTaskModel>();

        public bool HasAssignedActiveTask => AssignedTask != null && AssignedTask.IsStarted;

        public IReadOnlyList<PlayerKingdomLeaderSpecificBonusModel> SpecificBonuses => A.List<PlayerKingdomLeaderSpecificBonusModel>();

        public void SetSelectedLeaderBonus(int value)
        {
            var bonus = SpecificBonuses
                .FirstOrDefault(b => string.Equals(b.Key, LeaderSelection, StringComparison.OrdinalIgnoreCase));

            if (bonus == null) {
                var list = A.List<PlayerKingdomLeaderSpecificBonusModel>("SpecificBonuses");
                bonus = list.Add((r, o) => {
                    o.Add("Key", LeaderSelection);
                    o.Add("Value", 0);
                });
            }

            bonus.Value = value;
        }

        public void Init(Portraits portraits)
        {
            _portraits = portraits;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class PlayerKingdomLeaderModel : RefModel
    {
        public PlayerKingdomLeaderModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public string Role => Type?.AsDisplayable();
        public bool IsAssigned => !string.IsNullOrEmpty(LeaderSelection?.Blueprint);

        public PlayerKingdomLeaderSelectionModel LeaderSelection { get => A.Object(factory: a => new PlayerKingdomLeaderSelectionModel(a)); }
        public IReadOnlyList<string> PossibleLeaders => A.ListValue<string>();
        public PlayerKingdomTaskModel AssignedTask => A.Object<PlayerKingdomTaskModel>();

        public bool HasAssignedActiveTask => AssignedTask != null && AssignedTask.IsStarted;

        public IReadOnlyList<PlayerKingdomLeaderSpecificBonusModel> SpecificBonuses => A.List<PlayerKingdomLeaderSpecificBonusModel>();

        public void SetSelectedLeaderBonus(int value)
        {
            var bonus = SpecificBonuses
                .FirstOrDefault(b => string.Equals(b.Key, LeaderSelection?.Blueprint, StringComparison.OrdinalIgnoreCase));

            if (bonus == null) {
                var list = A.List<PlayerKingdomLeaderSpecificBonusModel>("SpecificBonuses");
                bonus = list.Add((r, o) => {
                    o.Add("Key", LeaderSelection?.Blueprint);
                    o.Add("Value", 0);
                });
            }

            bonus.Value = value;
        }
    }
}
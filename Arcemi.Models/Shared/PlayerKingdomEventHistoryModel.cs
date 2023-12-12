using System.Collections.Generic;

namespace Arcemi.Models
{
    public class PlayerKingdomEventHistoryModel : RefModel
    {
        public PlayerKingdomEventHistoryModel(ModelDataAccessor accessor) : base(accessor)
        {
            
        }

        public string Event { get => A.Value<string>(); set => A.Value(value); }
        public string Region { get => A.Value<string>(); set => A.Value(value); }
        public int TriggeredOn { get => A.Value<int>(); set => A.Value(value); }
        //public object Solver { get => A.Value<object>(); set => A.Value(value); }
        public string SolverLeader { get => A.Value<string>(); set => A.Value(value); }
        public int SolutionCheck { get => A.Value<int>(); set => A.Value(value); }
        public int SolvedInDays { get => A.Value<int>(); set => A.Value(value); }
        public bool IsShow { get => A.Value<bool>(); set => A.Value(value); }
        public bool IsUserClick { get => A.Value<bool>(); set => A.Value(value); }
        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public bool WasIgnored { get => A.Value<bool>(); set => A.Value(value); }

        public IReadOnlyList<int> SolveResults => A.ListValue<int>();
        public IReadOnlyList<int> SolveResultsFinal => A.ListValue<int>();
        public PlayerKingdomChangeModel TotalChanges => A.Object<PlayerKingdomChangeModel>();
        public PlayerKingdomChangeModel ResolutionChanges => A.Object<PlayerKingdomChangeModel>();

    }
}
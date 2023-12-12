namespace Arcemi.Models
{
    public class PlayerKingdomTaskModel : RefModel
    {
        public PlayerKingdomTaskModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>("$type"); }
        public int StartedOn { get => A.Value<int>(); set => A.Value(value); }
        public string Name { get => A.Value<string>(); set => A.Value(value); }
        public string Description { get => A.Value<string>(); set => A.Value(value); }
        public bool IsStarted { get => A.Value<bool>(); set => A.Value(value); }

        public PlayerKingdomEventModel Event => A.Object<PlayerKingdomEventModel>();
        public PlayerKingdomLeaderModel AssignedLeader => A.Object<PlayerKingdomLeaderModel>();
        public PlayerKingdomRegionModel Region => A.Object<PlayerKingdomRegionModel>();

    }
}
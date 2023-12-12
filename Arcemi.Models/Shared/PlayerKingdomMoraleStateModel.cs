namespace Arcemi.Models
{
    public class PlayerKingdomMoraleStateModel : RefModel
    {
        public PlayerKingdomMoraleStateModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CurrentValue { get => A.Value<int>(); set => A.Value(value); }
        public int MinValue { get => A.Value<int>(); set => A.Value(value); }
        public int MaxValue { get => A.Value<int>(); set => A.Value(value); }
    }
}
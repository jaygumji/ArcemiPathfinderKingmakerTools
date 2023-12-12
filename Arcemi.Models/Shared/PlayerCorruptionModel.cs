namespace Arcemi.Models
{
    public class PlayerCorruptionModel : RefModel
    {
        public PlayerCorruptionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CurrentValue { get => A.Value<int>(); set => A.Value(value); }
    }
}
namespace Arcemi.Models
{
    public class PlayerArmyMoraleModel : RefModel
    {
        public PlayerArmyMoraleModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Value { get => A.Value<int>("m_Value"); set => A.Value(value, "m_Value"); }
        public bool HaveMorale { get => A.Value<bool>(); set => A.Value(value); }
    }
}
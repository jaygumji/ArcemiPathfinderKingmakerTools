namespace Arcemi.Models
{
    public class PlayerKingdomResources : RefModel
    {
        public PlayerKingdomResources(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Finances { get => A.Value<int>("m_Finances"); set => A.Value(value, "m_Finances"); }
        public int Materials { get => A.Value<int>("m_Materials"); set => A.Value(value, "m_Materials"); }
        public int Favors { get => A.Value<int>("m_Favors"); set => A.Value(value, "m_Favors"); }
    }
}
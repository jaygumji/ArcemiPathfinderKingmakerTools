namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerArmySquadAttributes : RefModel
    {
        public PlayerArmySquadAttributes(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int BaseValue { get => A.Value<int>("m_BaseValue"); set => A.Value(value, "m_BaseValue"); }
    }
}
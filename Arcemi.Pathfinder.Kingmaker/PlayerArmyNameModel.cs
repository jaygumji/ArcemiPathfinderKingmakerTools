namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerArmyNameModel : RefModel
    {
        public PlayerArmyNameModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name { get => A.Value<string>("ArmyName"); set => A.Value(value, "ArmyName"); }
        public int ArmyIndex { get => A.Value<int>(); set => A.Value(value); }
    }
}
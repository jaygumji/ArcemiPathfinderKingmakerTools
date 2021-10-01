namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerArmySquadModel : RefModel
    {
        public PlayerArmySquadModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Guid { get => A.Value<string>("Id"); set => A.Value(value, "Id"); }
        public string Unit { get => A.Value<string>(); set => A.Value(value); }
        public string DisplayName => A.Res.GetArmyUnitName(Unit);
        public int Count { get => A.Value<int>(); set => A.Value(value); }
    }
}
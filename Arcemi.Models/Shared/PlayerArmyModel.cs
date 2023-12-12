using System;

namespace Arcemi.Models
{
    public class PlayerArmyModel : RefModel
    {
        public PlayerArmyModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string ArmyType { get => A.Value<string>(); set => A.Value(value); }
        public double MovementPoints { get => A.Value<double>(); set => A.Value(value); }
        public int MovementPointsRounded { get => (int)Math.Round(MovementPoints); set => MovementPoints = value; }
        public bool IsGarrison { get => A.Value<bool>(); set => A.Value(value); }
        public bool HasNoReward { get => A.Value<bool>(); set => A.Value(value); }
        public bool IsInSettlement { get => A.Value<bool>(); set => A.Value(value); }

        public PlayerArmyDataModel Data => A.Object(factory: a => new PlayerArmyDataModel(a));
    }
}
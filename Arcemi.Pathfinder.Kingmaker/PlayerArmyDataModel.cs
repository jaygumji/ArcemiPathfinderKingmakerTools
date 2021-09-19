using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerArmyDataModel : RefModel, IModelWithFaction
    {
        public PlayerArmyDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string LeaderGuid { get => A.Value<string>("m_LeaderGuid"); set => A.Value(value, "m_LeaderGuid"); }
        public IReadOnlyList<PlayerArmySquadModel> Squads { get => A.List("m_Squads", a => new PlayerArmySquadModel(a)); }
        public PlayerArmyNameModel ArmyName => A.Object("m_ArmyName", a => new PlayerArmyNameModel(a));
        public PlayerArmyMoraleModel ArmyMorale => A.Object("m_ArmyMorale", a => new PlayerArmyMoraleModel(a));

        public string Faction { get => A.Value<string>(); set => A.Value(value); }
        public PlayerArmySquadAttributes MaxSquadsCount => A.Object(factory: a => new PlayerArmySquadAttributes(a));
        public string Preset { get => A.Value<string>(); set => A.Value(value); }
    }
}
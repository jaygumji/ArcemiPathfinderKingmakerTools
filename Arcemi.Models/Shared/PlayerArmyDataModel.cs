namespace Arcemi.Models
{
    public class PlayerArmyDataModel : RefModel, IModelWithFaction
    {
        public PlayerArmyDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string LeaderGuid { get => A.Value<string>("m_LeaderGuid"); set => A.Value(value, "m_LeaderGuid"); }
        public ListAccessor<PlayerArmySquadModel> Squads => A.List("m_Squads", a => new PlayerArmySquadModel(a));
        public ListAccessor<PlayerArmySquadModel> SquadsPosition => A.List("m_SquadsPosition", a => new PlayerArmySquadModel(a));
        public PlayerArmyNameModel ArmyName => A.Object("m_ArmyName", a => new PlayerArmyNameModel(a));
        public PlayerArmyMoraleModel ArmyMorale => A.Object("m_ArmyMorale", a => new PlayerArmyMoraleModel(a));

        public string Faction { get => A.Value<string>(); set => A.Value(value); }
        public PlayerArmySquadAttributes MaxSquadsCount => A.Object(factory: a => new PlayerArmySquadAttributes(a));
        public string Preset { get => A.Value<string>(); set => A.Value(value); }
    }
}
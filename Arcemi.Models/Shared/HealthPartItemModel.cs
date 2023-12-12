namespace Arcemi.Models
{
    public class HealthPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartHealth, Code";

        public HealthPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int DamageReceivedThisTurn { get => A.Value<int>("m_DamageReceivedThisTurn"); set => A.Value(value, "m_DamageReceivedThisTurn"); }
        public int LastTurnWhenReceiveDamage { get => A.Value<int>("m_LastTurnWhenReceiveDamage"); set => A.Value(value, "m_LastTurnWhenReceiveDamage"); }
        public int ConsecutiveOutOfCombatTurnsWithoutDamage { get => A.Value<int>("m_ConsecutiveOutOfCombatTurnsWithoutDamage"); set => A.Value(value, "m_ConsecutiveOutOfCombatTurnsWithoutDamage"); }
        public int TemporaryHitPoints { get => A.Value<int>("m_TemporaryHitPoints"); set => A.Value(value, "m_TemporaryHitPoints"); }
        public double MissingHpFraction { get => A.Value<int>("m_MissingHpFraction"); set => A.Value(value, "m_MissingHpFraction"); }
    }
}
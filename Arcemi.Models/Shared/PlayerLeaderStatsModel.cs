using System.Collections;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class PlayerLeaderStatsModel : RefModel, IEnumerable<PlayerLeaderAttributeModel>
    {
        public PlayerLeaderStatsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CurrentMana { get => A.Value<int>("m_CurrentMana"); set => A.Value(value, "m_CurrentMana"); }
        public PlayerLeaderAttributeModel MaxMana => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Max Mana"));
        public PlayerLeaderAttributeModel AttackBonus => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Attack Bonus"));
        public PlayerLeaderAttributeModel DefenseBonus => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Defense Bonus"));
        public PlayerLeaderAttributeModel SpellStrength => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Spell Strength"));
        public PlayerLeaderAttributeModel ManaRegeneration => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Mana Regeneration"));
        public PlayerLeaderAttributeModel InfirmarySize => A.Object(factory: a => new PlayerLeaderAttributeModel(a, "Infirmary Size"));

        public IEnumerator<PlayerLeaderAttributeModel> GetEnumerator()
        {
            yield return AttackBonus;
            yield return DefenseBonus;
            yield return SpellStrength;
            yield return MaxMana;
            yield return ManaRegeneration;
            yield return InfirmarySize;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentLevel
    {
        public EnchantmentLevel(params EnchantmentSpec[] levels)
        {
            Levels = levels;
        }

        public IReadOnlyList<EnchantmentSpec> Levels { get; }

        public int Level(string blueprint)
        {
            for (var i = 0; i < Levels.Count; i++) {
                if (string.Equals(Levels[i].Blueprint, blueprint, StringComparison.Ordinal)) {
                    return i + 1;
                }
            }
            return 0;
        }
        public EnchantmentSpec Level(int level)
        {
            if (level < 1 || level > Levels.Count) {
                throw new ArgumentOutOfRangeException(nameof(level), level, $"Enchantmentlevel must be between 1 and {Levels.Count}");
            }
            return Levels[level - 1];
        }

        public int GetLevelFrom(IEnumerable<FactItemModel> facts)
        {
            foreach (var fact in facts.OfType<EnchantmentFactItemModel>()) {
                var level = Level(fact.Blueprint);
                if (level > 0) return level;
            }
            return 0;
        }

        public void SetLevelOn(ListAccessor<FactItemModel> facts, int value)
        {
            var spec = value > 0 ? Level(value) : null;
            var fact = facts.OfType<EnchantmentFactItemModel>().Where(f => Level(f.Blueprint) > 0).FirstOrDefault();
            if (fact != null) {
                if (spec == null) {
                    facts.Remove(fact);
                    return;
                }
                fact.Blueprint = spec.Blueprint;
                fact.ActivateCustomEnchantments();
                fact.Components.Clear();
                foreach (var component in spec.Components) {
                    fact.Components.AddNull(component);
                }
                return;
            }
            if (spec == null) return;
            fact = (EnchantmentFactItemModel)facts.Add(EnchantmentFactItemModel.Prepare);
            fact.Blueprint = spec.Blueprint;
            fact.ActivateCustomEnchantments();
            foreach (var component in spec.Components) {
                fact.Components.AddNull(component);
            }
        }
    }
}
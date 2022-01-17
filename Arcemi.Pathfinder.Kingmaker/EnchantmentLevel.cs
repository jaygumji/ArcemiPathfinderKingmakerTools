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
            var res = 0;
            foreach (var fact in facts.OfType<EnchantmentFactItemModel>()) {
                var level = Level(fact.Blueprint);
                if (level > res) res = level;
            }
            return res;
        }

        private void FindLevel(ListAccessor<FactItemModel> facts, out EnchantmentFactItemModel levelFact)
        {
            levelFact = null;
            foreach (var fact in facts.OfType<EnchantmentFactItemModel>()) {
                var level = Level(fact.Blueprint);
                if (level > 0) {
                    levelFact = fact;
                    return;
                }
            }
        }

        public void SetLevelOn(ListAccessor<FactItemModel> facts, int value)
        {
            var spec = value > 0 ? Level(value) : null;
            FindLevel(facts, out var fact);
            SetLevelFact(facts, fact, spec);
        }

        private void SetLevelFact(ListAccessor<FactItemModel> facts, EnchantmentFactItemModel fact, EnchantmentSpec spec)
        {
            if (fact != null) {
                if (spec == null) {
                    facts.Remove(fact);
                    return;
                }
                fact.Blueprint = spec.Blueprint;
                fact.ActivateCustomEnchantments();
                fact.Components.Clear();
                foreach (var component in spec.Components) {
                    component.AddTo(fact.Components);
                }
                return;
            }
            if (spec == null) return;
            fact = (EnchantmentFactItemModel)facts.Add(EnchantmentFactItemModel.Prepare);
            fact.Blueprint = spec.Blueprint;
            fact.ActivateCustomEnchantments();
            foreach (var component in spec.Components) {
                component.AddTo(fact.Components);
            }
        }
    }
}
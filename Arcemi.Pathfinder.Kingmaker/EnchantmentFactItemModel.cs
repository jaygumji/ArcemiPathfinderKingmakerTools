using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class Enchantments
    {
        public static EnchantmentSpec Level1 = new EnchantmentSpec("d42fc23b92c640846ac137dc26e000d4", "$WeaponEnhancementBonus$f1459788-04d5-4128-ad25-dace4b8dee42");

        public static IReadOnlyList<EnchantmentSpec> Levels { get; } = new[] {
            Level1
        };
        public static int Level(string blueprint)
        {
            for (var i = 0; i < Levels.Count; i++) {
                if (string.Equals(Levels[i].Blueprint, blueprint, StringComparison.Ordinal)) {
                    return i + 1;
                }
            }
            return 0;
        }
        public static EnchantmentSpec Level(int level)
        {
            if (level < 1 || level > Levels.Count) {
                throw new ArgumentOutOfRangeException(nameof(level), level, $"Enchantmentlevel must be between 1 and {Levels.Count}");
            }
            return Levels[level - 1];
        }
    }

    public class EnchantmentFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.Blueprints.Items.Ecnchantments.ItemEnchantment, Assembly-CSharp";
        public EnchantmentFactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }

        public int Level
        {
            get {
                return Enchantments.Level(Blueprint);
            }
            set {
                var spec = Enchantments.Level(value);
                Blueprint = spec.Blueprint;
                foreach (var level in Enchantments.Levels) {
                    Components.Remove(level.Component);
                }
                Components.Add(spec.Component, null);
            }
        }
    }
}
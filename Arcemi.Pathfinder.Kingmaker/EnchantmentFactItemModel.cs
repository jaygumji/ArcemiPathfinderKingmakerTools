using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.Blueprints.Items.Ecnchantments.ItemEnchantment, Assembly-CSharp";
        public EnchantmentFactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }

        private static class Levels
        {
            public static class One
            {
                public const string Blueprint = "d42fc23b92c640846ac137dc26e000d4";
                public const string Component = "$WeaponEnhancementBonus$f1459788-04d5-4128-ad25-dace4b8dee42";
            }
        }

        public int Level
        {
            get {
                if (string.Equals(Blueprint, Levels.One.Blueprint, StringComparison.Ordinal)) return 1;

                return 0;
            }
            set {
                if (value == 1) {
                    Blueprint = Levels.One.Blueprint;
                    if (Components.Count > 0) Components.Clear();
                    Components.Add(Levels.One.Component, null);
                }
                else {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Enchantmentlevel must be between 1 and 5");
                }
            }
        }
    }
}
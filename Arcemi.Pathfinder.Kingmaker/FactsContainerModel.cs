using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactsContainerModel : RefModel
    {
        public FactsContainerModel(ModelDataAccessor accessor) : base(accessor) { }

        public ListAccessor<FactItemModel> Items => A.List("m_Facts", FactItemModel.Factory, createIfNull: true);
    }

    public class FactItemModel : RefModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string DisplayName => A.Res.GetFactName(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); }

        public static FactItemModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, FeatureFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new FeatureFactItemModel(accessor);
            }
            if (string.Equals(type, EnchantmentFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new EnchantmentFactItemModel(accessor);
            }
            return new FactItemModel(accessor);
        }
    }

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

        public DictionaryOfValueAccessor<string> Components => A.DictionaryOfValue<string>(createIfNull: true);
    }

    public class FeatureFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Feature, Assembly-CSharp";
        public FeatureFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public int Rank { get => A.Value<int>() > 0 ? A.Value<int>() : RankToSource?.Count > 0 ? 1 : 0; set => A.Value(value); }
        public int SourceLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Source { get => A.Value<string>(); set => A.Value(value); }
        public ListAccessor<FeatureRankToSourceModel> RankToSource => A.List("m_RankToSource", a => new FeatureRankToSourceModel(a));
    }
}
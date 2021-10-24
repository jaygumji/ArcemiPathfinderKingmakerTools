using Newtonsoft.Json.Linq;
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
                fact.Components.Clear();
                foreach (var component in spec.Components) {
                    fact.Components.AddNull(component);
                }
                return;
            }
            if (spec == null) return;
            fact = (EnchantmentFactItemModel)facts.Add(EnchantmentFactItemModel.Prepare);
            fact.Blueprint = spec.Blueprint;
            foreach (var component in spec.Components) {
                fact.Components.AddNull(component);
            }
        }
    }

    public static class Enchantments
    {
        private const string WeaponLevelComponent = "$WeaponEnhancementBonus$f1459788-04d5-4128-ad25-dace4b8dee42";
        public static EnchantmentLevel WeaponLevel { get; } = new EnchantmentLevel(
            new EnchantmentSpec("d42fc23b92c640846ac137dc26e000d4", WeaponLevelComponent),
            new EnchantmentSpec("eb2faccc4c9487d43b3575d7e77ff3f5", WeaponLevelComponent),
            new EnchantmentSpec("80bb8a737579e35498177e1e3c75899b", WeaponLevelComponent),
            new EnchantmentSpec("783d7d496da6ac44f9511011fc5f1979", WeaponLevelComponent),
            new EnchantmentSpec("bdba267e951851449af552aa9f9e3992", WeaponLevelComponent),
            new EnchantmentSpec("0326d02d2e24d254a9ef626cc7a3850f", WeaponLevelComponent)
        );

        public static EnchantmentLevel ArmorLevel { get; } = new EnchantmentLevel(
            new EnchantmentSpec("a9ea95c5e02f9b7468447bc1010fe152", "$ArmorEnhancementBonus$5a486a8c-d492-4a6f-a4c5-d0dcff3699e9", "$AdvanceArmorStats$e260771a-2fbe-4887-a404-c350595e312f"),
            new EnchantmentSpec("758b77a97640fd747abf149f5bf538d0", "$ArmorEnhancementBonus$6d181b1b-f120-4420-8f60-dee4abbeb9f2", "$AdvanceArmorStats$ca2bdcea-5159-4376-bd99-71c422364011"),
            new EnchantmentSpec("9448d3026111d6d49b31fc85e7f3745a", "$ArmorEnhancementBonus$3f3183ee-32cf-422a-93ac-7e868aeb81cf", "$AdvanceArmorStats$f787b7e4-5fef-434f-bdb5-64c67478736b"),
            new EnchantmentSpec("eaeb89df5be2b784c96181552414ae5a", "$ArmorEnhancementBonus$4a0d34c2-145f-4439-94cf-db83c3ce40ef", "$AdvanceArmorStats$8655dc3a-fe68-48d3-9b07-19d000a169e1"),
            new EnchantmentSpec("6628f9d77fd07b54c911cd8930c0d531", "$ArmorEnhancementBonus$7f092aa2-3fb6-406c-9f15-6c453cb6e4f4", "$AdvanceArmorStats$8de57ec6-0c32-4f18-b2cc-60c0507b35a6")
        //new EnchantmentSpec("de15272d1f4eb7244aa3af47dbb754ef", "", "")
        );

        public static EnchantmentLevel ShieldLevel { get; } = new EnchantmentLevel(
            new EnchantmentSpec("e90c252e08035294eba39bafce76c119", "$ArmorEnhancementBonus$bb160059-fd6e-47ec-94ef-966087f9cc72", "$AdvanceArmorStats$cfdf702d-673d-4c44-a00f-ec087b438418"),
            new EnchantmentSpec("7b9f2f78a83577d49927c78be0f7fbc1", "$ArmorEnhancementBonus$66ec610e-e1eb-4298-ae47-13cd1238ff65", "$AdvanceArmorStats$9a0cf77a-6124-4adc-a3f4-6da4066baa0e"),
            new EnchantmentSpec("ac2e3a582b5faa74aab66e0a31c935a9", "$ArmorEnhancementBonus$e7e0737e-1113-40e4-ae34-f0aa5521e19d", "$AdvanceArmorStats$3db8a64a-d758-4202-8a99-ce362a312979"),
            new EnchantmentSpec("a5d27d73859bd19469a6dde3b49750ff", "$ArmorEnhancementBonus$f2d70d6c-0969-4751-b3fe-8591d8a1da62", "$AdvanceArmorStats$a903c095-8003-4239-ad87-4a245dec7fae"),
            new EnchantmentSpec("84d191a748edef84ba30c13b8ab83bd9", "$ArmorEnhancementBonus$3b531e97-77d2-4289-952b-19d68e22b272", "$AdvanceArmorStats$e517aa19-8ab6-4751-a4d3-e6ab75af94b8")
        //new EnchantmentSpec("70c26c66adb96d74baec38fc8d20c139", "", "")
        );
    }

    public class EnchantmentFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.Blueprints.Items.Ecnchantments.ItemEnchantment, Assembly-CSharp";
        public EnchantmentFactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }

        public static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add(nameof(AttachTime), "00:00:00");
            obj.Add(nameof(IsActive), false);
            obj.Add(nameof(UniqueId), Guid.NewGuid().ToString());
            FactItemModel.Prepare(refs, obj);
        }
    }
}
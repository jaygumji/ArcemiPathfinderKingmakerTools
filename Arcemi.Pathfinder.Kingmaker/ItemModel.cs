#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class WeaponItemModel : ItemModel
    {
        public WeaponItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        //public object Second => null;
        public bool ForceSecondary { get => A.Value<bool>(); set => A.Value(value); }
        public bool IsSecondPartOfDoubleWeapon { get => A.Value<bool>(); set => A.Value(value); }
        public bool IsShield { get => A.Value<bool>(); set => A.Value(value); }

    }

    public class UsableItemModel : ItemModel
    {
        public UsableItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class ShieldItemModel : ItemModel
    {
        public ShieldItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class ArmorItemModel : ItemModel
    {
        public ArmorItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class SimpleItemModel : ItemModel
    {
        public SimpleItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class ItemModel : RefModel
    {
        private const string TypeSimple = "Kingmaker.Items.ItemEntitySimple, Assembly-CSharp";
        private const string TypeWeapon = "Kingmaker.Items.ItemEntityWeapon, Assembly-CSharp";
        private const string TypeShield = "Kingmaker.Items.ItemEntityShield, Assembly-CSharp";
        private const string TypeArmor = "Kingmaker.Items.ItemEntityArmor, Assembly-CSharp";
        private const string TypeUsable = "Kingmaker.Items.ItemEntityUsable, Assembly-CSharp";

        public ItemModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(nameof(Blueprint), nameof(RawData));
        }

        public RawItemData RawData => Mappings.RawItems.GetByBlueprint(Blueprint);
        public DescriptiveItemData DescriptiveData => Mappings.DescriptiveItems.GetByBlueprint(Blueprint);

        public string DisplayName => (DescriptiveData?.Name).OrIfEmpty(RawData?.Name.AsDisplayable()).OrIfEmpty(Blueprint);
        public string DisplayType => (DescriptiveData?.Type).OrIfEmpty(RawData?.TypeName).OrIfEmpty(null);
        public string DisplayDescription => (DescriptiveData?.Description).OrIfEmpty(null);

        public bool IsStackable => RawData?.IsStackable ?? false;
        public bool IsChargable => RawData?.IsChargable ?? false;

        public string Type { get => A.Value<string>("$type"); }
        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public int Count { get => A.Value<int?>("m_Count") ?? 1; set => A.Value(value, "m_Count"); }
        public int InventorySlotIndex { get => A.Value<int>("m_InventorySlotIndex"); set => A.Value(value, "m_InventorySlotIndex"); }
        public TimeSpan Time { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public int Charges { get => A.Value<int?>() ?? 1; set => A.Value(value); }
        public bool IsIdentified { get => A.Value<bool>(); set => A.Value(value); }
        public TimeSpan? SellTime { get => A.Value<TimeSpan?>(); set => A.Value(value); }
        public bool IsNonRemovable { get => A.Value<bool>(); set => A.Value(value); }
        public InventoryModel Collection => A.Object<InventoryModel>();
        //public object Ability => A.Object();
        //public object ActivatableAbility => A.Object();
        //public object Enchantments => A.Object("m_Enchantments");
        public CharacterModel Wielder => A.Object<CharacterModel>();
        public HoldingSlotModel HoldingSlot => A.Object(factory: a => new HoldingSlotModel(a));

        public static ItemModel Create(ModelDataAccessor accessor)
        {
            var type = accessor.Value<string>("$type", "$type");
            switch (type) {
                case TypeWeapon: return new WeaponItemModel(accessor);
                case TypeArmor: return new ArmorItemModel(accessor);
                case TypeShield: return new ShieldItemModel(accessor);
                case TypeUsable: return new UsableItemModel(accessor);
                case TypeSimple: return new SimpleItemModel(accessor);
            }
            return new ItemModel(accessor);
        }

        private static void AddDefaultItemProperties(JObject jObj)
        {
            jObj.Add("m_Count", 1);
            jObj.Add("Wielder", null);
            jObj.Add("HoldingSlot", null);
            jObj.Add("m_Enchantments", null);
            jObj.Add("m_FactsAppliedToWielder", null);
            jObj.Add("m_IdentifyRolls", new JArray());
            jObj.Add("Time", TimeSpan.Zero);
            jObj.Add("Ability", null);
            jObj.Add("ActivatableAbility", null);
            jObj.Add("IsIdentified", true);
            jObj.Add("SellTime", null);
            jObj.Add("IsNonRemovable", false);
            jObj.Add("UniqueId", System.Guid.NewGuid().ToString());
        }

        public static void PrepareDuplicate(IReferences refs, JObject jObj, ItemModel item, ListAccessor<ItemModel> list)
        {
            jObj.Add("UniqueId", Guid.NewGuid().ToString());
            jObj.Add("m_InventorySlotIndex", list.Count > 0 ? list.Max(i => i.InventorySlotIndex) + 1 : 0);
            jObj.Add("Collection", refs.CreateReference(item.Collection.Id));
            item.A.ShallowMerge(jObj);
            jObj.Remove("m_WielderRef");
        }

        public static void Prepare(IReferences refs, JObject jObj, RawItemData rawData, ItemType itemType, string blueprint, InventoryModel inventory, ListAccessor<ItemModel> list)
        {
            AddDefaultItemProperties(jObj);
            jObj.Add("Charges", rawData.IsChargable ? 1 : 0);
            jObj.Add("m_Blueprint", blueprint);
            jObj.Add("Collection", refs.CreateReference(inventory.Id));
            jObj.Add("m_InventorySlotIndex", list.Count > 0 ? list.Max(i => i.InventorySlotIndex) + 1 : 0);
            jObj.Add("UniqueId", System.Guid.NewGuid().ToString());

            var addArmorComponent = itemType == ItemType.Shield;
            if (addArmorComponent) {
                var component = refs.Create();
                AddDefaultItemProperties(component);
                component.Add("m_ModifierDescriptor", "Shield");
                component.Add("m_Modifiers", null);
                component.Add("m_DexBonusLimeterAC", null);
                component.Add("m_InventorySlotIndex", -1);
                component.Add("Collection", null);
                component.Add("Charges", 0);
                if (rawData != null && rawData.TryGetComponent(ItemType.Armor, out var item)) {
                    component.Add("m_Blueprint", item.Blueprint);
                }
                jObj.Add("ArmorComponent", component);
            }

            var addWeaponComponent = itemType == ItemType.Shield;
            if (addWeaponComponent) {
                var component = refs.Create();
                AddDefaultItemProperties(component);
                component.Add("Second", null);
                component.Add("ForceSecondary", false);
                component.Add("IsSecondPartOfDoubleWeapon", false);
                component.Add("IsShield", true);
                component.Add("Collection", null);
                component.Add("Charges", 0);
                jObj.Add("WeaponComponent", component);
            }
        }
    }
}
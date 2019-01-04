#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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
        public bool IsChargable => RawData?.Type == ItemType.UsableWand;

        public string Type { get => A.Value<string>("$type"); }
        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public int Count { get => A.Value<int>("m_Count"); set => A.Value(value, "m_Count"); }
        public int InventorySlotIndex { get => A.Value<int>("m_InventorySlotIndex"); set => A.Value(value, "m_InventorySlotIndex"); }
        public TimeSpan Time { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public int Charges { get => A.Value<int>(); set => A.Value(value); }
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

        public static void Prepare(IReferences refs, JObject jObj, RawItemData rawData, InventoryModel inventory, ListAccessor<ItemModel> list)
        {
            switch (rawData.Type) {
                case ItemType.Weapon:
                    jObj.Add("$type", TypeWeapon);
                    break;
                case ItemType.Armor:
                    jObj.Add("$type", TypeArmor);
                    break;
                case ItemType.Shield:
                    jObj.Add("$type", TypeShield);
                    break;
                case ItemType.UsableLantern:
                case ItemType.UsableMisc:
                case ItemType.UsableOil:
                case ItemType.UsablePotion:
                case ItemType.UsableQuiverItem:
                case ItemType.UsableRod:
                case ItemType.UsableRodMetamagic:
                case ItemType.UsableScroll:
                case ItemType.UsableWand:
                    jObj.Add("$type", TypeUsable);
                    break;
                default:
                    jObj.Add("$type", TypeSimple);
                    break;
            }
            jObj.Add("m_Count", 1);
            jObj.Add("Charges", rawData.Type == ItemType.UsableWand ? 1 : 0);
            jObj.Add("m_Blueprint", rawData.Blueprint);
            jObj.Add("Wielder", null);
            jObj.Add("HoldingSlot", null);
            jObj.Add("Collection", refs.CreateReference(inventory.Id));
            jObj.Add("m_Enchantments", null);
            jObj.Add("m_FactsAppliedToWielder", null);
            jObj.Add("m_IdentifyRolls", new JArray());
            jObj.Add("Time", TimeSpan.Zero);
            jObj.Add("Ability", null);
            jObj.Add("ActivatableAbility", null);
            jObj.Add("IsIdentified", true);
            jObj.Add("SellTime", null);
            jObj.Add("IsNonRemovable", false);
            jObj.Add("m_InventorySlotIndex", list.Max(i => i.InventorySlotIndex) + 1);

        }
    }
}
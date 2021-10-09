#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class WeaponItemModel : ItemModel
    {
        public WeaponItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
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

        public void SetEnhancementLevel(int enchantmentLevel)
        {
            var enchantment = (EnchantmentFactItemModel)Facts.Items.Add((refs, jobj) => jobj.Add("$type", EnchantmentFactItemModel.TypeRef));
            enchantment.Level = enchantmentLevel;
            enchantment.AttachTime = TimeSpan.Zero;
            enchantment.IsActive = false;
            enchantment.UniqueId = Guid.NewGuid().ToString();
        }

        public ItemModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(nameof(Blueprint), nameof(RawData));
        }

        public RawItemData RawData => Mappings.RawItems.GetByBlueprint(Blueprint);
        public DescriptiveItemData DescriptiveData => Mappings.DescriptiveItems.GetByBlueprint(Blueprint);

        public string DisplayName => (DescriptiveData?.Name)
            .OrIfEmpty(A.Res.GetItemName(Blueprint))
            .OrIfEmpty(RawData?.Name.AsDisplayable())
            .OrIfEmpty(Blueprint);
        public string DisplayType => (DescriptiveData?.SubType)
            .OrIfEmpty(DescriptiveData?.Type)
            .OrIfEmpty(ItemType?.AsDisplayable())
            .OrIfEmpty(RawData?.TypeName)
            .OrIfEmpty(null);

        public string DisplayDescription => (DescriptiveData?.Description).OrIfEmpty(null);

        public bool IsStackable => true;
        public bool IsChargable => ItemType == Kingmaker.ItemType.Usable;

        public ItemType? ItemType
        {
            get {
                switch (Type) {
                    case TypeWeapon: return Kingmaker.ItemType.Weapon;
                    case TypeArmor: return Kingmaker.ItemType.Armor;
                    case TypeShield: return Kingmaker.ItemType.Shield;
                    case TypeUsable: return Kingmaker.ItemType.Usable;
                    case TypeSimple: return Kingmaker.ItemType.Simple;
                }
                return null;
            }
        }
        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a), createIfNull: true);
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a), createIfNull: true);
        public string Type { get => A.Value<string>("$type"); }
        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public int Count { get => A.Value<int?>("m_Count") ?? 1; set => A.Value(value, "m_Count"); }
        public int InventorySlotIndex { get => A.Value<int>("m_InventorySlotIndex"); set => A.Value(value, "m_InventorySlotIndex"); }
        public TimeSpan Time { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public int Charges { get => A.Value<int?>() ?? 1; set => A.Value(value); }
        public bool IsIdentified { get => A.Value<bool>(); set => A.Value(value); }
        //public TimeSpan? SellTime { get => A.Value<TimeSpan?>(); set => A.Value(value); }
        public bool IsNonRemovable { get => A.Value<bool>(); set => A.Value(value); }
        public InventoryModel Collection => A.Object<InventoryModel>();
        //public object Ability => A.Object();
        //public object ActivatableAbility => A.Object();
        //public object Enchantments => A.Object("m_Enchantments");
        public string WielderRef => A.Value<string>("m_WielderRef");
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

        private static void AddRequiredItemProperties(JObject jObj, ItemType itemType)
        {
            switch (itemType) {
                case Kingmaker.ItemType.Weapon:
                    jObj.Add("$type", TypeWeapon);
                    break;
                case Kingmaker.ItemType.Armor:
                    jObj.Add("$type", TypeArmor);
                    break;
                case Kingmaker.ItemType.Shield:
                    jObj.Add("$type", TypeShield);
                    break;
                case Kingmaker.ItemType.Usable:
                    jObj.Add("$type", TypeUsable);
                    jObj.Add("Charges", 1);
                    break;
                default:
                    jObj.Add("$type", TypeSimple);
                    break;
            }
            jObj.Add("Time", TimeSpan.Zero);
            jObj.Add("IsIdentified", true);
            jObj.Add("UniqueId", Guid.NewGuid().ToString());
        }

        public static void Duplicate(IReferences refs, JObject jObj, ItemModel item)
        {
            // $type is a reserved value and used by the serializer, it must be the second after the $id field.
            jObj.Add("$type", item.Type);
            jObj.Add("UniqueId", Guid.NewGuid().ToString());
            jObj.Add("Collection", refs.CreateReference(item.Collection.Id));
            item.A.ShallowMerge(jObj);
            jObj.Remove("m_WielderRef");
        }

        public static void Prepare(InventoryModel inventory, IReferences refs, JObject jObj, ItemType itemType)
        {
            AddRequiredItemProperties(jObj, itemType);
            jObj.Add("Collection", refs.CreateReference(inventory.Id));

            //var addArmorComponent = itemType == ItemType.Shield;
            //if (addArmorComponent) {
            //    var component = refs.Create();
            //    AddDefaultItemProperties(component);
            //    component.Add("m_ModifierDescriptor", "Shield");
            //    component.Add("m_Modifiers", null);
            //    component.Add("m_DexBonusLimeterAC", null);
            //    component.Add("m_InventorySlotIndex", -1);
            //    component.Add("Collection", null);
            //    component.Add("Charges", 0);
            //    if (rawData != null && rawData.TryGetComponent(ItemType.Armor, out var item)) {
            //        component.Add("m_Blueprint", item.Blueprint);
            //    }
            //    jObj.Add("ArmorComponent", component);
            //}

            //var addWeaponComponent = itemType == ItemType.Weapon;
            //if (addWeaponComponent) {
            //    var component = refs.Create();
            //    AddDefaultItemProperties(component);
            //    component.Add("Second", null);
            //    component.Add("ForceSecondary", false);
            //    component.Add("IsSecondPartOfDoubleWeapon", false);
            //    component.Add("IsShield", true);
            //    component.Add("Collection", null);
            //    component.Add("Charges", 0);
            //    jObj.Add("WeaponComponent", component);
            //}
        }
    }
}
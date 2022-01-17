using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintType
    {
        public BlueprintType(string displayName, string fullName)
            : this(displayName, BlueprintTypeCategory.NotSet, fullName)
        {
        }

        public BlueprintType(string displayName, BlueprintTypeCategory category, string fullName)
        {
            FullName = fullName;
            DisplayName = displayName;
            Category = category;
        }

        public string FullName { get; }
        public string DisplayName { get; }
        public BlueprintTypeCategory Category { get; }

        public static IReadOnlyList<BlueprintType> AllVendorItems = new[] {
            BlueprintTypes.ItemArmor,
            BlueprintTypes.ItemEquipmentBelt,
            BlueprintTypes.ItemEquipmentFeet,
            BlueprintTypes.ItemEquipmentGlasses,
            BlueprintTypes.ItemEquipmentGloves,
            BlueprintTypes.ItemEquipmentHead,
            BlueprintTypes.Ingredient,
            BlueprintTypes.ItemEquipmentNeck,
            BlueprintTypes.ItemEquipmentRing,
            BlueprintTypes.ItemEquipmentShirt,
            BlueprintTypes.ItemEquipmentShoulders,
            BlueprintTypes.ItemEquipmentUsable,
            BlueprintTypes.ItemEquipmentWrist,
            BlueprintTypes.ItemShield,
            BlueprintTypes.ItemThiefTool,
            BlueprintTypes.ItemWeapon,
            BlueprintTypes.Item
        };

        public static IReadOnlyList<BlueprintType> AddableItems = new[] {
            //BlueprintTypes.ItemArmor,
            BlueprintTypes.ItemEquipmentBelt,
            BlueprintTypes.ItemEquipmentFeet,
            BlueprintTypes.ItemEquipmentGlasses,
            BlueprintTypes.ItemEquipmentGloves,
            BlueprintTypes.ItemEquipmentHead,
            BlueprintTypes.Ingredient,
            BlueprintTypes.ItemEquipmentNeck,
            BlueprintTypes.ItemEquipmentRing,
            BlueprintTypes.ItemEquipmentShirt,
            BlueprintTypes.ItemEquipmentShoulders,
            BlueprintTypes.ItemEquipmentUsable,
            BlueprintTypes.ItemEquipmentWrist,
            BlueprintTypes.ItemKey,
            BlueprintTypes.ItemNote,
            BlueprintTypes.ItemThiefTool,
            BlueprintTypes.ItemWeapon,
            BlueprintTypes.Item
        };

        private static ISet<BlueprintType> ItemAccessories { get; } = new HashSet<BlueprintType> {
            BlueprintTypes.ItemEquipmentBelt,
            BlueprintTypes.ItemEquipmentFeet,
            BlueprintTypes.ItemEquipmentGlasses,
            BlueprintTypes.ItemEquipmentGloves,
            BlueprintTypes.ItemEquipmentHead,
            BlueprintTypes.ItemEquipmentNeck,
            BlueprintTypes.ItemEquipmentRing,
            BlueprintTypes.ItemEquipmentShirt,
            BlueprintTypes.ItemEquipmentShoulders,
            BlueprintTypes.ItemEquipmentWrist
        };

        public bool IsItemWeapon => Equals(BlueprintTypes.ItemWeapon);
        public bool IsItemArmorOrShield => Equals(BlueprintTypes.ItemArmor) || Equals(BlueprintTypes.ItemShield);
        public bool IsItemAccessory => ItemAccessories.Contains(this);
        public bool IsItemIngredient => Equals(BlueprintTypes.Ingredient);
        public bool IsItemUsable => Equals(BlueprintTypes.ItemEquipmentUsable);
        public bool IsItemNotable => Equals(BlueprintTypes.ItemNote);

        public static ItemType GetItemType(string typeFullName)
        {
            var type = BlueprintTypes.Resolve(typeFullName);
            if (ReferenceEquals(type, BlueprintTypes.ItemWeapon)) {
                return ItemType.Weapon;
            }
            if (ReferenceEquals(type, BlueprintTypes.ItemShield)) {
                return ItemType.Shield;
            }
            if (ReferenceEquals(type, BlueprintTypes.ItemArmor)) {
                return ItemType.Armor;
            }
            if (ReferenceEquals(type, BlueprintTypes.ItemEquipmentUsable)) {
                return ItemType.Usable;
            }
            return ItemType.Simple;
        }

        public static bool IsStackableAtVendor(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName)) return false;
            return IsStackable(BlueprintTypes.Resolve(typeFullName));
        }

        public static bool IsStackable(BlueprintType type)
        {
            return type.Equals(BlueprintTypes.ItemEquipmentUsable)
                || type.Equals(BlueprintTypes.Ingredient)
                || type.Equals(BlueprintTypes.Item)
                || type.Equals(BlueprintTypes.ItemThiefTool);
        }
    }
}

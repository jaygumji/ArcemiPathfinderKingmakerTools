using System.Collections.Generic;

namespace Arcemi.Models
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
            PathfinderWotr.WotrBlueprintTypeProvider.ItemArmor,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentBelt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentFeet,
            PathfinderWotr.WotrBlueprintTypeProvider    .ItemEquipmentGlasses,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentGloves,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentHead,
            PathfinderWotr.WotrBlueprintTypeProvider.Ingredient,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentNeck,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentRing,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShirt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShoulders,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentUsable,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentWrist,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemShield,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemThiefTool,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemWeapon,
            PathfinderWotr.WotrBlueprintTypeProvider.Item
        };

        public static IReadOnlyList<BlueprintType> AddableItems = new[] {
            PathfinderWotr.WotrBlueprintTypeProvider.ItemArmor,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemShield,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentBelt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentFeet,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentGlasses,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentGloves,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentHead,
            PathfinderWotr.WotrBlueprintTypeProvider.Ingredient,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentNeck,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentRing,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShirt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShoulders,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentUsable,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentWrist,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemKey,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemNote,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemThiefTool,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemWeapon,
            PathfinderWotr.WotrBlueprintTypeProvider.Item
        };

        private static ISet<BlueprintType> ItemAccessories { get; } = new HashSet<BlueprintType> {
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentBelt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentFeet,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentGlasses,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentGloves,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentHead,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentNeck,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentRing,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShirt,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentShoulders,
            PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentWrist
        };

        public bool IsItemWeapon => Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemWeapon);
        public bool IsItemArmorOrShield => IsItemArmor || IsItemShield;
        public bool IsItemArmor => Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemArmor);
        public bool IsItemShield => Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemShield);
        public bool IsItemAccessory => ItemAccessories.Contains(this);
        public bool IsItemIngredient => Equals(PathfinderWotr.WotrBlueprintTypeProvider.Ingredient);
        public bool IsItemUsable => Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentUsable);
        public bool IsItemNotable => Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemNote);

        public static ItemType GetItemType(string typeFullName)
        {
            var type = new PathfinderWotr.WotrBlueprintTypeProvider().Get(typeFullName);
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintTypeProvider.ItemWeapon)) {
                return ItemType.Weapon;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintTypeProvider.ItemShield)) {
                return ItemType.Shield;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintTypeProvider.ItemArmor)) {
                return ItemType.Armor;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentUsable)) {
                return ItemType.Usable;
            }
            return ItemType.Simple;
        }

        public static bool IsStackableAtVendor(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName)) return false;
            return IsStackable(new PathfinderWotr.WotrBlueprintTypeProvider().Get(typeFullName));
        }

        public static bool IsStackable(BlueprintType type)
        {
            return type.Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemEquipmentUsable)
                || type.Equals(PathfinderWotr.WotrBlueprintTypeProvider.Ingredient)
                || type.Equals(PathfinderWotr.WotrBlueprintTypeProvider.Item)
                || type.Equals(PathfinderWotr.WotrBlueprintTypeProvider.ItemThiefTool);
        }
    }
}

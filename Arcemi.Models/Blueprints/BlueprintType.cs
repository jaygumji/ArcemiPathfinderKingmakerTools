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
            PathfinderWotr.WotrBlueprintProvider.ItemArmor,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentBelt,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentFeet,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentGlasses,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentGloves,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentHead,
            PathfinderWotr.WotrBlueprintProvider.Ingredient,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentNeck,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentRing,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentShirt,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentShoulders,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentUsable,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentWrist,
            PathfinderWotr.WotrBlueprintProvider.ItemShield,
            PathfinderWotr.WotrBlueprintProvider.ItemThiefTool,
            PathfinderWotr.WotrBlueprintProvider.ItemWeapon,
            PathfinderWotr.WotrBlueprintProvider.Item
        };

        private static ISet<BlueprintType> ItemAccessories { get; } = new HashSet<BlueprintType> {
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentBelt,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentFeet,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentGlasses,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentGloves,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentHead,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentNeck,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentRing,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentShirt,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentShoulders,
            PathfinderWotr.WotrBlueprintProvider.ItemEquipmentWrist
        };

        public bool IsItemWeapon => Equals(PathfinderWotr.WotrBlueprintProvider.ItemWeapon);
        public bool IsItemArmorOrShield => IsItemArmor || IsItemShield;
        public bool IsItemArmor => Equals(PathfinderWotr.WotrBlueprintProvider.ItemArmor);
        public bool IsItemShield => Equals(PathfinderWotr.WotrBlueprintProvider.ItemShield);
        public bool IsItemAccessory => ItemAccessories.Contains(this);
        public bool IsItemIngredient => Equals(PathfinderWotr.WotrBlueprintProvider.Ingredient);
        public bool IsItemUsable => Equals(PathfinderWotr.WotrBlueprintProvider.ItemEquipmentUsable);
        public bool IsItemNotable => Equals(PathfinderWotr.WotrBlueprintProvider.ItemNote);

        public static ItemType GetItemType(string typeFullName)
        {
            var type = new PathfinderWotr.WotrBlueprintProvider().GetType(typeFullName);
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintProvider.ItemWeapon)) {
                return ItemType.Weapon;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintProvider.ItemShield)) {
                return ItemType.Shield;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintProvider.ItemArmor)) {
                return ItemType.Armor;
            }
            if (ReferenceEquals(type, PathfinderWotr.WotrBlueprintProvider.ItemEquipmentUsable)) {
                return ItemType.Usable;
            }
            return ItemType.Simple;
        }

        public static bool IsStackableAtVendor(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName)) return false;
            return IsStackable(new PathfinderWotr.WotrBlueprintProvider().GetType(typeFullName));
        }

        public static bool IsStackable(BlueprintType type)
        {
            return type.Equals(PathfinderWotr.WotrBlueprintProvider.ItemEquipmentUsable)
                || type.Equals(PathfinderWotr.WotrBlueprintProvider.Ingredient)
                || type.Equals(PathfinderWotr.WotrBlueprintProvider.Item)
                || type.Equals(PathfinderWotr.WotrBlueprintProvider.ItemThiefTool);
        }

        public override int GetHashCode()
        {
            return FullName?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            return obj is BlueprintType other && string.Equals(FullName, other.FullName, System.StringComparison.Ordinal);
        }
    }
}

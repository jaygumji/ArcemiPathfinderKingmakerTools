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
            BlueprintTypes.Item
        };

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
    }
}

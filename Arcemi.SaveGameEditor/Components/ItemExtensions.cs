using Arcemi.Models;
using System.Resources;

namespace Arcemi.SaveGameEditor.Components
{
    public static class ItemExtensions
    {
        public static string GetIconUrl(this ItemModel item, IGameResourcesProvider res)
        {
            if (res.Blueprints.TryGet(item.Blueprint, out var blueprint)) {
                if (blueprint.Type.IsItemWeapon) {
                    return "/images/ItemTypes/Weapons.png";
                }
                if (blueprint.Type.IsItemArmorOrShield) {
                    return "/images/ItemTypes/ArmorsAndShields.png";
                }
                if (blueprint.Type.IsItemAccessory) {
                    return "/images/ItemTypes/Accessories.png";
                }
                if (blueprint.Type.IsItemIngredient) {
                    return "/images/ItemTypes/Ingredients.png";
                }
                if (blueprint.Type.IsItemUsable) {
                    return "/images/ItemTypes/Usable.png";
                }
                if (blueprint.Type.IsItemNotable) {
                    return "/images/ItemTypes/Notable.png";
                }
                return "/images/ItemTypes/Other.png";
            }
            switch (item.ItemType) {
                case ItemType.Weapon:
                    return "/images/ItemTypes/Weapons.png";
                case ItemType.Armor:
                case ItemType.Shield:
                    return "/images/ItemTypes/ArmorsAndShields.png";
                case ItemType.Usable:
                    return "/images/ItemTypes/Usable.png";
            }
            return "/images/ItemTypes/Other.png";
        }

        public static string GetIconUrl(this BlueprintType type)
        {
            if (type.IsItemWeapon) {
                return "/images/ItemTypes/Weapons.png";
            }
            if (type.IsItemArmorOrShield) {
                return "/images/ItemTypes/ArmorsAndShields.png";
            }
            if (type.IsItemAccessory) {
                return "/images/ItemTypes/Accessories.png";
            }
            if (type.IsItemIngredient) {
                return "/images/ItemTypes/Ingredients.png";
            }
            if (type.IsItemUsable) {
                return "/images/ItemTypes/Usable.png";
            }
            if (type.IsItemNotable) {
                return "/images/ItemTypes/Notable.png";
            }
            return "/images/ItemTypes/Other.png";
        }

        public static bool CanEdit(this ItemModel item)
        {
            return item.ItemType == ItemType.Usable
                || item.ItemType == ItemType.Weapon
                || item.ItemType == ItemType.Armor
                //|| item.ItemType == ItemType.Shield
                ;
        }

    }
}

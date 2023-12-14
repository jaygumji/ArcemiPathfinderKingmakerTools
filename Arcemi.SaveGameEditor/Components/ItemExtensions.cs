using Arcemi.Models;
using System.Resources;

namespace Arcemi.SaveGameEditor.Components
{
    public static class ItemExtensions
    {
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
    }
}

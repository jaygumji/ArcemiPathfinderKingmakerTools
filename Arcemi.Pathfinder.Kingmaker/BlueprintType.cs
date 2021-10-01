using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintType
    {
        public BlueprintType(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; }
        public string Name { get; }

        public static IReadOnlyList<BlueprintType> AddableItems = new[] {
            //new BlueprintType(BlueprintTypes.ItemArmor, "Armor"),
            new BlueprintType(BlueprintTypes.ItemEquipmentBelt, "Belt"),
            new BlueprintType(BlueprintTypes.ItemEquipmentFeet, "Feet"),
            new BlueprintType(BlueprintTypes.ItemEquipmentGlasses, "Glasses"),
            new BlueprintType(BlueprintTypes.ItemEquipmentGloves, "Gloves"),
            new BlueprintType(BlueprintTypes.ItemEquipmentHead, "Head"),
            new BlueprintType(BlueprintTypes.Ingredient, "Ingredient"),
            new BlueprintType(BlueprintTypes.ItemEquipmentNeck, "Neck"),
            new BlueprintType(BlueprintTypes.ItemEquipmentRing, "Ring"),
            new BlueprintType(BlueprintTypes.ItemEquipmentShirt, "Shirt"),
            new BlueprintType(BlueprintTypes.ItemEquipmentShoulders, "Shoulders"),
            new BlueprintType(BlueprintTypes.ItemEquipmentUsable, "Usable"),
            new BlueprintType(BlueprintTypes.ItemEquipmentWrist, "Wrist"),
            new BlueprintType(BlueprintTypes.ItemKey, "Key"),
            new BlueprintType(BlueprintTypes.ItemNote, "Note"),
            //new BlueprintType(BlueprintTypes.ItemShield, "Shield"),
            new BlueprintType(BlueprintTypes.ItemThiefTool, "Thief Tool"),
            //new BlueprintType(BlueprintTypes.ItemWeapon, "Weapon"),
            //new BlueprintType(BlueprintTypes.HiddenItem, "Hidden"),
            new BlueprintType(BlueprintTypes.Item, "Other")
        };

        public static ItemType GetItemType(string typeFullName)
        {
            if (string.Equals(typeFullName, BlueprintTypes.ItemWeapon, StringComparison.Ordinal)) {
                return ItemType.Weapon;
            }
            if (string.Equals(typeFullName, BlueprintTypes.ItemShield, StringComparison.Ordinal)) {
                return ItemType.Shield;
            }
            if (string.Equals(typeFullName, BlueprintTypes.ItemArmor, StringComparison.Ordinal)) {
                return ItemType.Armor;
            }
            if (string.Equals(typeFullName, BlueprintTypes.ItemEquipmentUsable, StringComparison.Ordinal)) {
                return ItemType.Usable;
            }
            return ItemType.Simple;
        }
    }
}

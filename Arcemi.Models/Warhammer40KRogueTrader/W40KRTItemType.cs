using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTItemType
    {
        public static W40KRTItemType StarshipPlasmaDrives { get; } = new W40KRTItemType("Warhammer.SpaceCombat.StarshipLogic.Equipment.ItemEntityPlasmaDrives, Code",
            W40KRTBlueprintProvider.ItemPlasmaDrives);
        public static W40KRTItemType StarshipVoidShieldGenerator { get; } = new W40KRTItemType("Warhammer.SpaceCombat.StarshipLogic.Equipment.ItemEntityVoidShieldGenerator, Code",
            W40KRTBlueprintProvider.ItemVoidShieldGenerator);
        public static W40KRTItemType StarshipAugerArray { get; } = new W40KRTItemType("Warhammer.SpaceCombat.StarshipLogic.StarshipItemEntity`1[[Warhammer.SpaceCombat.Blueprints.BlueprintStarshipItem, Code]], Code",
            W40KRTBlueprintProvider.ItemAugerArray, W40KRTBlueprintProvider.ItemArmorPlating, W40KRTBlueprintProvider.ItemLifeSustainer);
        public static W40KRTItemType StarshipWeapon { get; } = new W40KRTItemType("Warhammer.SpaceCombat.StarshipLogic.Weapon.ItemEntityStarshipWeapon, Code",
            W40KRTBlueprintProvider.StarshipWeapon);
        public static W40KRTItemType StarshipAmmo { get; } = new W40KRTItemType("Warhammer.SpaceCombat.StarshipLogic.Weapon.ItemEntityStarshipAmmo, Code",
            W40KRTBlueprintProvider.StarshipAmmo);
        public static W40KRTItemType Mechadendrite { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntityMechadendrite, Code",
            W40KRTBlueprintProvider.ItemMechadendrite);
        public static W40KRTItemType Weapon { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntityWeapon, Code",
            W40KRTBlueprintProvider.ItemWeapon);
        public static W40KRTItemType Shield { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntityShield, Code",
            W40KRTBlueprintProvider.ItemShield);
        public static W40KRTItemType Armor { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntityArmor, Code",
            W40KRTBlueprintProvider.ItemArmor);
        public static W40KRTItemType Usable { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntityUsable, Code",
            W40KRTBlueprintProvider.ItemEquipmentUsable);
        public static W40KRTItemType Simple { get; } = new W40KRTItemType("Kingmaker.Items.ItemEntitySimple, Code",
            W40KRTBlueprintProvider.ItemEquipmentFeet, W40KRTBlueprintProvider.ItemEquipmentGloves, W40KRTBlueprintProvider.ItemEquipmentHead,
            W40KRTBlueprintProvider.ItemEquipmentNeck, W40KRTBlueprintProvider.ItemEquipmentRing, W40KRTBlueprintProvider.ItemEquipmentShoulders,
            W40KRTBlueprintProvider.ItemResourceMiner, W40KRTBlueprintProvider.ItemNote, W40KRTBlueprintProvider.ItemKey, W40KRTBlueprintProvider.Item);

        private static IReadOnlyDictionary<string, W40KRTItemType> TypeRefLookup { get; } = typeof(W40KRTItemType).GetProperties()
            .Where(p => (p.GetMethod?.IsStatic ?? false) && p.PropertyType == typeof(W40KRTItemType))
            .Select(p => (W40KRTItemType)p.GetValue(null))
            .ToDictionary(x => x.TypeRef, StringComparer.Ordinal);

        private static IReadOnlyDictionary<BlueprintType, W40KRTItemType> BlueprintTypeLookup = (
            from it in TypeRefLookup.Values
            from t in it.Types
            select new {it, t}).ToDictionary(x => x.t, x => x.it);

        public static W40KRTItemType Get(string typeRef) => TypeRefLookup[typeRef];
        public static W40KRTItemType Get(BlueprintType type) => BlueprintTypeLookup[type];

        public IReadOnlyList<BlueprintType> Types { get; }
        public string TypeRef { get; }
        
        public W40KRTItemType(string typeRef, params BlueprintType[] types)
        {
            Types = types;
            TypeRef = typeRef;
        }
    }
}
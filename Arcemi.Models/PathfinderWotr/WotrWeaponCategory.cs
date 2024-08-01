using Arcemi.Models.Shared;
using System;

namespace Arcemi.Models.PathfinderWotr
{
    public enum WotrWeaponCategory
    {
        UnarmedStrike,
        Dagger,
        LightMace,
        PunchingDagger,
        Sickle,
        Club,
        HeavyMace,
        Shortspear,
        Greatclub,
        Longspear,
        Quarterstaff,
        Spear,
        Trident,
        Dart,
        LightCrossbow,
        HeavyCrossbow,
        Javelin,
        Sling,
        Handaxe,
        Kukri,
        LightHammer,
        LightPick,
        Shortsword,
        Starknife,
        WeaponLightShield,
        SpikedLightShield,
        Battleaxe,
        Flail,
        HeavyPick,
        Longsword,
        Rapier,
        Scimitar,
        Warhammer,
        WeaponHeavyShield,
        SpikedHeavyShield,
        EarthBreaker,
        Falchion,
        Glaive,
        Greataxe,
        Greatsword,
        HeavyFlail,
        Scythe,
        Shortbow,
        Longbow,
        Kama,
        Nunchaku,
        Sai,
        Siangham,
        BastardSword,
        DuelingSword,
        DwarvenWaraxe,
        Estoc,
        Falcata,
        Tongi,
        ElvenCurvedBlade,
        Fauchard,
        HandCrossbow,
        LightRepeatingCrossbow,
        HeavyRepeatingCrossbow,
        Shuriken,
        SlingStaff,
        Touch,
        Ray,
        Bomb,
        Bite,
        Claw,
        Gore,
        OtherNaturalWeapons,
        Bardiche,
        DoubleSword,
        DoubleAxe,
        Urgrosh,
        HookedHammer,
        KineticBlast,
        ThrowingAxe,
        Tail,
        Hoof,
        Talon,
        Tentacle,
        Sting,
        Slam,
        Spike,
        Wing,
        SawtoothSabre
    }
    public static class WotrWeaponCategoryExtensions
    {
        public static bool IsCaster(this WotrWeaponCategory category)
            => category == WotrWeaponCategory.Touch || category == WotrWeaponCategory.Ray;
        public static bool IsMelee(this WotrWeaponCategory category)
            => category.Range() <= 6;
        public static bool IsRanged(this WotrWeaponCategory category)
            => category.Range() > 6;
        public static int Range(this WotrWeaponCategory category)
        {
            switch (category) {
                case WotrWeaponCategory.UnarmedStrike:
                case WotrWeaponCategory.Dagger:
                case WotrWeaponCategory.LightMace:
                case WotrWeaponCategory.PunchingDagger:
                case WotrWeaponCategory.Sickle:
                case WotrWeaponCategory.Club:
                case WotrWeaponCategory.HeavyMace:
                case WotrWeaponCategory.Shortspear:
                case WotrWeaponCategory.Greatclub:
                case WotrWeaponCategory.Quarterstaff:
                case WotrWeaponCategory.Spear:
                case WotrWeaponCategory.Trident:
                case WotrWeaponCategory.Handaxe:
                case WotrWeaponCategory.Kukri:
                case WotrWeaponCategory.LightHammer:
                case WotrWeaponCategory.LightPick:
                case WotrWeaponCategory.Shortsword:
                case WotrWeaponCategory.Starknife:
                case WotrWeaponCategory.WeaponLightShield:
                case WotrWeaponCategory.SpikedLightShield:
                case WotrWeaponCategory.Battleaxe:
                case WotrWeaponCategory.Flail:
                case WotrWeaponCategory.HeavyPick:
                case WotrWeaponCategory.Longsword:
                case WotrWeaponCategory.Rapier:
                case WotrWeaponCategory.Scimitar:
                case WotrWeaponCategory.Warhammer:
                case WotrWeaponCategory.WeaponHeavyShield:
                case WotrWeaponCategory.SpikedHeavyShield:
                case WotrWeaponCategory.EarthBreaker:
                case WotrWeaponCategory.Falchion:
                case WotrWeaponCategory.Greataxe:
                case WotrWeaponCategory.Greatsword:
                case WotrWeaponCategory.HeavyFlail:
                case WotrWeaponCategory.Scythe:
                case WotrWeaponCategory.Kama:
                case WotrWeaponCategory.Nunchaku:
                case WotrWeaponCategory.Sai:
                case WotrWeaponCategory.Siangham:
                case WotrWeaponCategory.BastardSword:
                case WotrWeaponCategory.DuelingSword:
                case WotrWeaponCategory.DwarvenWaraxe:
                case WotrWeaponCategory.Estoc:
                case WotrWeaponCategory.Falcata:
                case WotrWeaponCategory.Tongi:
                case WotrWeaponCategory.ElvenCurvedBlade:
                case WotrWeaponCategory.Touch:
                case WotrWeaponCategory.Bite:
                case WotrWeaponCategory.Claw:
                case WotrWeaponCategory.Gore:
                case WotrWeaponCategory.OtherNaturalWeapons:
                case WotrWeaponCategory.DoubleSword:
                case WotrWeaponCategory.DoubleAxe:
                case WotrWeaponCategory.Urgrosh:
                case WotrWeaponCategory.HookedHammer:
                case WotrWeaponCategory.Tail:
                case WotrWeaponCategory.Hoof:
                case WotrWeaponCategory.Talon:
                case WotrWeaponCategory.Tentacle:
                case WotrWeaponCategory.Sting:
                case WotrWeaponCategory.Slam:
                case WotrWeaponCategory.Spike:
                case WotrWeaponCategory.Wing:
                case WotrWeaponCategory.SawtoothSabre:
                    return 2;

                case WotrWeaponCategory.Glaive:
                case WotrWeaponCategory.Longspear:
                case WotrWeaponCategory.Fauchard:
                case WotrWeaponCategory.Bardiche:
                    return 6;

                case WotrWeaponCategory.Dart:
                    return 20;

                case WotrWeaponCategory.Javelin:
                case WotrWeaponCategory.Bomb:
                case WotrWeaponCategory.Ray:
                case WotrWeaponCategory.KineticBlast:
                case WotrWeaponCategory.ThrowingAxe:
                case WotrWeaponCategory.HandCrossbow:
                case WotrWeaponCategory.Shuriken:
                    return 30;

                case WotrWeaponCategory.Shortbow:
                    return 40;

                case WotrWeaponCategory.LightCrossbow:
                case WotrWeaponCategory.HeavyCrossbow:
                case WotrWeaponCategory.Sling:
                case WotrWeaponCategory.SlingStaff:
                case WotrWeaponCategory.Longbow:
                    return 50;

                case WotrWeaponCategory.LightRepeatingCrossbow:
                    return 80;

                case WotrWeaponCategory.HeavyRepeatingCrossbow:
                    return 120;
            }
            throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
    }
}

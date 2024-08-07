using System;

namespace Arcemi.Models.Kingmaker
{
    public enum KingmakerWeaponCategory
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
        ThrowingAxe
    }
    public static class KingmakerWeaponCategoryExtensions
    {
        public static bool IsCaster(this KingmakerWeaponCategory category)
            => category == KingmakerWeaponCategory.Touch || category == KingmakerWeaponCategory.Ray;
        public static bool IsMelee(this KingmakerWeaponCategory category)
            => category.Range() <= 6;
        public static bool IsRanged(this KingmakerWeaponCategory category)
            => category.Range() > 6;
        public static int Range(this KingmakerWeaponCategory category)
        {
            switch (category) {
                case KingmakerWeaponCategory.UnarmedStrike:
                case KingmakerWeaponCategory.Dagger:
                case KingmakerWeaponCategory.LightMace:
                case KingmakerWeaponCategory.PunchingDagger:
                case KingmakerWeaponCategory.Sickle:
                case KingmakerWeaponCategory.Club:
                case KingmakerWeaponCategory.HeavyMace:
                case KingmakerWeaponCategory.Shortspear:
                case KingmakerWeaponCategory.Greatclub:
                case KingmakerWeaponCategory.Quarterstaff:
                case KingmakerWeaponCategory.Spear:
                case KingmakerWeaponCategory.Trident:
                case KingmakerWeaponCategory.Handaxe:
                case KingmakerWeaponCategory.Kukri:
                case KingmakerWeaponCategory.LightHammer:
                case KingmakerWeaponCategory.LightPick:
                case KingmakerWeaponCategory.Shortsword:
                case KingmakerWeaponCategory.Starknife:
                case KingmakerWeaponCategory.WeaponLightShield:
                case KingmakerWeaponCategory.SpikedLightShield:
                case KingmakerWeaponCategory.Battleaxe:
                case KingmakerWeaponCategory.Flail:
                case KingmakerWeaponCategory.HeavyPick:
                case KingmakerWeaponCategory.Longsword:
                case KingmakerWeaponCategory.Rapier:
                case KingmakerWeaponCategory.Scimitar:
                case KingmakerWeaponCategory.Warhammer:
                case KingmakerWeaponCategory.WeaponHeavyShield:
                case KingmakerWeaponCategory.SpikedHeavyShield:
                case KingmakerWeaponCategory.EarthBreaker:
                case KingmakerWeaponCategory.Falchion:
                case KingmakerWeaponCategory.Greataxe:
                case KingmakerWeaponCategory.Greatsword:
                case KingmakerWeaponCategory.HeavyFlail:
                case KingmakerWeaponCategory.Scythe:
                case KingmakerWeaponCategory.Kama:
                case KingmakerWeaponCategory.Nunchaku:
                case KingmakerWeaponCategory.Sai:
                case KingmakerWeaponCategory.Siangham:
                case KingmakerWeaponCategory.BastardSword:
                case KingmakerWeaponCategory.DuelingSword:
                case KingmakerWeaponCategory.DwarvenWaraxe:
                case KingmakerWeaponCategory.Estoc:
                case KingmakerWeaponCategory.Falcata:
                case KingmakerWeaponCategory.Tongi:
                case KingmakerWeaponCategory.ElvenCurvedBlade:
                case KingmakerWeaponCategory.Touch:
                case KingmakerWeaponCategory.Bite:
                case KingmakerWeaponCategory.Claw:
                case KingmakerWeaponCategory.Gore:
                case KingmakerWeaponCategory.OtherNaturalWeapons:
                case KingmakerWeaponCategory.DoubleSword:
                case KingmakerWeaponCategory.DoubleAxe:
                case KingmakerWeaponCategory.Urgrosh:
                case KingmakerWeaponCategory.HookedHammer:
                    return 2;

                case KingmakerWeaponCategory.Glaive:
                case KingmakerWeaponCategory.Longspear:
                case KingmakerWeaponCategory.Fauchard:
                case KingmakerWeaponCategory.Bardiche:
                    return 6;

                case KingmakerWeaponCategory.Dart:
                    return 20;

                case KingmakerWeaponCategory.Javelin:
                case KingmakerWeaponCategory.Bomb:
                case KingmakerWeaponCategory.Ray:
                case KingmakerWeaponCategory.KineticBlast:
                case KingmakerWeaponCategory.ThrowingAxe:
                case KingmakerWeaponCategory.HandCrossbow:
                case KingmakerWeaponCategory.Shuriken:
                    return 30;

                case KingmakerWeaponCategory.Shortbow:
                    return 40;

                case KingmakerWeaponCategory.LightCrossbow:
                case KingmakerWeaponCategory.HeavyCrossbow:
                case KingmakerWeaponCategory.Sling:
                case KingmakerWeaponCategory.SlingStaff:
                case KingmakerWeaponCategory.Longbow:
                    return 50;

                case KingmakerWeaponCategory.LightRepeatingCrossbow:
                    return 80;

                case KingmakerWeaponCategory.HeavyRepeatingCrossbow:
                    return 120;
            }
            throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
    }
}

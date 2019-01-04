#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public static class ItemTypeExtensions
    {
        public static string AsDisplayable(this ItemType type)
        {
            switch (type) {
                case ItemType.HandSimple:
                    return "Hand Simple";
                case ItemType.UsableMisc:
                    return "Usable Misc";
                case ItemType.UsableLantern:
                    return "Lantern";
                case ItemType.UsableOil:
                    return "Oil";
                case ItemType.UsableRod:
                    return "Rod";
                case ItemType.UsableRodMetamagic:
                    return "Rod - Metamagic";
                case ItemType.UsablePotion:
                    return "Potion";
                case ItemType.UsableQuiverItem:
                    return "Quiver";
                case ItemType.UsableScroll:
                    return "Scroll";
                case ItemType.UsableWand:
                    return "Wand";
                default:
                    return type.ToString();
            }
        }
    }
}

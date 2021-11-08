using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public static class InventoryViewModel
    {
        public static readonly List<KeyValuePair<ItemType, string>> ItemTypeStaticList;

        static InventoryViewModel()
        {
            ItemTypeStaticList = new List<KeyValuePair<ItemType, string>>();
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType))) {
                ItemTypeStaticList.Add(new KeyValuePair<ItemType, string>(itemType, itemType.AsDisplayable()));
            }
        }
    }
}

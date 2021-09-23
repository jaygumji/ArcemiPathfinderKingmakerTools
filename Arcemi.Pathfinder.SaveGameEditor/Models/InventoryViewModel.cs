using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class InventoryViewModel
    {
        public static readonly Dictionary<ItemType, List<ItemViewModel>> AllItems;
        public static readonly List<KeyValuePair<ItemType, string>> ItemTypeStaticList;

        static InventoryViewModel()
        {
            AllItems = new Dictionary<ItemType, List<ItemViewModel>>();
            ItemTypeStaticList = new List<KeyValuePair<ItemType, string>>();
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType))) {
                var models = Mappings.RawItems.GetByType(itemType)
                    .Select(items => new ItemViewModel(items))
                    .OrderBy(i => i.DisplayName)
                    .ToList();
                AllItems.Add(itemType, models);
                ItemTypeStaticList.Add(new KeyValuePair<ItemType, string>(itemType, itemType.AsDisplayable()));
            }
        }

        public InventoryModel Inventory { get; }

        public InventoryViewModel(InventoryModel inventory)
        {
            Inventory = inventory;
        }

    }
}

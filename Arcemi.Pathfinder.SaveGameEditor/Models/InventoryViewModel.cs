using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class InventoryViewModel
    {
        private static readonly Dictionary<ItemType, List<ItemViewModel>> AllItems;
        private static readonly List<KeyValuePair<ItemType, string>> ItemTypeStaticList;

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

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set {
                _searchTerm = value;
                Items = AllItems.Values.SelectMany(l => l).Where(i => i.DisplayName?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0
                            || i.DisplayDescription?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0
                            || i.RawData.Name?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
            }
        }

        public InventoryModel Inventory { get; }
        //public IEnumerable<ItemModel> ItemsView => Items.Where(i => i.InventorySlotIndex >= 0);

        public List<KeyValuePair<ItemType, string>> ItemTypes => ItemTypeStaticList;

        private KeyValuePair<ItemType, string> _selectedItemType;
        public KeyValuePair<ItemType, string> SelectedItemType
        {
            get => _selectedItemType;
            set {
                _selectedItemType = value;
                Items = AllItems[value.Key];
            }
        }

        public string CustomBlueprint { get; set; }
        public List<ItemViewModel> Items { get; set; }

        public InventoryViewModel(InventoryModel inventory)
        {
            Inventory = inventory;
            SelectedItemType = ItemTypes.FirstOrDefault(kv => kv.Key == ItemType.Armor);
        }

    }
}

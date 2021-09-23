#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models
{
    public class InventoryViewModel : ViewModel
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
                NotifyPropertyChanged();
                Items = AllItems.Values.SelectMany(l => l).Where(i => i.DisplayName?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0
                            || i.DisplayDescription?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0
                            || i.RawData.Name?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
            }
        }

        public InventoryModel Inventory { get; }
        public ListCollectionView ItemsView { get; }

        public List<KeyValuePair<ItemType, string>> ItemTypes => ItemTypeStaticList;

        private KeyValuePair<ItemType, string> _selectedItemType;
        public KeyValuePair<ItemType, string> SelectedItemType
        {
            get => _selectedItemType;
            set {
                _selectedItemType = value;
                Items = AllItems[value.Key];
                NotifyPropertyChanged();
            }
        }

        private string _customBlueprint;
        public string CustomBlueprint { get => _customBlueprint; set { _customBlueprint = value; NotifyPropertyChanged(); } }

        private List<ItemViewModel> _items;
        public List<ItemViewModel> Items
        {
            get => _items;
            set {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        public InventoryViewModel(InventoryModel inventory)
        {
            Inventory = inventory;
            SelectedItemType = ItemTypes.FirstOrDefault(kv => kv.Key == ItemType.Armor);
            ItemsView = new ListCollectionView((System.Collections.IList)inventory.Items) {
                Filter = i => {
                    var item = (ItemModel)i;
                    return item.InventorySlotIndex >= 0;
                }
            };

        }

    }

    public class ItemViewModel
    {

        public RawItemData RawData { get; }
        public DescriptiveItemData DescriptiveData { get; }

        public string DisplayName => (DescriptiveData?.Name).OrIfEmpty(RawData.Name.AsDisplayable());
        public string DisplayType => (DescriptiveData?.Type).OrIfEmpty(RawData.TypeName);
        public string DisplayDescription => (DescriptiveData?.Description).OrIfEmpty(null);

        public ItemViewModel(RawItemData rawItem)
        {
            RawData = rawItem;
            DescriptiveData = Mappings.DescriptiveItems.GetByBlueprint(rawItem.Blueprint);
        }

    }
}

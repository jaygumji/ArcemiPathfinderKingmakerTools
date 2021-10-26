#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class InventoryModel : RefModel
    {
        public InventoryModel(ModelDataAccessor accessor) : base(accessor)
        {
            //N.On(nameof(Items), nameof(UnequipedItems));
        }

        public ListAccessor<ItemModel> Items => A.List<ItemModel>("m_Items");

        //public IEnumerable<ItemModel> UnequipedItems => Items.Where(i => i.InventorySlotIndex >= 0);

        //public void AddItem(RawItemData rawData)
        //{
        //    var list = A.List<ItemModel>("m_Items");
        //    var item = list.Add((refs, jObj) => ItemModel.Prepare(refs, jObj, rawData,
        //        rawData.Type, rawData.Blueprint, this, list));
        //}

        public void AddItem(ItemType itemType, string blueprint)
        {
            var list = A.List<ItemModel>("m_Items");
            var item = list.Add((refs, jObj) => ItemModel.Prepare(this, refs, jObj, itemType));
            SetInventorySlotIndexToLast(item);
            item.Blueprint = blueprint;
            item.Parts.Items.Touch();
            item.Facts.Items.Touch();
            //switch (itemType) {
            //    case ItemType.Weapon:
            //    case ItemType.Armor:
            //    case ItemType.Shield:
            //        if (enchantmentLevel > 0) {
            //            item.SetEnhancementLevel(enchantmentLevel);
            //        }
            //        break;
            //}
        }

        public ItemModel Duplicate(ItemModel item)
        {
            var list = A.List<ItemModel>("m_Items");
            var newItem = list.Add((refs, jObj) => ItemModel.Duplicate(refs, jObj, item));
            SetInventorySlotIndexToLast(newItem);
            newItem.Parts.Items.Touch();
            newItem.Facts.Items.Touch();
            return newItem;
        }

        private void SetInventorySlotIndexToLast(ItemModel item)
        {
            var list = A.List<ItemModel>("m_Items");
            int FindFirstAvailable()
            {
                var last = 0;
                foreach (var x in list.Where(x => x.InventorySlotIndex > 0).OrderBy(x => x.InventorySlotIndex)) {
                    var next = last + 1;
                    if (x.InventorySlotIndex > next) return next;
                }
                return list.Count > 0 ? list.Max(i => i.InventorySlotIndex) + 1 : 0;
            }
            item.InventorySlotIndex = FindFirstAvailable();
        }
    }
}
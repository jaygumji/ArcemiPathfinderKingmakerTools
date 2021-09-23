#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class InventoryModel : RefModel
    {
        public InventoryModel(ModelDataAccessor accessor) : base(accessor)
        {
            //N.On(nameof(Items), nameof(UnequipedItems));
        }

        public IReadOnlyList<ItemModel> Items => A.List<ItemModel>("m_Items");

        //public IEnumerable<ItemModel> UnequipedItems => Items.Where(i => i.InventorySlotIndex >= 0);

        public void AddItem(RawItemData rawData)
        {
            var list = A.List<ItemModel>("m_Items");
            var item = list.Add((refs, jObj) => ItemModel.Prepare(refs, jObj, rawData,
                rawData.Type, rawData.Blueprint, this, list));
        }

        public void AddItem(ItemType itemType, string blueprint)
        {
            var list = A.List<ItemModel>("m_Items");
            var item = list.Add((refs, jObj) => ItemModel.Prepare(refs, jObj, null,
                itemType, blueprint, this, list));
        }

        public ItemModel Duplicate(ItemModel item)
        {
            var list = A.List<ItemModel>("m_Items");
            return list.Add((refs, jObj) => ItemModel.PrepareDuplicate(refs, jObj, item, list));
        }

    }
}
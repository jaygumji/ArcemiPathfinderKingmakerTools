#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.Linq;

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
            var item = list.Add((refs, jObj) => ItemModel.Prepare(refs, jObj, rawData, this, list));
        }
    }
}
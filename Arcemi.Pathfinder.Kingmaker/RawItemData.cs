#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class RawItemData
    {
        public ItemType Type { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Blueprint { get; set; }
        public bool IsStackable { get; set; }
        public int Cost { get; set; }
        public double Weight { get; set; }
        public bool IsNotable { get; set; }
        public int? DC { get; set; }
        public string TypeName => Type.AsDisplayable();

        private readonly Dictionary<ItemType, RawItemData> _components = new Dictionary<ItemType, RawItemData>();

        public void AddComponent(RawItemData it)
        {
            _components.Add(it.Type, it);
        }

        public bool TryGetComponent(ItemType type, out RawItemData item)
        {
            return _components.TryGetValue(type, out item);
        }
    }
}

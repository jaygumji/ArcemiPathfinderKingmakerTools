#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.IO;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class DescriptiveItems
    {
        private readonly Dictionary<string, DescriptiveItemData> _lookup;

        public DescriptiveItems()
        {
            _lookup = new Dictionary<string, DescriptiveItemData>(StringComparer.Ordinal);
        }

        public DescriptiveItemData GetByBlueprint(string blueprint)
        {
            return _lookup.TryGetValue(blueprint, out var item)
                ? item : null;
        }

        private void Load(string path)
        {
            var data = JsonUtilities.Deserialize<Dictionary<ItemType, List<DescriptiveItemData>>>(path);
            foreach (var kv in data) {
                foreach (var item in kv.Value) {
                    _lookup.Add(item.Blueprint, item);
                }
            }
        }

        public static DescriptiveItems LoadFrom(string directory)
        {
            var items = new DescriptiveItems();
            foreach (var file in Directory.EnumerateFiles(directory, "Item_*.json")) {
                items.Load(file);
            }
            return items;
        }

        public static DescriptiveItems LoadFromDefault()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs");
            return LoadFrom(path);
        }
    }
}

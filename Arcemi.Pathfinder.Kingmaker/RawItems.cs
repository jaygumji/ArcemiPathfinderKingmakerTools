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
    public class RawItems
    {
        private readonly Dictionary<ItemType, List<RawItemData>> _items;
        private readonly Dictionary<string, RawItemData> _lookup;

        public RawItems()
        {
            _items = new Dictionary<ItemType, List<RawItemData>>();
            _lookup = new Dictionary<string, RawItemData>(StringComparer.Ordinal);
        }

        public IReadOnlyList<RawItemData> GetByType(ItemType type)
        {
            return _items.TryGetValue(type, out var list)
                ? (IReadOnlyList<RawItemData>)list
                : new RawItemData[0];
        }

        public RawItemData GetByBlueprint(string blueprint)
        {
            return _lookup.TryGetValue(blueprint, out var item)
                ? item : null;
        }

        private void Load(string path)
        {
            var data = JsonUtilities.Deserialize<Dictionary<ItemType, List<RawItemData>>>(path);
            foreach (var kv in data) {
                _items.Add(kv.Key, kv.Value);
                foreach (var item in kv.Value) {
                    item.Type = kv.Key;
                    _lookup.Add(item.Blueprint, item);
                }
            }
        }

        public static RawItems LoadFrom(string directory)
        {
            var items = new RawItems();
            foreach (var file in Directory.EnumerateFiles(directory, "Raw_*.json")) {
                items.Load(file);
            }
            return items;
        }

        public static RawItems LoadFromDefault()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs");
            return LoadFrom(path);
        }
    }
}

#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class RawItems
    {
        private readonly Dictionary<ItemType, List<RawItemData>> _items;
        private readonly Dictionary<string, RawItemData> _lookup;

        public RawItems(IEnumerable<RawItemData> items)
        {
            _items = new Dictionary<ItemType, List<RawItemData>>();
            _lookup = new Dictionary<string, RawItemData>(StringComparer.Ordinal);

            ItemType ct = default;
            List<RawItemData> list = null;
            foreach (var item in items) {
                if (list == null || ct != item.Type) {
                    ct = item.Type;
                    if (!_items.TryGetValue(item.Type, out list)) {
                        list = new List<RawItemData>();
                        _items.Add(ct, list);
                    }
                }
                list.Add(item);
                _lookup.Add(item.Blueprint, item);
            }
            SetupComponents(ItemType.Shield, ItemType.Armor);
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

        private void SetupComponents(ItemType baseType, params ItemType[] componentTypes)
        {
            if (!_items.TryGetValue(baseType, out var baseItems)) {
                return;
            }

            foreach (var componentType in componentTypes) {
                if (!_items.TryGetValue(componentType, out var componentItems)) {
                    continue;
                }

                var typeComponent = componentType.ToString();

                foreach (var baseItem in baseItems) {
                    var nameComponents = baseItem.Name.ToComponents();
                    var nameAndTypeComponents = nameComponents.Concat(new[] { typeComponent }).OrderBy(n => n).ToArray();

                    for (var i = 0; i < componentItems.Count; i++) {
                        var it = componentItems[i];
                        if (!nameComponents.All(n => it.Name.IndexOf(n, StringComparison.Ordinal) >= 0)) {
                            continue;
                        }
                        var itComponents = it.Name.ToComponents();
                        if (itComponents.OrderBy(n => n).SequenceEqual(nameAndTypeComponents, StringComparer.Ordinal)) {
                            baseItem.AddComponent(it);
                            componentItems.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        public static RawItems LoadFrom(string path)
        {
            var data = JsonUtilities.Deserialize<List<RawItemData>>(path);
            return new RawItems(data);
        }

        public static RawItems LoadFromDefault()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "RawItems.json");
            return LoadFrom(path);
        }
    }
}

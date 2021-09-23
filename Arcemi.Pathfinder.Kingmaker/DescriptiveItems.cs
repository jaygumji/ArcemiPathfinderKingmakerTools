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
    public class DescriptiveItems
    {
        private readonly Dictionary<string, DescriptiveItemData> _lookup;

        public DescriptiveItems(IEnumerable<DescriptiveItemData> items)
        {
            _lookup = items.ToDictionary(i => i.Blueprint, StringComparer.Ordinal);
        }

        public DescriptiveItemData GetByBlueprint(string blueprint)
        {
            return _lookup.TryGetValue(blueprint, out var item)
                ? item : null;
        }

        public static DescriptiveItems LoadFrom(string path)
        {
            var data = JsonUtilities.Deserialize<List<DescriptiveItemData>>(path);
            return new DescriptiveItems(data);
        }

        public static DescriptiveItems LoadFromDefault()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "DescItems.json");
            return LoadFrom(path);
        }
    }
}

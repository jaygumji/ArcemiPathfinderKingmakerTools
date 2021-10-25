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
    public class PortraitCategory
    {
        public static PortraitCategory Others { get; } = new PortraitCategory("others", "Others", isAvailable: true, order: 100000);
        public static PortraitCategory Custom { get; } = new PortraitCategory("custom", "Custom", isAvailable: true, order: 100);
        public static PortraitCategory Unmapped { get; } = new PortraitCategory("unmapped", "Unmapped", isAvailable: true, order: 100000000);
        public static IReadOnlyDictionary<string, PortraitCategory> All { get; }
            = new PortraitCategory[] {
                new PortraitCategory("main", "Main", isAvailable: true, order: 1),
                new PortraitCategory("supernatural", "Supernaturals", isAvailable: true, order: 10),
                Custom,
                new PortraitCategory("animals", "Animals", isAvailable: true, order: 200),
                new PortraitCategory("creatures", "Creatures", isAvailable: true, order: 201),
                new PortraitCategory("special", "Special", isAvailable: false, order: 10000),
                Others,
                Unmapped
            }.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);

        public static PortraitCategory GetCategoryFor(string key)
        {
            return All.TryGetValue(key, out var category) ? category : Others;
        }

        public PortraitCategory(string key, string name, bool isAvailable, int order)
        {
            Key = key;
            Name = name;
            IsAvailable = isAvailable;
            Order = order;
        }

        public string Key { get; }
        public string Name { get; }
        public bool IsAvailable { get; }
        public object Order { get; }
    }
}
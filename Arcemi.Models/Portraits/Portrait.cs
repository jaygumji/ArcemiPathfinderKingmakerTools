#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
namespace Arcemi.Models
{
    public class Portrait
    {
        public string Key { get; }
        public string Uri { get; }
        public PortraitCategory Category { get; }
        public bool IsCustom { get; }
        public bool IsCompanion { get; }
        public string Name { get; }

        public Portrait(string key, string uri, PortraitCategory category, bool isCustom = false, bool isCompanion = false, string name = null)
        {
            Key = key;
            Uri = uri;
            Category = category;
            IsCustom = isCustom;
            IsCompanion = isCompanion;
            Name = name;
        }

    }
}
#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public class Portrait
    {

        public string Key { get; }
        public string Uri { get; }
        public bool IsCustom { get; }
        public bool IsCompanion { get; }

        public Portrait(string key, string uri, bool isCustom = false, bool isCompanion = false)
        {
            Key = key;
            Uri = uri;
            IsCustom = isCustom;
            IsCompanion = isCompanion;
        }

    }
}
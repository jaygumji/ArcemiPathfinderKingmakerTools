#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
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
    }
}

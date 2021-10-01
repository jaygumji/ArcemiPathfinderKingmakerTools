#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ClassDataMapping
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ClassDataMapping> Archetypes { get; set; }
        public string Flags { get; set; }
    }
}
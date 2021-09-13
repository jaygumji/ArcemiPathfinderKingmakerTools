#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ResourceData
    {
        public ResourceData(IReadOnlyDictionary<string, Portrait> allPortraits, IReadOnlyList<Portrait> availablePortraits)
        {
            AllPortraits = allPortraits;
            AvailablePortraits = availablePortraits;
        }

        public IReadOnlyDictionary<string, Portrait> AllPortraits { get; }
        public IReadOnlyList<Portrait> AvailablePortraits { get; }
    }
}
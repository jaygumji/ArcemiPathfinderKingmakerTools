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
    public class DataMappings
    {

        public List<ClassDataMapping> Classes { get; set; }
        public List<RaceDataMapping> Races { get; set; }
        public List<CharacterDataMapping> Characters { get; set; }
        public List<LeaderDataMapping> Leaders { get; set; }
        public List<ArmyUnitMapping> ArmyUnits { get; set; }

        public static DataMappings LoadFrom(string path)
        {
            return JsonUtilities.Deserialize<DataMappings>(path);
        }

        public static DataMappings LoadFromDefault()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "Mappings.json");
            return LoadFrom(path);
        }

    }
}

#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.IO;
using System.Reflection;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PathfinderAppData
    {
        public string Directory { get; }
        public string PortraitsDirectory { get; }
        public string SavedGamesDirectory { get; }
        public Portraits Portraits { get; }

        public PathfinderAppData(Type type, string directory)
            : this(type.Assembly, directory)
        {
        }

        public PathfinderAppData(Assembly assembly, string directory)
        {
            Directory = directory;
            if (!string.IsNullOrEmpty(directory)) {
                PortraitsDirectory = Path.Combine(directory, "Portraits");
                SavedGamesDirectory = Path.Combine(directory, "Saved Games");
            }

            Portraits = new Portraits(assembly, PortraitsDirectory);
        }

    }
}
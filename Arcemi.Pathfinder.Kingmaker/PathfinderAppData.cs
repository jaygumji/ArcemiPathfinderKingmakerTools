#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PathfinderAppData
    {
        private readonly IResourceProvider resourceProvider;

        public string Directory => resourceProvider.Directory;
        public string PortraitsDirectory => resourceProvider.PortraitsDirectory;
        public string SavedGamesDirectory => resourceProvider.SavedGamesDirectory;
        public Portraits Portraits { get; }

        public PathfinderAppData(IResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
            Portraits = new Portraits(resourceProvider);
        }
    }
}
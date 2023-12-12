#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

namespace Arcemi.Models
{
    public class GameAppData
    {
        private readonly IResourceProvider resourceProvider;

        public string Directory => resourceProvider.Directory;
        public string PortraitsDirectory => resourceProvider.PortraitsDirectory;
        public string SavedGamesDirectory => resourceProvider.SavedGamesDirectory;
        public Portraits Portraits { get; }

        public GameAppData(IResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
            Portraits = new Portraits(resourceProvider);
        }
    }
}
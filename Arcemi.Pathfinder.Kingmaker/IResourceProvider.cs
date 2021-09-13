#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IResourceProvider
    {
        string Directory { get; }
        string PortraitsDirectory { get; }
        string SavedGamesDirectory { get; }
        ResourceData GetResources();
    }

    public abstract class ResourceProvider : IResourceProvider
    {
        public string Directory { get; }
        public string PortraitsDirectory { get; }
        public string SavedGamesDirectory { get; }

        public ResourceProvider(string directory)
        {
            Directory = directory;
            if (!string.IsNullOrEmpty(directory)) {
                PortraitsDirectory = Path.Combine(directory, "Portraits");
                SavedGamesDirectory = Path.Combine(directory, "Saved Games");
            }
        }

        protected void AppendCustomPortraits(Dictionary<string, Portrait> all, List<Portrait> available)
        {
            if (!System.IO.Directory.Exists(PortraitsDirectory)) {
                return;
            }

            foreach (var folder in System.IO.Directory.EnumerateDirectories(PortraitsDirectory)) {
                var uri = System.IO.Directory.EnumerateFiles(folder, "Small.*").FirstOrDefault();
                if (string.IsNullOrEmpty(uri)) {
                    continue;
                }
                var key = Path.GetFileName(folder);
                var portrait = new Portrait(key, uri, isCustom: true);
                available.Add(portrait);
                if (!all.ContainsKey(key)) {
                    all.Add(key, portrait);
                }
            }
        }

        public abstract ResourceData GetResources();
    }
}
#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.IO;

namespace Arcemi.Models
{
    public class WwwRootResourceProvider : ResourceProvider
    {
        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            ".jpg",
            ".png"
        };
        private readonly GameDefinition game;
        private readonly string wwwDirectory;

        public WwwRootResourceProvider(GameDefinition game, string wwwDirectory, Func<string> appDataDirectory)
            : base(appDataDirectory)
        {
            this.game = game;
            this.wwwDirectory = wwwDirectory;
        }

        public override ResourceData GetResources()
        {
            var available = new List<Portrait>();
            var all = new Dictionary<string, Portrait>(StringComparer.OrdinalIgnoreCase);
            var portraitsDirectory = Path.Combine(wwwDirectory, "images", "Portraits", game.Id);
            var unknownUri = Path.Combine(wwwDirectory, "images", "Portraits", "_s_unknown.jpg");
            var unknown = new Portrait("_s_unknown", unknownUri, PortraitCategory.GetCategoryFor("special"));
            all.Add(unknown.Key, unknown);

            if (System.IO.Directory.Exists(portraitsDirectory))
            {
                foreach (var uri in System.IO.Directory.EnumerateFiles(portraitsDirectory, "*", SearchOption.AllDirectories))
                {
                    var extension = Path.GetExtension(uri);
                    if (!ImageExtensions.Contains(extension)) continue;

                    var key = Path.GetFileNameWithoutExtension(uri);
                    var dir = Path.GetDirectoryName(uri);
                    var type = Path.GetFileName(dir);
                    var category = PortraitCategory.GetCategoryFor(type);
                    var portrait = new Portrait(key, uri, category);

                    if (category.IsAvailable)
                    {
                        available.Add(portrait);
                    }

                    if (!all.ContainsKey(key))
                    {
                        all.Add(key, portrait);
                    }
                }

                AppendCustomPortraits(all, available);
            }

            return new ResourceData(all, available);
        }
    }
}
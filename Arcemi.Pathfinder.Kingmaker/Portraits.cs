#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class Portraits
    {
        private readonly List<Portrait> _available;
        private readonly Dictionary<string, Portrait> _all;

        public Portraits(Assembly assembly, string directory)
        {
            _available = new List<Portrait>();
            _all = new Dictionary<string, Portrait>(StringComparer.OrdinalIgnoreCase);

            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName)) {
                using (var reader = new System.Resources.ResourceReader(stream)) {
                    foreach (var uri in reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key)) {
                        var key = Path.GetFileNameWithoutExtension(uri);
                        var dir = Path.GetDirectoryName(uri);
                        var type = Path.GetFileName(dir);
                        var portrait = new Portrait(key, "/" + uri);

                        if (string.Equals(type, "available", StringComparison.OrdinalIgnoreCase)) {
                            _available.Add(portrait);
                        }
                        else if (string.Equals(type, "Overridable", StringComparison.OrdinalIgnoreCase)) {
                            _available.Add(portrait);
                        }

                        if (!_all.ContainsKey(key)) {
                            _all.Add(key, portrait);
                        }
                    }
                }
            }

            if (Directory.Exists(directory)) {
                foreach (var folder in Directory.EnumerateDirectories(directory)) {
                    var uri = Directory.EnumerateFiles(folder, "Small.*").FirstOrDefault();
                    if (string.IsNullOrEmpty(uri)) {
                        continue;
                    }
                    var key = Path.GetFileName(folder);
                    var portrait = new Portrait(key, uri, isCustom: true);
                    _available.Add(portrait);
                    if (!_all.ContainsKey(key)) {
                        _all.Add(key, portrait);
                    }
                }
            }
        }

        public IReadOnlyList<Portrait> GetAvailableFor(string characterBlueprint)
        {
            var cn = Mappings.GetCharacterName(characterBlueprint);
            var key = "_c_" + cn;
            if (_all.TryGetValue(key, out var portrait)) {
                return new[] { portrait }.Concat(_available).ToArray();
            }
            else {
                return _available;
            }
        }

        public IReadOnlyList<Portrait> Available => _available;

        public string GetPortraitsUri(string key)
        {
            return _all.TryGetValue(key, out var portrait) ? portrait.Uri : _all["_s_unknown"].Uri;
        }

    }
}
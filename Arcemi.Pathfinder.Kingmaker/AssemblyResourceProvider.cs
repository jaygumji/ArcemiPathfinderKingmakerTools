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
    public class AssemblyResourceProvider : ResourceProvider
    {
        private readonly Assembly assembly;

        public AssemblyResourceProvider(Type type, string directory)
            : this(type.Assembly, directory)
        {
        }

        public AssemblyResourceProvider(Assembly assembly, string directory)
            : base(() => directory)
        {
            this.assembly = assembly;
        }

        public override ResourceData GetResources()
        {
            var available = new List<Portrait>();
            var all = new Dictionary<string, Portrait>(StringComparer.OrdinalIgnoreCase);

            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName)) {
                using (var reader = new System.Resources.ResourceReader(stream)) {
                    foreach (var uri in reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key)) {
                        var key = Path.GetFileNameWithoutExtension(uri);
                        var dir = Path.GetDirectoryName(uri);
                        var type = Path.GetFileName(dir);
                        var portrait = new Portrait(key, "/" + uri);

                        if (string.Equals(type, "available", StringComparison.OrdinalIgnoreCase)) {
                            available.Add(portrait);
                        }
                        else if (string.Equals(type, "Overridable", StringComparison.OrdinalIgnoreCase)) {
                            available.Add(portrait);
                        }

                        if (!all.ContainsKey(key)) {
                            all.Add(key, portrait);
                        }
                    }
                }
            }

            AppendCustomPortraits(all, available);

            return new ResourceData(all, available);
        }
    }
}

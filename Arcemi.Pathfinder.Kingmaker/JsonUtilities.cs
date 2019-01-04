#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class JsonUtilities
    {
        public static void Serialize(string path, object graph)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (graph == null) {
                File.WriteAllText(path, "null");
            }
            var serializer = new JsonSerializer {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8)) {
                    using (var writer = new JsonTextWriter(streamWriter)) {
                        serializer.Serialize(writer, graph);
                    }
                }
            }
        }

        public static T Deserialize<T>(string path)
        {
            var serializer = new JsonSerializer {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var streamReader = new StreamReader(stream, Encoding.UTF8)) {
                    using (var reader = new JsonTextReader(streamReader)) {
                        return serializer.Deserialize<T>(reader);
                    }
                }
            }
        }
    }
}

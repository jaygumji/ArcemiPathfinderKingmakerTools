using System;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Resources;
using System.Linq;

namespace Arcemi.Models
{
    public class GameBlueprintsArchive : IDisposable
    {
        private readonly FileInfo _blueprintsFile;
        private readonly SharpCompress.Archives.Zip.ZipArchive _blueprints;
        private readonly IGameResourcesProvider _res;

        public GameBlueprintsArchive(string gameFolder, IGameResourcesProvider res)
        {
            if (string.IsNullOrEmpty(gameFolder)) return;
            _blueprintsFile = new FileInfo(Path.Combine(gameFolder, "Blueprints.zip"));
            if (!_blueprintsFile.Exists) return;

            _blueprints = SharpCompress.Archives.Zip.ZipArchive.Open(_blueprintsFile);
            _res = res;
        }

        public Blueprint Load(IBlueprintMetadataEntry metadata)
        {
            if (_blueprints is null) return null;
            var actualPath = metadata.Path.StartsWith("Blueprints\\", StringComparison.OrdinalIgnoreCase)
                ? metadata.Path.Remove(0, 11)
                : metadata.Path;

            actualPath = actualPath.Replace("\\", "/");

            var entry = _blueprints.Entries.FirstOrDefault(e => e.Key.IStart(actualPath));
            if (entry is null) return null;

            JObject blueprintJson;
            using (var stream = entry.OpenEntryStream())
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader)) {
                blueprintJson = JObject.Load(reader);
                return new Blueprint(new ModelDataAccessor(blueprintJson, new References()));
            }
        }

        public void Dispose()
        {
            if (_blueprints is object) _blueprints.Dispose();
        }
    }
}

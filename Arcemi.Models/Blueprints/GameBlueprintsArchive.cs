﻿using System;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Resources;

namespace Arcemi.Models
{
    public class GameBlueprintsArchive : IDisposable
    {
        private readonly FileInfo _blueprintsFile;
        private FileStream _blueprintsStream;
        private ZipArchive _blueprints;
        private readonly IGameResourcesProvider _res;

        public GameBlueprintsArchive(string gameFolder, IGameResourcesProvider res)
        {
            if (string.IsNullOrEmpty(gameFolder)) return;
            _blueprintsFile = new FileInfo(Path.Combine(gameFolder, "Blueprints.zip"));
            if (!_blueprintsFile.Exists) return;
            _res = res;
        }

        public Blueprint Load(IBlueprintMetadataEntry metadata)
        {
            if (!(_blueprintsFile?.Exists ?? false)) return null;

            if (_blueprints is null) {
                _blueprintsStream = new FileStream(_blueprintsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                _blueprints = new ZipArchive(_blueprintsStream);
            }
            var actualPath = metadata.Path.StartsWith("Blueprints\\", StringComparison.OrdinalIgnoreCase)
                ? metadata.Path.Remove(0, 11)
                : metadata.Path;

            actualPath = actualPath.Replace("\\", "/");

            var entry = _blueprints.GetEntry(actualPath);
            if (entry is null) return null;

            JObject blueprintJson;
            using (var stream = entry.Open())
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader)) {
                blueprintJson = JObject.Load(reader);
                return new Blueprint(new ModelDataAccessor(blueprintJson, new References()));
            }
        }

        public void Dispose()
        {
            if (_blueprints is object) {
                _blueprints.Dispose();
                _blueprints = null;
            }

            if (_blueprintsStream is object) {
                _blueprintsStream.Dispose();
                _blueprintsStream = null;
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace Arcemi.Models
{
    public class BlueprintMetadata : IDisposable
    {
        private readonly Dictionary<string, BlueprintMetadataEntry> _lookup;

        private readonly Dictionary<BlueprintType, List<BlueprintMetadataEntry>> _byType;

        public BlueprintMetadata(IBlueprintTypeProvider typeProvider) : this(Array.Empty<BlueprintMetadataEntry>(), typeProvider) { }
        public BlueprintMetadata(IReadOnlyList<BlueprintMetadataEntry> entries, IBlueprintTypeProvider typeProvider)
        {
            TypeProvider = typeProvider;
            _lookup = new Dictionary<string, BlueprintMetadataEntry>(StringComparer.Ordinal);
            _byType = new Dictionary<BlueprintType, List<BlueprintMetadataEntry>>();

            string clsType = default;
            List<BlueprintMetadataEntry> list = default;
            foreach (var entry in entries) {
                ((IBlueprintMetadataEntrySetup)entry).Type = typeProvider.Get(entry.TypeFullName);
                ((IBlueprintMetadataEntrySetup)entry).Name = BlueprintName.Detect(typeProvider, entry.Guid, ((IBlueprintMetadataEntry)entry).Type, entry.Name);

                _lookup.Add(entry.Guid, entry);

                if (clsType == null || !string.Equals(clsType, entry.TypeFullName)) {
                    if (!_byType.TryGetValue(entry.Type, out list)) {
                        list = new List<BlueprintMetadataEntry>();
                        _byType.Add(entry.Type, list);
                    }
                }
                list.Add(entry);
            }
        }

        public bool IsEmpty => _lookup.Count == 0;

        public IBlueprintTypeProvider TypeProvider { get; }

        public string GetNameOrBlueprint(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) return blueprint;
            return TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public IBlueprintMetadataEntry Get(string blueprintId)
        {
            if (_lookup.TryGetValue(blueprintId, out var entry)) {
                return entry;
            }
            throw new ArgumentException("Blueprint could not be found");
        }

        public bool TryGet(string blueprintId, out IBlueprintMetadataEntry blueprint)
        {
            if (_lookup.TryGetValue(blueprintId, out var entry)) {
                blueprint = entry;
                return true;
            }
            blueprint = default;
            return false;
        }

        public bool TryGetName(string blueprint, out string name)
        {
            if (_lookup.TryGetValue(blueprint, out var entry)) {
                name = entry.DisplayName;
                return true;
            }
            name = default;
            return false;
        }

        public bool TryGetType(string blueprint, out string type)
        {
            if (_lookup.TryGetValue(blueprint, out var entry)) {
                type = entry.TypeFullName;
                return true;
            }
            type = default;
            return false;
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(BlueprintTypeId id)
        {
            var type = TypeProvider.Get(id);
            if (type is null) throw new ArgumentOutOfRangeException(nameof(id), id, $"Blueprint '{id}' was missing from '{TypeProvider.GetType().Name}'");
            return GetEntries(type);
        }
        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName)) return Array.Empty<IBlueprintMetadataEntry>();
            var type = TypeProvider.Get(typeFullName);
            return GetEntries(type);
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(IEnumerable<BlueprintType> types)
        {
            return types.SelectMany(GetEntries).ToArray();
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(BlueprintType type)
        {
            if (_byType.TryGetValue(type, out var entries)) {
                return entries;
            }
            return Array.Empty<IBlueprintMetadataEntry>();
        }

        public static BlueprintMetadata Load(string gameFolder, IBlueprintTypeProvider typeProvider)
        {
            if (string.IsNullOrEmpty(gameFolder)) return new BlueprintMetadata(typeProvider);
            var cheatdataPath = Path.Combine(gameFolder, "Bundles", "cheatdata.json");
            BlueprintMetadataContainer cheatdata;
            if (!File.Exists(cheatdataPath)) {
                // Small backup
                cheatdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "Blueprints.json");
            }
            if (File.Exists(cheatdataPath)) {
                var serializer = new JsonSerializer();
                using (var stream = new FileStream(cheatdataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader)) {
                    cheatdata = serializer.Deserialize<BlueprintMetadataContainer>(jsonReader);
                }
            }
            else {
                return new BlueprintMetadata(typeProvider);
            }
            return new BlueprintMetadata(cheatdata.Entries, typeProvider);
        }

        public void Dispose()
        {
        }
    }
}

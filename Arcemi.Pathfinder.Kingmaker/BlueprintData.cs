using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintData
    {
        private readonly Dictionary<string, BlueprintEntry> _lookup;

        private readonly Dictionary<string, List<BlueprintEntry>> _byType;

        public static BlueprintData Empty { get; } = new BlueprintData(Array.Empty<BlueprintEntry>());

        public BlueprintData(IReadOnlyList<BlueprintEntry> entries)
        {
            _lookup = new Dictionary<string, BlueprintEntry>(StringComparer.Ordinal);
            _byType = new Dictionary<string, List<BlueprintEntry>>(StringComparer.Ordinal);

            string clsType = default;
            List<BlueprintEntry> list = default;
            foreach (var entry in entries) {
                _lookup.Add(entry.Guid, entry);

                if (clsType == null || !string.Equals(clsType, entry.TypeFullName)) {
                    clsType = entry.TypeFullName;
                    if (!_byType.TryGetValue(clsType, out list)) {
                        list = new List<BlueprintEntry>();
                        _byType.Add(clsType, list);
                    }
                }
                list.Add(entry);
            }
        }

        public bool IsEmpty => _lookup.Count == 0;

        public string GetNameOrBlueprint(string blueprint)
        {
            return TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public IBlueprint Get(string blueprintId)
        {
            if (_lookup.TryGetValue(blueprintId, out var entry)) {
                return entry;
            }
            throw new ArgumentException("Blueprint could not be found");
        }

        public bool TryGet(string blueprintId, out IBlueprint blueprint)
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

        public IReadOnlyList<IBlueprint> GetEntries(string typeFullName)
        {
            if (_byType.TryGetValue(typeFullName, out var entries)) {
                return entries;
            }
            return Array.Empty<IBlueprint>();
        }

        public static BlueprintData Load(string gameFolder)
        {
            if (string.IsNullOrEmpty(gameFolder)) return Empty;
            var cheatdataPath = Path.Combine(gameFolder, "Bundles", "cheatdata.json");
            BlueprintDataContainer cheatdata;
            if (File.Exists(cheatdataPath)) {
                var serializer = new JsonSerializer();
                using (var stream = new FileStream(cheatdataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader)) {
                    cheatdata = serializer.Deserialize<BlueprintDataContainer>(jsonReader);
                }
            }
            else {
                return Empty;
            }
            return new BlueprintData(cheatdata.Entries);
        }
    }
}

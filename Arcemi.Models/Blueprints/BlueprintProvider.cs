using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.Models
{
    public abstract class BlueprintProvider
    {
        private IReadOnlyList<BlueprintMetadataEntry> _entries;
        private Dictionary<string, BlueprintMetadataEntry> _entryLookup;
        private Dictionary<BlueprintType, List<BlueprintMetadataEntry>> _entryByType;

        private readonly IReadOnlyDictionary<string, BlueprintType> typeFullNameLookup;
        private readonly IReadOnlyDictionary<BlueprintTypeId, BlueprintType> typeIdLookup;

        public IReadOnlyList<IBlueprintMetadataEntry> Entries => _entries;
        public IEnumerable<BlueprintType> Types => typeFullNameLookup.Values;

        protected BlueprintProvider(IReadOnlyDictionary<string, BlueprintType> typeFullNameLookup, IReadOnlyDictionary<BlueprintTypeId, BlueprintType> typeIdLookup)
        {
            this.typeFullNameLookup = typeFullNameLookup;
            this.typeIdLookup = typeIdLookup;
            _entries = Array.Empty<BlueprintMetadataEntry>();
            _entryLookup = new Dictionary<string, BlueprintMetadataEntry>();
            _entryByType = new Dictionary<BlueprintType, List<BlueprintMetadataEntry>>();
        }
        public BlueprintType GetType(BlueprintTypeId id)
        {
            return typeIdLookup.TryGetValue(id, out var type) ? type : new BlueprintType("<Unknown>", id.ToString());
        }
        public BlueprintType GetType(string fullName)
        {
            return typeFullNameLookup.TryGetValue(fullName, out var type) ? type : new BlueprintType("<Unknown>", fullName);
        }

        public bool IsEmpty => _entryLookup.Count == 0;

        public string GetNameOrBlueprint(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) return blueprint;
            return TryGetName(blueprint, out var name) ? name : blueprint;
        }
        public IBlueprintMetadataEntry Get(string blueprintId)
        {
            if (_entryLookup.TryGetValue(blueprintId, out var entry)) {
                return entry;
            }
            throw new ArgumentException("Blueprint could not be found");
        }
        public bool TryGet(string blueprintId, out IBlueprintMetadataEntry blueprint)
        {
            if (_entryLookup.TryGetValue(blueprintId, out var entry)) {
                blueprint = entry;
                return true;
            }
            blueprint = default;
            return false;
        }
        public bool TryGetName(string blueprint, out string name)
        {
            if (_entryLookup.TryGetValue(blueprint, out var entry)) {
                name = entry.DisplayName;
                return true;
            }
            name = default;
            return false;
        }
        public bool TryGetType(string blueprint, out string type)
        {
            if (_entryLookup.TryGetValue(blueprint, out var entry)) {
                type = entry.TypeFullName;
                return true;
            }
            type = default;
            return false;
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(BlueprintTypeId id)
        {
            var type = GetType(id);
            if (type is null) throw new ArgumentOutOfRangeException(nameof(id), id, $"Blueprint '{id}' was missing from '{GetType().Name}'");
            return GetEntries(type);
        }
        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName)) return Array.Empty<IBlueprintMetadataEntry>();
            var type = GetType(typeFullName);
            return GetEntries(type);
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(IEnumerable<BlueprintType> types)
        {
            return types.SelectMany(GetEntries).ToArray();
        }

        public IReadOnlyList<IBlueprintMetadataEntry> GetEntries(BlueprintType type)
        {
            if (_entryByType.TryGetValue(type, out var entries)) {
                return entries;
            }
            return Array.Empty<IBlueprintMetadataEntry>();
        }

        public void SetEntries(IReadOnlyList<BlueprintMetadataEntry> entries)
        {
            _entries = entries;
            var lookup = new Dictionary<string, BlueprintMetadataEntry>(StringComparer.Ordinal);
            var byType = new Dictionary<BlueprintType, List<BlueprintMetadataEntry>>();
            string clsType = default;
            List<BlueprintMetadataEntry> list = default;
            foreach (var entry in _entries) {
                ((IBlueprintMetadataEntrySetup)entry).Type = GetType(entry.TypeFullName);
                ((IBlueprintMetadataEntrySetup)entry).Name = ResolveName(entry);

                if (lookup.ContainsKey(entry.Guid)) {
                    Logger.Current.Warning($"Entry {entry.Guid} '{entry.DisplayName}' already exists");
                    continue;
                }
                lookup.Add(entry.Guid, entry);

                if (clsType == null || !string.Equals(clsType, entry.TypeFullName)) {
                    if (!byType.TryGetValue(entry.Type, out list)) {
                        list = new List<BlueprintMetadataEntry>();
                        byType.Add(entry.Type, list);
                    }
                }
                list.Add(entry);
            }
            _entryByType = byType;
            _entryLookup = lookup;
        }

        public async Task SetupAsync(BlueprintProviderSetupArgs args)
        {
            await OnBeforeSetupAsync(args);

            var cheatdataPath = args.GameFolder.HasValue()
                ? Path.Combine(args.GameFolder, "Bundles", "cheatdata.json")
                : null;

            BlueprintMetadataContainer cheatdata;
            if (!File.Exists(cheatdataPath)) {
                // Small backup
                cheatdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", $"Blueprints_{args.Game.Id}.json");
                Logger.Current.Information($"{GetType().Name} using blueprint backup at '{cheatdataPath}'");
            }
            if (!File.Exists(cheatdataPath)) {
                Logger.Current.Information($"{GetType().Name} could not load blueprints");
                return;
            }
            var serializer = new JsonSerializer();
            using (var stream = new FileStream(cheatdataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader)) {
                cheatdata = serializer.Deserialize<BlueprintMetadataContainer>(jsonReader);
            }
            SetEntries(cheatdata.Entries);

            await OnAfterSetupAsync(args);
        }

        protected virtual BlueprintName ResolveName(BlueprintMetadataEntry entry)
        {
            return BlueprintName.Detect(this, entry.Guid, ((IBlueprintMetadataEntry)entry).Type, entry.Name);
        }

        protected virtual Task OnBeforeSetupAsync(BlueprintProviderSetupArgs args) { return Task.CompletedTask; }
        protected virtual Task OnAfterSetupAsync(BlueprintProviderSetupArgs args) { return Task.CompletedTask; }
    }

    public class BlueprintProviderSetupArgs
    {
        public BlueprintProviderSetupArgs(GameDefinition game, string workingDirectory, string gameFolder)
        {
            Game = game;
            WorkingDirectory = workingDirectory;
            GameFolder = gameFolder;
        }

        public GameDefinition Game { get; }
        public string WorkingDirectory { get; }
        public string GameFolder { get; }
    }
}

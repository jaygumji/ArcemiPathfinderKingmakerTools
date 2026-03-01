#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcemi.Models.BinaryTraversal.Owl;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Arcemi.Models
{
    public class SaveGameFile
    {
        public string Filepath { get; private set; }
        public string WorkingPath { get; }

        private readonly Dictionary<string, string> _lookup;

        public SaveGameFile(string path)
        {
            Filepath = path;
            WorkingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            _lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Extract();
        }

        public static HeaderModel ReadHeader(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read)) {
                var entry = archive.Entries.FirstOrDefault(e => string.Equals(e.Name, "header.json", StringComparison.OrdinalIgnoreCase));
                if (entry == null) return null;
                using (var entryStream = entry.Open())
                using (var entryReader = new StreamReader(entryStream))
                using (var entryJsonReader = new JsonTextReader(entryReader)) {
                    var refs = new References();
                    var obj = JObject.Load(entryJsonReader);
                    var accessor = new ModelDataAccessor(obj, refs);
                    return new HeaderModel(accessor);
                }
            }
        }

        private void Extract()
        {
            if (!Directory.Exists(WorkingPath)) {
                Directory.CreateDirectory(WorkingPath);
            }

            using (var stream = new FileStream(Filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read)) {
                    foreach (var entry in archive.Entries) {
                        using (var entryStream = entry.Open()) {
                            var writePath = Path.Combine(WorkingPath, entry.Name);
                            _lookup.Add(entry.Name, writePath);

                            using (var writeStream = new FileStream(writePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read)) {
                                entryStream.CopyTo(writeStream);
                            }
                        }
                    }
                }
            }
        }

        public JsonPartSaveGameFile GetParty()
        {
            if (HasEntry("party.json")) {
                return new JsonPartSaveGameFile(GetWorkingPath("party.json"), GetJson("party.json"));
            }

            var header = GetHeader().GetRoot<HeaderModel>();
            var path = GetWorkingPath("party.owl");
            var originalBytes = File.ReadAllBytes(path);
            var analysis = Warhammer40KDarkHeresy.W40KDHPartyAnalysis.AnalyzeToJson(header, originalBytes);
            var dynamicSlots = CreateDarkHeresyDynamicSlotsFromAnalysis(analysis);
            return GetOwlPart("party.owl", () => CreateDarkHeresyPartySeedJson(analysis), dynamicSlots, ApplyDarkHeresyPartyPatches);
        }

        public JsonPartSaveGameFile GetPlayer()
        {
            if (HasEntry("player.json")) {
                return new JsonPartSaveGameFile(GetWorkingPath("player.json"), GetJson("player.json"));
            }
            return GetOwlPart("player.owl", CreateDarkHeresyPlayerSeedJson);
        }

        public JsonPartSaveGameFile GetHeader()
        {
            return new JsonPartSaveGameFile(GetWorkingPath("header.json"), GetJson("header.json"));
        }

        public void Close()
        {
            foreach (var file in Directory.EnumerateFiles(WorkingPath)) {
                File.Delete(file);
            }
            Directory.Delete(WorkingPath);
        }

        private string GetWorkingPath(string name)
        {
            if (!_lookup.TryGetValue(name, out var path)) {
                throw new ArgumentException($"No entries with the name {name} exists.");
            }
            return path;
        }

        private bool HasEntry(string name)
        {
            return _lookup.ContainsKey(name);
        }

        public string GetText(string name)
        {
            return File.ReadAllText(GetWorkingPath(name), Encoding.UTF8);
        }

        public void Save(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create)) {
                    foreach (var file in Directory.EnumerateFiles(WorkingPath)) {
                        var entryName = Path.GetFileName(file);
                        archive.CreateEntryFromFile(file, entryName);
                    }
                }
            }
            Filepath = filePath;
        }

        public JObject GetJson(string name)
        {
            using (var stream = new FileStream(GetWorkingPath(name), FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var streamReader = new StreamReader(stream, Encoding.UTF8)) {
                    using (var reader = new JsonTextReader(streamReader)) {
                        return JObject.Load(reader);
                    }
                }
            }
        }

        private JsonPartSaveGameFile GetOwlPart(
            string entryName,
            Func<JObject> seedFactory,
            IReadOnlyList<BinaryTraversal.BinarySlotDefinition> dynamicSlots = null,
            Action<OwlBinaryDocument, JObject> beforeSave = null)
        {
            var path = GetWorkingPath(entryName);
            var originalBytes = File.ReadAllBytes(path);
            var baseSlots = Warhammer40KDarkHeresy.W40KDHBinarySlotCatalog.GetSlotsForEntry(entryName);
            var slots = dynamicSlots is object && dynamicSlots.Count > 0
                ? baseSlots.Concat(dynamicSlots).ToArray()
                : baseSlots;
            var document = new OwlBinaryDocument(originalBytes, slots);
            var json = seedFactory();

            return new JsonPartSaveGameFile(path, json, _ => {
                beforeSave?.Invoke(document, json);
                File.WriteAllBytes(path, document.Save());
            });
        }

        private static JObject CreateDarkHeresyPartySeedJson(JObject analysis)
        {
            return new JObject {
                { "$id", "1" },
                { "m_EntityData", new JArray() },
                { "__owl", true },
                { "__arcemi_dh", analysis ?? new JObject() }
            };
        }

        private static JObject CreateDarkHeresyPlayerSeedJson()
        {
            return new JObject {
                { "$id", "1" },
                { "__owl", true }
            };
        }

        private static IReadOnlyList<BinaryTraversal.BinarySlotDefinition> CreateDarkHeresyDynamicSlotsFromAnalysis(JObject analysis)
        {
            var slots = new List<BinaryTraversal.BinarySlotDefinition>();
            var seenPaths = new HashSet<string>(StringComparer.Ordinal);
            var chars = analysis?["characters"] as JArray;
            if (chars is null) return slots;

            foreach (var ch in chars.OfType<JObject>()) {
                var rankOverrides = new Dictionary<string, int>(StringComparer.Ordinal);
                var stats = ch["stats"] as JArray;
                if (stats is object) {
                    foreach (var stat in stats.OfType<JObject>()) {
                        var slotPath = stat["slotPath"]?.Value<string>();
                        if (string.IsNullOrWhiteSpace(slotPath)) continue;
                        var offset = ParseOffsetFromSlotPath(slotPath);
                        if (!offset.HasValue) continue;
                        var path = BinaryTraversal.BinaryPath.Parse(slotPath);
                        if (!seenPaths.Add(path.Value)) continue;
                        slots.Add(new BinaryTraversal.BinarySlotDefinition(
                            path,
                            BinaryTraversal.BinarySlotValueKind.Int32,
                            offset.Value,
                            sizeof(int)));
                    }
                }

                var selections = ch["selections"] as JArray;
                if (selections is object) {
                    foreach (var selection in selections.OfType<JObject>()) {
                        var slotPath = selection["rankSlotPath"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(slotPath)) {
                            var offset = ParseOffsetFromSlotPath(slotPath);
                            if (offset.HasValue) {
                                var path = BinaryTraversal.BinaryPath.Parse(slotPath);
                                if (!seenPaths.Add(path.Value)) continue;
                                slots.Add(new BinaryTraversal.BinarySlotDefinition(
                                    path,
                                    BinaryTraversal.BinarySlotValueKind.Int32,
                                    offset.Value,
                                    sizeof(int)));
                            }
                        }

                        var featureSlotPath = selection["featureSlotPath"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(featureSlotPath)) {
                            var featureOffset = ParseUtf8OffsetFromSlotPath(featureSlotPath);
                            var featureLength = ParseUtf8LengthFromSlotPath(featureSlotPath);
                            if (featureOffset.HasValue && featureLength.HasValue) {
                                var path = BinaryTraversal.BinaryPath.Parse(featureSlotPath);
                                if (!seenPaths.Add(path.Value)) continue;
                                slots.Add(new BinaryTraversal.BinarySlotDefinition(
                                    path,
                                    BinaryTraversal.BinarySlotValueKind.Utf8String,
                                    featureOffset.Value,
                                    featureLength.Value));
                            }
                        }
                    }
                }
            }

            return slots;
        }

        private static void ApplyDarkHeresyPartyPatches(OwlBinaryDocument document, JObject json)
        {
            var chars = json?["__arcemi_dh"]?["characters"] as JArray;
            if (chars is null) return;

            foreach (var ch in chars.OfType<JObject>()) {
                var rankOverrides = new Dictionary<string, int>(StringComparer.Ordinal);
                var stats = ch["stats"] as JArray;
                if (stats is object) {
                    foreach (var stat in stats.OfType<JObject>()) {
                        var slotPath = stat["slotPath"]?.Value<string>();
                        if (string.IsNullOrWhiteSpace(slotPath)) continue;
                        var value = stat["value"]?.Value<int>() ?? 0;
                        var slotScale = stat["slotScale"]?.Value<int>() ?? 1;
                        var slotBase = stat["slotBase"]?.Value<int>() ?? 0;
                        if (slotScale > 1) {
                            value = (int)Math.Round((value - slotBase) / (double)slotScale, MidpointRounding.AwayFromZero);
                            if (value < 0) value = 0;
                        }
                        rankOverrides[slotPath] = value;
                        try {
                            document.Set(BinaryTraversal.BinaryPath.Parse(slotPath), value);
                        }
                        catch {
                            // Ignore broken dynamic slot entries to avoid blocking save entirely.
                        }
                    }
                }

                var selections = ch["selections"] as JArray;
                if (selections is object) {
                    foreach (var selection in selections.OfType<JObject>()) {
                        var rankSlotPath = selection["rankSlotPath"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(rankSlotPath)) {
                            var rank = rankOverrides.TryGetValue(rankSlotPath, out var mappedRank)
                                ? mappedRank
                                : (selection["rank"]?.Value<int>() ?? 0);
                            if (rank < 0) rank = 0;
                            try {
                                document.Set(BinaryTraversal.BinaryPath.Parse(rankSlotPath), rank);
                            }
                            catch {
                            }
                        }

                        var featureSlotPath = selection["featureSlotPath"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(featureSlotPath)) {
                            var featureBlueprint = selection["featureBlueprint"]?.Value<string>() ?? string.Empty;
                            try {
                                document.Set(BinaryTraversal.BinaryPath.Parse(featureSlotPath), featureBlueprint);
                            }
                            catch {
                            }
                        }
                    }
                }
            }
        }

        private static int? ParseOffsetFromSlotPath(string slotPath)
        {
            var marker = ".i32@";
            var idx = slotPath.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return null;
            var number = slotPath.Substring(idx + marker.Length);
            if (!int.TryParse(number, out var offset)) return null;
            return offset >= 0 ? offset : null;
        }

        private static int? ParseUtf8OffsetFromSlotPath(string slotPath)
        {
            var marker = ".str@";
            var idx = slotPath.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return null;
            var payload = slotPath.Substring(idx + marker.Length);
            var separator = payload.IndexOf(':');
            if (separator <= 0) return null;
            return int.TryParse(payload.Substring(0, separator), out var offset) && offset >= 0
                ? offset
                : null;
        }

        private static int? ParseUtf8LengthFromSlotPath(string slotPath)
        {
            var marker = ".str@";
            var idx = slotPath.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return null;
            var payload = slotPath.Substring(idx + marker.Length);
            var separator = payload.IndexOf(':');
            if (separator <= 0 || separator + 1 >= payload.Length) return null;
            return int.TryParse(payload.Substring(separator + 1), out var length) && length > 0
                ? length
                : null;
        }

    }
}

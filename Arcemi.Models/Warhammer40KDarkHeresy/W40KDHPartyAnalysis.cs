using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHAnalyzedStat
    {
        public W40KDHAnalyzedStat(string name, int value, bool isEditable, string source)
        {
            Name = name;
            Value = value;
            IsEditable = isEditable;
            Source = source;
        }

        public string Name { get; }
        public int Value { get; set; }
        public bool IsEditable { get; set; }
        public string Source { get; }
        public string SlotPath { get; set; }
        public int SlotScale { get; set; } = 1;
        public int SlotBase { get; set; }
        public JObject JsonRef { get; set; }
    }

    internal sealed class W40KDHAnalyzedSelection
    {
        public string FeatureBlueprint { get; set; }
        public int? FeatureBlueprintOffset { get; set; }
        public int? FeatureBlueprintLength { get; set; }
        public int Rank { get; set; }
        public int? RankOffset { get; set; }
        public JObject JsonRef { get; set; }
    }

    internal sealed class W40KDHAnalyzedCharacter
    {
        public W40KDHAnalyzedCharacter(int index)
        {
            Index = index;
            Stats = new List<W40KDHAnalyzedStat>();
        }

        public int Index { get; }
        public bool IsMainCharacter { get; set; }
        public bool IsInCurrentSquad { get; set; }
        public string PortraitBlueprint { get; set; }
        public string UnitBlueprint { get; set; }
        public string DisplayName { get; set; }
        public List<W40KDHAnalyzedStat> Stats { get; }
        public List<W40KDHAnalyzedSelection> Selections { get; } = new();
        public int DebugFactCount { get; set; }
        public int DebugFeatureFactCount { get; set; }
        public IReadOnlyDictionary<string, int> DebugFactTypes { get; set; } = new Dictionary<string, int>(StringComparer.Ordinal);
        public JObject JsonRef { get; set; }
    }

    internal static class W40KDHPartyAnalysis
    {
        private static readonly string[] CoreCharacteristics = {
            "Weapon Skill",
            "Ballistic Skill",
            "Strength",
            "Toughness",
            "Agility",
            "Intelligence",
            "Perception",
            "Willpower",
            "Fellowship",
        };

        public static JObject AnalyzeToJson(HeaderModel header, byte[] partyBytes)
        {
            if (header is null) throw new ArgumentNullException(nameof(header));
            if (partyBytes is null) throw new ArgumentNullException(nameof(partyBytes));

            var result = new JObject {
                { "characters", new JArray() }
            };

            var chars = (JArray)result["characters"];
            var parsedUnits = W40KDHOwlTraversal.ReadUnitStats(partyBytes);

            for (var i = 0; i < parsedUnits.Count; i++) {
                var unit = parsedUnits[i];
                var ch = new W40KDHAnalyzedCharacter(i) {
                    IsMainCharacter = unit.IsMainCharacter,
                    IsInCurrentSquad = unit.IsMainCharacter || unit.InParty == true,
                    PortraitBlueprint = unit.PortraitBlueprint,
                    UnitBlueprint = unit.UnitBlueprint,
                    DisplayName = unit.CustomName,
                };

                AddParsedUnitValues(ch, unit);
                if (ch.IsMainCharacter) {
                    EnsureCoreCharacteristics(ch);
                }

                chars.Add(ToJson(ch));
            }

            if (chars.Count == 0) {
                var fallback = new W40KDHAnalyzedCharacter(0) {
                    IsMainCharacter = true,
                    IsInCurrentSquad = true,
                };
                EnsureCoreCharacteristics(fallback);
                chars.Add(ToJson(fallback));
            }

            // Some saves don't expose UnitPartMainCharacter in serialized parts. Keep one
            // deterministic main character so the UI can still open the roster.
            if (!chars.OfType<JObject>().Any(x => x["isMainCharacter"]?.Value<bool>() == true)
                && chars.Count > 0
                && chars[0] is JObject firstCharacter) {
                firstCharacter["isMainCharacter"] = true;
            }

            return result;
        }

        public static IReadOnlyList<W40KDHAnalyzedCharacter> ReadFromJson(JObject partyRoot)
        {
            var res = new List<W40KDHAnalyzedCharacter>();
            var chars = partyRoot?["__arcemi_dh"]?["characters"] as JArray;
            if (chars is null) return res;

            foreach (var token in chars) {
                if (token is not JObject ch) continue;
                var index = ch["index"]?.Value<int>() ?? -1;
                if (index < 0) continue;

                var model = new W40KDHAnalyzedCharacter(index) {
                    JsonRef = ch,
                    IsMainCharacter = ch["isMainCharacter"]?.Value<bool>() == true,
                    IsInCurrentSquad = ch["isInCurrentSquad"]?.Value<bool>() == true,
                    PortraitBlueprint = ch["portraitBlueprint"]?.Value<string>(),
                    UnitBlueprint = ch["unitBlueprint"]?.Value<string>(),
                    DisplayName = ch["displayName"]?.Value<string>(),
                };

                var stats = new List<W40KDHAnalyzedStat>();
                var statArr = ch["stats"] as JArray;
                if (statArr is object) {
                    foreach (var statToken in statArr) {
                        if (statToken is not JObject statObj) continue;
                        var name = statObj["name"]?.Value<string>();
                        if (string.IsNullOrWhiteSpace(name)) continue;
                        var value = statObj["value"]?.Value<int>() ?? 0;
                        var editable = statObj["editable"]?.Value<bool>() == true;
                        var source = statObj["source"]?.Value<string>() ?? string.Empty;
                        var slotPath = statObj["slotPath"]?.Value<string>();
                        var slotScale = statObj["slotScale"]?.Value<int>() ?? 1;
                        var slotBase = statObj["slotBase"]?.Value<int>() ?? 0;
                        stats.Add(new W40KDHAnalyzedStat(name, value, editable, source) {
                            SlotPath = slotPath,
                            SlotScale = slotScale <= 0 ? 1 : slotScale,
                            SlotBase = slotBase,
                            JsonRef = statObj
                        });
                    }
                }

                var selections = ch["selections"] as JArray;
                if (selections is object) {
                    foreach (var selectionToken in selections.OfType<JObject>()) {
                        var featureBlueprint = selectionToken["featureBlueprint"]?.Value<string>();
                        var featureOffset = ParseUtf8OffsetFromSlotPath(selectionToken["featureSlotPath"]?.Value<string>());
                        var featureLength = ParseUtf8LengthFromSlotPath(selectionToken["featureSlotPath"]?.Value<string>());
                        var rankOffset = ParseIntOffsetFromSlotPath(selectionToken["rankSlotPath"]?.Value<string>());
                        if (string.IsNullOrWhiteSpace(featureBlueprint) && !featureOffset.HasValue) continue;
                        model.Selections.Add(new W40KDHAnalyzedSelection {
                            FeatureBlueprint = featureBlueprint ?? string.Empty,
                            FeatureBlueprintOffset = featureOffset,
                            FeatureBlueprintLength = featureLength,
                            Rank = selectionToken["rank"]?.Value<int>() ?? 0,
                            RankOffset = rankOffset,
                            JsonRef = selectionToken,
                        });
                    }
                }

                model.Stats.AddRange(stats);
                res.Add(model);
            }

            return res.OrderBy(x => x.Index).ToArray();
        }

        private static JObject ToJson(W40KDHAnalyzedCharacter ch)
        {
            var stats = new JArray();
            foreach (var stat in ch.Stats) {
                var statObj = new JObject {
                    { "name", stat.Name },
                    { "value", stat.Value },
                    { "editable", stat.IsEditable },
                    { "source", stat.Source },
                    { "slotPath", stat.SlotPath ?? string.Empty },
                    { "slotScale", stat.SlotScale },
                    { "slotBase", stat.SlotBase },
                };
                stat.JsonRef = statObj;
                stats.Add(statObj);
            }

            var selections = new JArray();
            foreach (var selection in ch.Selections) {
                var selectionObj = new JObject {
                    { "featureBlueprint", selection.FeatureBlueprint ?? string.Empty },
                    { "featureSlotPath", selection.FeatureBlueprintOffset.HasValue && selection.FeatureBlueprintLength.HasValue
                        ? BuildUtf8SlotPath(ch.Index, "SelectionFeature", selection.FeatureBlueprintOffset.Value, selection.FeatureBlueprintLength.Value)
                        : string.Empty },
                    { "rank", selection.Rank },
                    { "rankSlotPath", selection.RankOffset.HasValue ? BuildInt32SlotPath(ch.Index, "SelectionRank", selection.RankOffset.Value) : string.Empty },
                };
                selection.JsonRef = selectionObj;
                selections.Add(selectionObj);
            }

            var characterObj = new JObject {
                { "index", ch.Index },
                { "isMainCharacter", ch.IsMainCharacter },
                { "isInCurrentSquad", ch.IsInCurrentSquad },
                { "portraitBlueprint", ch.PortraitBlueprint ?? string.Empty },
                { "unitBlueprint", ch.UnitBlueprint ?? string.Empty },
                { "displayName", ch.DisplayName ?? string.Empty },
                { "stats", stats },
                { "selections", selections },
                { "debugFacts", new JObject {
                    { "factCount", ch.DebugFactCount },
                    { "featureFactCount", ch.DebugFeatureFactCount },
                    { "factTypes", new JObject(ch.DebugFactTypes.OrderByDescending(x => x.Value).Select(kv => new JProperty(kv.Key, kv.Value))) },
                } },
            };
            ch.JsonRef = characterObj;
            return characterObj;
        }

        private static void AddParsedUnitValues(W40KDHAnalyzedCharacter ch, W40KDHParsedUnitStats unit)
        {
            if (unit is null) return;

            if (unit.CharacterLevel.HasValue) {
                ch.Stats.Add(new W40KDHAnalyzedStat("Character Level", unit.CharacterLevel.Value, isEditable: unit.CharacterLevelOffset.HasValue, source: "party.owl") {
                    SlotPath = unit.CharacterLevelOffset.HasValue
                        ? BuildInt32SlotPath(ch.Index, "CharacterLevel", unit.CharacterLevelOffset.Value)
                        : null
                });
            }

            if (unit.Experience.HasValue) {
                ch.Stats.Add(new W40KDHAnalyzedStat("Experience", unit.Experience.Value, isEditable: unit.ExperienceOffset.HasValue, source: "party.owl") {
                    SlotPath = unit.ExperienceOffset.HasValue
                        ? BuildInt32SlotPath(ch.Index, "Experience", unit.ExperienceOffset.Value)
                        : null
                });
            }

            if (unit.MissingHpFraction.HasValue) {
                var hpPercent = Math.Clamp(100 - (int)Math.Round(unit.MissingHpFraction.Value * 100), 0, 100);
                ch.Stats.Add(new W40KDHAnalyzedStat("Health %", hpPercent, isEditable: false, source: "party.owl"));
            }

            if (unit.ProgressionSelections.Count > 0) {
                ch.Selections.AddRange(unit.ProgressionSelections.Select(x => new W40KDHAnalyzedSelection {
                    FeatureBlueprint = x.FeatureBlueprint,
                    FeatureBlueprintOffset = x.FeatureBlueprintOffset,
                    FeatureBlueprintLength = x.FeatureBlueprintLength,
                    Rank = x.Rank,
                    RankOffset = x.RankOffset,
                }));
            }

            if (unit.Characteristics.Count > 0) {
                foreach (var item in unit.Characteristics.Values) {
                    var existing = ch.Stats.FirstOrDefault(x => string.Equals(x.Name, item.Name, StringComparison.Ordinal));
                    if (existing is null) {
                        ch.Stats.Add(new W40KDHAnalyzedStat(item.Name, item.Value, isEditable: item.Offset.HasValue, source: "party.owl") {
                            SlotPath = item.Offset.HasValue
                                ? BuildInt32SlotPath(ch.Index, item.Name.Replace(" ", string.Empty, StringComparison.Ordinal), item.Offset.Value)
                                : null
                        });
                    }
                    else {
                        existing.Value = item.Value;
                        existing.IsEditable = existing.IsEditable || item.Offset.HasValue;
                        existing.SlotPath = existing.SlotPath.OrIfEmpty(
                            item.Offset.HasValue
                                ? BuildInt32SlotPath(ch.Index, item.Name.Replace(" ", string.Empty, StringComparison.Ordinal), item.Offset.Value)
                                : null);
                    }
                }
            }
            ch.DebugFactCount = unit.DebugFactCount;
            ch.DebugFeatureFactCount = unit.DebugFeatureFactCount;
            ch.DebugFactTypes = new Dictionary<string, int>(unit.DebugFactTypes, StringComparer.Ordinal);
        }

        private static string BuildInt32SlotPath(int characterIndex, string statName, int offset)
        {
            return string.Concat("chars[", characterIndex, "].stats.", statName, ".i32@", offset);
        }

        private static string BuildUtf8SlotPath(int characterIndex, string statName, int offset, int length)
        {
            return string.Concat("chars[", characterIndex, "].stats.", statName, ".str@", offset, ":", length);
        }

        private static int? ParseIntOffsetFromSlotPath(string slotPath)
        {
            if (string.IsNullOrWhiteSpace(slotPath)) return null;
            var marker = ".i32@";
            var idx = slotPath.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return null;
            return int.TryParse(slotPath.Substring(idx + marker.Length), out var offset) && offset >= 0
                ? offset
                : null;
        }

        private static int? ParseUtf8OffsetFromSlotPath(string slotPath)
        {
            if (string.IsNullOrWhiteSpace(slotPath)) return null;
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
            if (string.IsNullOrWhiteSpace(slotPath)) return null;
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

        private static void EnsureCoreCharacteristics(W40KDHAnalyzedCharacter ch)
        {
            var names = new HashSet<string>(ch.Stats.Select(x => x.Name), StringComparer.Ordinal);
            foreach (var statName in CoreCharacteristics) {
                if (names.Contains(statName)) continue;
                // We currently parse reliable progression fields from OWL, but characteristic
                // base values are not yet fully reconstructed. Keep placeholders visible in UI.
                ch.Stats.Add(new W40KDHAnalyzedStat(statName, 0, isEditable: false, source: "inferred"));
            }
        }
    }
}

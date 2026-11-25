using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal static class W40KRTProgressionFacts
    {
        private static W40KRTBlueprintProvider Blueprints => (W40KRTBlueprintProvider)GameDefinition.Warhammer40K_RogueTrader.Resources.Blueprints;

        public static IEnumerable<string> GetAutomaticFeatures(string path, int rank)
        {
            var entries = Blueprints.GetAssetData(path)?.RankEntries;
            return entries?.ElementAtOrDefault(rank - 1)?.Features?.Select(BlueprintId);
        }

        private static string BlueprintId(string reference) =>
            reference?.StartsWith("!bp_", StringComparison.Ordinal) == true ? reference.Substring(4) : reference;

        public static bool HasSources(FactItemModel fact) => Sources(fact)?.Count > 0;

        private static ListAccessor<RefModel> Sources(FactItemModel fact, bool create = false) =>
            fact.GetAccessor().List<RefModel>("m_Sources", createIfNull: create);

        public static void AddSelectionRank(IGameUnitModel owner, UnitProgressionSelectionOfPartModel selection)
        {
            var parent = owner.Progression.Selections.OfType<W40KRTGameUnitSelectionProgressionEntry>()
                .FirstOrDefault(e => W40KRTCharacterSelection.TryGet(e.Ref.Selection, out var definition)
                    && definition.Options.Any(o => o.Id.Eq(e.Feature) && o.SubSelectionId.Eq(selection.Selection)));
            selection.Rank = AddPathRank(owner, selection.Feature, selection.Path, parent?.Feature ?? selection.Path, selection.Level);
        }

        public static int AddPathRank(IGameUnitModel owner, string blueprint, string path, string source, int level)
        {
            var feat = (W40KRTGameUnitFeatEntry)owner.Feats.FirstOrDefault(f => f.Blueprint.Eq(blueprint));
            if (feat is null) {
                feat = (W40KRTGameUnitFeatEntry)owner.Feats.AddByBlueprint(blueprint);
            }
            else {
                feat.Rank++;
            }
            Sources(feat.Model, create: true).Add((_, obj) => {
                obj.Add("m_Blueprint", path);
                obj.Add("m_PathFeatureSource", source);
                obj.Add("m_PathRank", level);
                obj.Add("m_FeatureRank", feat.Rank);
            });
            return feat.Rank;
        }

        public static void RemoveSelectionRank(IGameUnitModel owner, UnitProgressionSelectionOfPartModel selection, bool removeDuplicates = false)
        {
            var retained = owner.Progression.Selections.OfType<W40KRTGameUnitSelectionProgressionEntry>()
                .Where(s => s.Ref != selection && s.Feature.Eq(selection.Feature)).ToArray();
            RemoveRanks(owner, selection.Feature, selection.Path, selection.Level, null, retained.Length, removeDuplicates, selection.Rank);
            foreach (var other in retained.Where(s => s.Ref.Rank > selection.Rank)) {
                other.Ref.Rank--;
            }
        }

        public static void RollbackDependentChoices(IGameUnitModel owner, string feature)
        {
            var unlocked = owner.Feats.OfType<W40KRTGameUnitFeatEntry>()
                .Where(f => Sources(f.Model)?.Any(s => s?.GetAccessor().Value<string>("m_PathFeatureSource").Eq(feature) == true) == true)
                .ToLookup(f => f.Blueprint, StringComparer.Ordinal);
            var first = owner.Progression.Selections.OfType<W40KRTGameUnitSelectionProgressionEntry>()
                .Where(e => !e.IsCreation && unlocked[e.Feature].Any(f => Sources(f.Model)
                    .Any(s => IsPathSource(s, e.Ref.Path, e.Ref.Level, feature))))
                .Select(e => e.Level).DefaultIfEmpty().Min();
            if (first > 1) owner.Progression.CurrentLevel = first - 1;
        }

        public static void RemovePathRank(IGameUnitModel owner, string path, int level, IEnumerable<string> automatic)
        {
            var known = automatic.Where(id => id.HasValue()).ToHashSet(StringComparer.Ordinal);
            var sourced = owner.Feats.OfType<W40KRTGameUnitFeatEntry>()
                .Where(f => Sources(f.Model)?.Any(s => IsPathSource(s, path, level, path)) == true)
                .Select(f => f.Blueprint).ToArray();
            foreach (var id in known.Concat(sourced).Distinct(StringComparer.Ordinal)) {
                RemoveRanks(owner, id, path, level, path, 0, false);
            }
        }

        private static bool IsPathSource(RefModel source, string path, int level, string selection)
        {
            if (source is null) return false;
            var a = source.GetAccessor();
            var sourceSelection = a.Value<string>("m_PathFeatureSource");
            return a.Value<string>("m_Blueprint").Eq(path) && a.Value<int?>("m_PathRank") == level
                && (selection is null || sourceSelection.Eq(selection) || !sourceSelection.HasValue());
        }

        private static void RemoveRanks(IGameUnitModel owner, string id, string path, int level, string selection, int retained, bool removeDuplicates, int? featureRank = null)
        {
            var feats = owner.Feats.OfType<W40KRTGameUnitFeatEntry>().Where(f => f.Blueprint.Eq(id)).ToArray();
            var changed = false;
            foreach (var feat in feats) {
                var sources = Sources(feat.Model);
                var matching = sources?.Where(s => IsPathSource(s, path, level, selection)).ToArray();
                var source = matching?.LastOrDefault(s => s.GetAccessor().Value<int>("m_FeatureRank") == featureRank)
                    ?? matching?.LastOrDefault();
                if (source is null) continue;
                var rank = source.GetAccessor().Value<int>("m_FeatureRank");
                sources.Remove(source);
                changed = true;
                foreach (var higher in sources.Where(s => s?.GetAccessor().Value<int>("m_FeatureRank") > rank)) {
                    var a = higher.GetAccessor();
                    a.Value(a.Value<int>("m_FeatureRank") - 1, "m_FeatureRank");
                }
                if (feat.Rank > 1) feat.Rank--;
                else if (sources.Count == 0 && retained == 0) owner.Feats.Remove(feat);
            }

            if (changed && !removeDuplicates) return;
            foreach (var feat in feats.Where(f => !HasSources(f.Model) && owner.Feats.IndexOf(f) >= 0)) {
                if (feat.Rank <= 1 && retained == 0) owner.Feats.Remove(feat);
                else if (!changed && feat.Rank > retained) feat.Rank--;
                if (!removeDuplicates || retained > 0) break;
            }
        }

        public static void RemoveGrants(W40KRTGameUnitModel owner, FactItemModel fact)
        {
            var id = fact.UniqueId;
            if (!id.HasValue()) return;

            // Older editor builds reused the unit ID for every feature they created.
            var useBlueprint = id.Eq(owner.UniqueId) || owner.Ref.Facts.Items.Any(f => f.UniqueId.Eq(id));
            var grants = Blueprints.GetAssetData(fact.Blueprint)?.FactGrants;
            var legacyGrants = (grants ?? GetFallbackGrants(fact.Blueprint)).Select(BlueprintId).ToHashSet(StringComparer.Ordinal);
            RemoveSourcedFacts(owner, (child, source) => {
                var a = source.GetAccessor().UnderlyingObject;
                var reference = a["m_FactRef"];
                if (!((string)reference?["EntityId"]).Eq(owner.UniqueId) || !((string)reference?["FactId"]).Eq(id)) return false;
                if (!useBlueprint) return true;
                var component = (string)a["m_ComponentId"];
                return legacyGrants.Contains(child.Blueprint) && component?.StartsWith("$AddFacts$", StringComparison.Ordinal) == true;
            });
        }

        private static string[] GetFallbackGrants(string blueprint)
        {
            return blueprint switch {
                "d840a5dc947546e0b4ac939287191fd8" => new[] { "33638b28fa5e49748f4a7a5eed5ec035", "171462027afe4bc6be83799f44fa9995" },
                "33638b28fa5e49748f4a7a5eed5ec035" => new[] { "c647c1c4a1a14c9cb31217ee439164ff" },
                _ => Array.Empty<string>()
            };
        }

        public static void RemoveItemGrants(W40KRTGameUnitModel owner, string itemId)
        {
            RemoveSourcedFacts(owner, (_, source) => source.GetAccessor().Value<string>("m_EntityRef").Eq(itemId));
        }

        private static void RemoveSourcedFacts(W40KRTGameUnitModel owner, Func<FactItemModel, RefModel, bool> isGrant)
        {
            foreach (var fact in owner.Ref.Facts.Items.ToArray()) {
                if (fact is W40KRTSoulMarkFactItemModel || !owner.Ref.Facts.Items.Contains(fact)) continue;
                var sources = Sources(fact);
                if (sources is null) continue;
                var removed = false;
                foreach (var source in sources.Where(s => s != null && isGrant(fact, s)).ToArray()) {
                    removed |= sources.Remove(source);
                }
                if (!removed || sources.Count > 0) continue;

                if (fact is W40KRTFeatFactItemModel) {
                    var feat = owner.Feats.OfType<W40KRTGameUnitFeatEntry>().FirstOrDefault(f => f.Model == fact);
                    if (feat != null) owner.Feats.Remove(feat);
                }
                else if (fact is W40KRTAbilityFactItemModel) {
                    var ability = owner.Abilities.OfType<W40KRTGameUnitAbilityEntry>().FirstOrDefault(f => f.Model == fact);
                    if (ability != null) owner.Abilities.Remove(ability);
                }
                else if (fact is W40KRTBuffFactItemModel) {
                    var buff = owner.Buffs.OfType<W40KRTGameUnitBuffEntry>().FirstOrDefault(f => f.Model == fact);
                    if (buff != null) owner.Buffs.Remove(buff);
                }
                else {
                    owner.Ref.Facts.Items.Remove(fact);
                }
            }
        }
    }
}

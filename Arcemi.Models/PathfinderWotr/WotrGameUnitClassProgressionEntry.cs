using System.Collections.Generic;
using System;
using System.Linq;
using System.Resources;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitClassProgressionEntry : IGameUnitClassProgressionEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        private readonly IGameUnitModel _unit;
        private readonly IReadOnlyList<IBlueprintMetadataEntry> _progressionBlueprints;

        public WotrGameUnitClassProgressionEntry(IGameUnitModel unit, ClassModel model)
        {
            _unit = unit;
            Model = model;

            _progressionBlueprints = _unit.Ref.Facts.Items
                .OfType<FeatureFactItemModel>()
                .Select(f => {
                    if (!string.Equals(Model.CharacterClass, f.Source, StringComparison.Ordinal)) return null;
                    if (!Res.Blueprints.TryGet(f.Blueprint, out var bp)) return null;
                    return bp.Type == WotrBlueprintProvider.Progression
                        ? bp
                        : null;
                })
                .Where(bp => bp != null)
                .ToArray();
        }

        public ClassModel Model { get; }

        public string Name => Res.GetClassTypeName(Model.CharacterClass);
        public string SpecializationName => Res.GetClassArchetypeName(Model.Archetypes);
        public int Level
        {
            get {
                return Model.Level;
            }
            set {
                if (value < 1 || value >= Model.Level) return;

                // Every time we level up (Excluding mythic classes) we level up the basic feat progression, the top selections in the game
                const string BasicFeatProgressionBlueprint = "5b72dd2ca2cb73b49903806ee8986325";
                var progression = _unit.Ref.Descriptor.Progression;
                var basicFeat = progression.Items.FirstOrDefault(item => item.Key.Eq(BasicFeatProgressionBlueprint))?.Value;
                if (basicFeat is null) {
                    Logger.Current.Warning("Missing basic feat progression, could not downlevel");
                    return;
                }

                // This is the actual class progression that we want to downlevel
                var act = progression.Items.FirstOrDefault(item => _progressionBlueprints.Any(b => b.Id.Eq(item.Key)));
                if (act is null) {
                    Logger.Current.Warning($"Missing progression {string.Join(", ", _progressionBlueprints.Select(b => b.Id))}, could not downlevel");
                    return;
                }

                // One level at a time
                for (var level = Model.Level; level > value; level--) {
                    var levelStr = level.ToString();
                    var basicFeatLevel = basicFeat.Level;
                    var basicFeatLevelStr = basicFeatLevel.ToString();

                    for (var i = progression.Selections.Count - 1; i >= 0; i--) {
                        var selection = progression.Selections[i];
                        // Only remove the basic feat progression if it's not a mythic class
                        if ((!Model.IsMythic && RemoveSelection(selection, BasicFeatProgressionBlueprint, basicFeatLevelStr, preserveFeats: true))
                            // Remove the class selection
                            || RemoveSelection(selection, act.Key, levelStr, preserveFeats: true)) {

                            if (selection.Value.ByLevel.Count == 0) {
                                // If we cleared all inner selections, then we can remove the entire selection
                                progression.Selections.RemoveAt(i);
                            }
                        }
                    }
                    if (!Model.IsMythic) {
                        // If it's not a mythic class then downlevel the basic feat level
                        basicFeat.Level--;
                    }
                }
                act.Value.Level = value;
                Model.Level = value;

                // Update the selection cache (All choices)
                ((WotrGameUnitProgressionModel)_unit.Progression).RefreshSelections();
            }
        }

        private bool RemoveSelection(ProgressionSelectionModel model, string progressionBlueprint, string levelStr, bool preserveFeats)
        {
            if (model.Value.Source.Blueprint.Eq(progressionBlueprint)) {
                if (model.Value.ByLevel.TryGetValue(levelStr, out var byLevel)) {
                    RemoveFeatures(byLevel, preserveFeats);
                    model.Value.ByLevel.Remove(levelStr);
                    return true;
                }
            }
            return false;
        }

        private void RemoveFeatures(ListValueAccessor<string> byLevel, bool preserveFeats)
        {
            if (preserveFeats) return;
            var hashset = new HashSet<string>(byLevel, StringComparer.Ordinal);
            for (var i = _unit.Ref.Facts.Items.Count - 1; i >= 0; i--) {
                var fact = _unit.Ref.Facts.Items[i];
                if (fact is FeatureFactItemModel feat && hashset.Contains(feat.Blueprint)) {
                    _unit.Ref.Facts.Items.RemoveAt(i);
                    hashset.Remove(feat.Blueprint);
                    if (hashset.Count == 0) break;
                }
            }
        }

        public bool IsReadOnly => false;
    }
}
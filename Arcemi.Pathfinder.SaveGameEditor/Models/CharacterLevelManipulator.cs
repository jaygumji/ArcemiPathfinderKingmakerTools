using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class CharacterLevelManipulator
    {
        public CharacterLevelManipulator(UnitEntityModel unit, IGameResourcesProvider resources)
        {
            Unit = unit;
            Resources = resources;
            if (unit == null) {
                return;
            }
            ProgressionBlueprints = unit.Descriptor.Progression.Classes.Select(cls => {
                var blueprints = (IReadOnlyList<IBlueprint>)unit.Facts.Items
                    .OfType<FeatureFactItemModel>()
                    .Select(f => {
                        if (!string.Equals(cls.CharacterClass, f.Source, StringComparison.Ordinal)) return null;
                        if (!resources.Blueprints.TryGet(f.Blueprint, out var bp)) return null;
                        return bp.Type == BlueprintTypes.Progression
                            ? bp
                            : null;
                    })
                    .Where(bp => bp != null)
                    .ToArray();
                return (cls.CharacterClass, blueprints);
            })
            .ToDictionary(x => x.CharacterClass, x => x.blueprints, StringComparer.Ordinal);
        }

        public UnitEntityModel Unit { get; }
        public IGameResourcesProvider Resources { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<IBlueprint>> ProgressionBlueprints { get; }

        public bool CanDowngrade(ClassModel cls)
        {
            return cls.Level > 1 && ProgressionBlueprints.ContainsKey(cls.CharacterClass);
        }

        public void DowngradeClass(ClassModel cls)
        {
            if (cls.Level <= 1) return;
            if (!ProgressionBlueprints.TryGetValue(cls.CharacterClass, out var blueprints)) return;

            var progression = Unit.Descriptor.Progression;

            // Some advanced classes is advancing base classes, so we find those progression blueprints as well
            var progressionLookup = new HashSet<string>(blueprints.Select(x => x.Id), StringComparer.Ordinal);
            var selectionProgressions = progression.Selections
                .Where(x => progressionLookup.Contains(x.Value.Source.Blueprint))
                .SelectMany(x => x.Value.ByLevel.Values.SelectMany(blList => blList.Select(v => Resources.Blueprints.TryGet(v, out var bp) ? bp : null)))
                .Where(x => x != null);
            foreach (var selectionProgression in selectionProgressions) {
                progressionLookup.Add(selectionProgression.Id);
            }

            var level = progression.CurrentLevel;
            var clsLevel = cls.Level;
            var clsBlueprints = cls.Archetypes?.Any() ?? false
                ? new HashSet<string>(cls.Archetypes, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            clsBlueprints.Add(cls.CharacterClass);

            foreach (var item in progression.Items) {
                if (item.Value.Level == level) {
                    item.Value.Level--;
                    continue;
                }
                if (!progressionLookup.Contains(item.Key)) continue;
                if (item.Value.Level == clsLevel) {
                    item.Value.Level--;
                }
            }

            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            for (var i = progression.Selections.Count - 1; i >= 0; i--) {
                var selection = progression.Selections[i];

                if (!cls.IsMythic && selection.Value.ByLevel.ContainsKey(levelStr)) {
                    selection.Value.ByLevel.Remove(levelStr);
                    if (selection.Value.ByLevel.Count == 0) {
                        progression.Selections.RemoveAt(i);
                    }
                    continue;
                }

                if (progressionLookup.Contains(selection.Value.Source.Blueprint)) {
                    if (selection.Value.ByLevel.ContainsKey(clsLevelStr)) {
                        selection.Value.ByLevel.Remove(clsLevelStr);
                        if (selection.Value.ByLevel.Count == 0) {
                            progression.Selections.RemoveAt(i);
                        }
                        continue;
                    }
                }
            }

            cls.Level = clsLevel - 1;
        }

    }
}

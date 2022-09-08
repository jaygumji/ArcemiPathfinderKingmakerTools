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

        public void DowngradeClass(ClassModel cls, bool preserveFeatures = false)
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

            // Decrease the level on the class
            foreach (var item in progression.Items) {
                // Lower the overall character level
                if (!cls.IsMythic)
                {
                    if (item.Value.Level == level)
                    {
                        item.Value.Level--;
                        continue;
                    }
                }

                // Lower the individual class level
                if (!progressionLookup.Contains(item.Key)) continue;
                if (item.Value.Level == clsLevel) {
                    item.Value.Level--;
                }
            }
            cls.Level = clsLevel - 1;

            // Remove things selected when leveling up to the level we're removing
            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            for (var i = progression.Selections.Count - 1; i >= 0; i--) {
                var selection = progression.Selections[i];

                // Remove class selections
                if (progressionLookup.Contains(selection.Value.Source.Blueprint))
                {
                    RemoveSelection(progression, i, clsLevelStr, preserveFeatures);
                }

                // Remove character selections
                if (!cls.IsMythic)
                {
                    RemoveSelection(progression, i, levelStr, preserveFeatures);
                }
            }
        }

        private void RemoveSelection(ProgressionModel progression, int i, string levelStr, bool preserveFeatures)
        {
            var selection = progression.Selections[i];

            if (selection.Value.ByLevel.ContainsKey(levelStr))
            {
                if (!preserveFeatures)
                {
                    RemoveClassFeatures(selection.Value.Source.Blueprint, int.Parse(levelStr));
                }

                selection.Value.ByLevel.Remove(levelStr);
                if (selection.Value.ByLevel.Count == 0)
                {
                    progression.Selections.RemoveAt(i);
                }
            }
        }

        private void RemoveClassFeatures(string classProgressionBlueprintId, int level)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact is FeatureFactItemModel feature
                && feature.Source == classProgressionBlueprintId
                && feature.SourceLevel == level).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }

        private void RemoveFeature(string blueprintId)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact.Blueprint == blueprintId).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
            Unit.Descriptor.UISettings.m_AlreadyAutomaticallyAdded.Remove(blueprintId);
            // To-do: remove from hotbar
        }

        private void RemoveFeature(FactItemModel fact)
        {
            foreach (var component in fact.Components)
            {
                if (component.Value is AddFactsComponentModel addFactsComponent)
                {
                    foreach (var addedFact in addFactsComponent.Data.AppliedFacts)
                    {
                        RemoveFeature(addedFact.Blueprint);
                    }
                }
            }

            Unit.Facts.Items.Remove(fact);
        }
    }
}

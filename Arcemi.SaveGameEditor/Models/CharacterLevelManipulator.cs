using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.SaveGameEditor.Models
{
    public class CharacterLevelManipulator
    {
        public CharacterLevelManipulator(IGameUnitModel unit, IGameResourcesProvider resources)
        {
            Unit = unit.Ref;
            Resources = resources;
            if (unit is null || unit is not WotrGameUnitModel) {
                return;
            }
            ProgressionBlueprints = Unit.Descriptor.Progression.Classes.Select(cls => {
                var blueprints = (IReadOnlyList<IBlueprintMetadataEntry>)Unit.Facts.Items
                    .OfType<FeatureFactItemModel>()
                    .Select(f => {
                        if (!string.Equals(cls.CharacterClass, f.Source, StringComparison.Ordinal)) return null;
                        if (!resources.Blueprints.TryGet(f.Blueprint, out var bp)) return null;
                        return bp.Type == WotrBlueprintProvider.Progression
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
        public IReadOnlyDictionary<string, IReadOnlyList<IBlueprintMetadataEntry>> ProgressionBlueprints { get; }

        public bool CanDowngrade(IGameUnitClassProgressionEntry cls)
        {
            if (cls is not WotrGameUnitClassProgressionEntry wotr) return false;
            if (wotr.Model.IsMythicChampion) return false;
            return cls.Level > 1 && ProgressionBlueprints.ContainsKey(wotr.Model.CharacterClass);
        }

        public void DowngradeClass(IGameUnitClassProgressionEntry cls, bool preserveFeatures = true)
        {
            if (cls is not WotrGameUnitClassProgressionEntry wotr) return;
            if (cls.Level <= 1) return;
            if (!ProgressionBlueprints.TryGetValue(wotr.Model.CharacterClass, out var blueprints)) return;

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
            var clsBlueprints = wotr.Model.Archetypes?.Any() ?? false
                ? new HashSet<string>(wotr.Model.Archetypes, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            clsBlueprints.Add(wotr.Model.CharacterClass);

            // Decrease the level on the class
            foreach (var item in progression.Items) {
                // Lower the overall character level
                if (!wotr.Model.IsMythic)
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
            wotr.Model.Level = clsLevel - 1;

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
                if (!wotr.Model.IsMythic)
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

        private void RemoveFeatureByBlueprint(string blueprintId)
        {
            var toRemove = Unit.Facts.Items.Where(fact => fact.Blueprint == blueprintId).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }

        private void RemoveFeatureById(string id)
        {
            var toRemove = Unit.Facts.Items.Where(f => f.Id == id).ToList();
            foreach (var fact in toRemove)
            {
                RemoveFeature(fact);
            }
        }

        private void RemoveFeature(FactItemModel fact)
        {
            // Remove facts added by this fact
            foreach (var component in fact.Components)
            {
                if (component.Value is AddFactsComponentModel addFactsComponent)
                {
                    foreach (var addedFact in addFactsComponent.Data.AppliedFacts)
                    {
                        if (addedFact is ActivatableAbilityFactItemModel activatableAbilityFact)
                        {
                            RemoveFeatureById(activatableAbilityFact.m_AppliedBuff.Id);
                        }

                        RemoveFeatureByBlueprint(addedFact.Blueprint);
                    }
                }
            }

            // Remove the fact itself
            Unit.Facts.Items.Remove(fact);
            Unit.Descriptor.UISettings.m_AlreadyAutomaticallyAdded.Remove(fact.Blueprint);
            // To-do: remove from hotbar. Might be optional.

            // To-do: reference class progression and re-add any features this one replaced during level-up
        }
    }
}

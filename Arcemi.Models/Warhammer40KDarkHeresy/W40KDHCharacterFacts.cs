using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHFeatureEntry : IGameUnitFeatEntry
    {
        private readonly W40KDHParsedProgressionSelection _fact;
        private readonly W40KDHAdvancementStat _advancement;

        public W40KDHFeatureEntry(W40KDHParsedProgressionSelection fact, string displayName, W40KDHAdvancementStat advancement)
        {
            _fact = fact;
            _advancement = advancement;
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public string Blueprint => _fact.FeatureBlueprint;
        public string Tooltip => Blueprint;
        public string Category => string.Empty;
        public bool IsRanked => true;
        public bool CanEditRank => _advancement?.IsReadOnly == false;
        public int? MaximumRank => _advancement?.Maximum / _advancement?.Step;
        public bool CanExport => false;
        public int Rank
        {
            get => _advancement?.Rank ?? _fact.Rank;
            set {
                if (!CanEditRank) throw new NotSupportedException();
                _advancement.Value = checked(value * _advancement.Step);
            }
        }
        public int SourceLevel => 0;
        public string Export() => throw new NotSupportedException();
    }

    internal sealed record W40KDHAbilityEntry(string DisplayName, string Blueprint, bool IsActive) : IGameUnitAbilityEntry
    {
        bool IGameUnitAbilityEntry.IsActive { get => IsActive; set => throw new NotSupportedException(); }
        public bool IsReadOnly => true;
    }

    internal sealed record W40KDHBuffEntry(string DisplayName, string Blueprint, bool IsActive) : IGameUnitBuffEntry
    {
        bool IGameUnitBuffEntry.IsActive { get => IsActive; set => throw new NotSupportedException(); }
        public bool IsReadOnly => true;
        public TimeParts Duration => null;
    }

    internal sealed class W40KDHProgressionModel : IGameUnitProgressionModel
    {
        private readonly W40KDHParsedUnitStats _unit;

        public W40KDHProgressionModel(W40KDHParsedUnitStats unit, BlueprintProvider blueprints)
        {
            _unit = unit;
            var selections = unit.ProgressionSelections.Where(selection => !selection.IsFeatureFact && selection.FeatureBlueprint.HasValue())
                .Select(selection => (IGameUnitSelectionProgressionEntry)new Selection(
                    blueprints.GetNameOrBlueprint(selection.FeatureBlueprint), selection.FeatureBlueprint, selection.Level ?? 0))
                .ToArray();

            Selections = new GameModelCollection<IGameUnitSelectionProgressionEntry>(
                selections, GameModelCollectionWriter<IGameUnitSelectionProgressionEntry>.ReadOnly);
            var rows = unit.ProgressionSelections.Where(selection => !selection.IsFeatureFact && selection.FeatureBlueprint.HasValue())
                .Select(selection => GameDataModels.Integer(
                    blueprints.GetNameOrBlueprint(selection.FeatureBlueprint), selection, value => value.Rank)).ToArray();
            Data = rows.Length > 0 ? GameDataModels.Object("Progression selections", rows) : null;
        }

        public bool IsSupported => _unit.Experience.HasValue && _unit.CharacterLevel.HasValue;
        public int Experience { get => _unit.Experience.GetValueOrDefault(); set => throw new NotSupportedException(); }
        public int CurrentLevel { get => _unit.CharacterLevel.GetValueOrDefault(); set => throw new NotSupportedException(); }
        public bool IsLevelReadOnly => true;
        public bool IsExperienceReadOnly => true;
        public bool IsSelectionsRepairable => false;
        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; }
        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();
        public IGameDataObject Data { get; }
        public void RepairSelections() => throw new NotSupportedException();

        private sealed record Selection(string Name, string Feature, int Level) : IGameUnitSelectionProgressionEntry
        {
            string IGameUnitSelectionProgressionEntry.Feature { get => Feature; set => throw new NotSupportedException(); }
            public bool IsCreation => false;
            public bool IsReadOnly => true;
            public IReadOnlyList<BlueprintOption> Options => Array.Empty<BlueprintOption>();
        }
    }
}

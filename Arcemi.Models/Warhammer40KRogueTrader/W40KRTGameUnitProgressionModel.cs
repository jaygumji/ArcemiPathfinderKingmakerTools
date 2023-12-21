using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitProgressionModel : IGameUnitProgressionModel
    {
        public W40KRTGameUnitProgressionModel(IGameUnitModel owner, UnitProgressionPartItemModel model)
        {
            Owner = owner;
            Model = model;
            if (model?.Selections is null) return;

            var classes = new List<IGameUnitClassProgressionEntry>();
            Classes = classes;
            Selections = model.Selections.Where(s => s.Level == 0).Select(s => new W40KRTGameUnitSelectionProgressionEntry(owner, s)).Where(s => !s.Name.IStart("Stat Advancement")).ToArray();
        }

        public IGameUnitModel Owner { get; }
        public UnitProgressionPartItemModel Model { get; }

        public int Experience { get => Model.Experience; set => Model.Experience = value; }
        public int CurrentLevel
        {
            get => Model.CharacterLevel;
            set {
                var oldLevel = Model.CharacterLevel;
                Model.CharacterLevel = value;
                var oldSelections = Model.Selections.Where(x => W40KRTArchetypes.ActualLevel(x.Path, x.Level) > value).ToArray();
                var featIds = new HashSet<string>(oldSelections.Select(x => x.Feature), StringComparer.Ordinal);
                var feats = Owner.Feats.Where(f => featIds.Contains(f.Blueprint)).ToArray();

                foreach (var selection in oldSelections) {
                    Model.Selections.Remove(selection);
                }
                foreach (var feat in feats) {
                    Owner.Feats.Remove(feat);
                }
            }
        }
        public bool IsLevelReadOnly => false;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IReadOnlyList<IGameUnitSelectionProgressionEntry> Selections { get; } = Array.Empty<IGameUnitSelectionProgressionEntry>();
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();

        public bool IsSupported => Model is object;
    }
}
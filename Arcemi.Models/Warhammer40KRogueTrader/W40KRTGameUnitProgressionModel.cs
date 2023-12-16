using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitProgressionModel : IGameUnitProgressionModel
    {
        public W40KRTGameUnitProgressionModel(UnitProgressionPartItemModel model)
        {
            Model = model;
            if (model?.Selections is null) return;

            var classes = new List<IGameUnitClassProgressionEntry>();
            Classes = classes;
            Selections = model.Selections.Where(s => s.Level == 0).Select(s => new W40KRTGameUnitSelectionProgressionEntry(s)).Where(s => !s.Name.IStart("Stat Advancement")).ToArray();
        }

        public UnitProgressionPartItemModel Model { get; }

        public int Experience { get => Model.Experience; set => Model.Experience = value; }
        public int CurrentLevel { get => Model.CharacterLevel; set => Model.CharacterLevel = value; }
        public bool IsLevelReadOnly => false;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IReadOnlyList<IGameUnitSelectionProgressionEntry> Selections { get; } = Array.Empty<IGameUnitSelectionProgressionEntry>();
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();

        public bool IsSupported => Model is object;
    }
}
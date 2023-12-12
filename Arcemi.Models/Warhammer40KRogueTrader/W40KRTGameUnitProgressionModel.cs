using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitProgressionModel : IGameUnitProgressionModel
    {
        public W40KRTGameUnitProgressionModel(UnitProgressionPartItemModel model)
        {
            Model = model;
        }

        public UnitProgressionPartItemModel Model { get; }

        public int Experience { get => Model.Experience; set => Model.Experience = value; }

        public int CurrentLevel => Model.CharacterLevel;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();

        public bool IsSupported => Model is object;
    }
}
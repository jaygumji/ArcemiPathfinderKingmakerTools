using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitProgressionModel : IGameUnitProgressionModel
    {
        public WotrGameUnitProgressionModel(UnitEntityModel unit)
        {
            Unit = unit;
            Ultimates = new IGameUnitUltimateProgressionEntry[] {
                new WotrGameUnitProgressionMythicModel(unit)
            };
            Classes = unit.Descriptor.Progression.Classes.Select(c => new WotrGameUnitClassProgressionEntry(c)).ToArray();
        }

        public UnitEntityModel Unit { get; }
        public int Experience { get => Unit.Descriptor.Progression.Experience; set => Unit.Descriptor.Progression.Experience = value; }

        public int CurrentLevel => Unit.Descriptor.Progression.CurrentLevel;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; }
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; }

        public bool IsSupported => true;
    }
}
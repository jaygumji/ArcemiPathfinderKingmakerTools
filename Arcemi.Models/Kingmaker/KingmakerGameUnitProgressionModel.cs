using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitProgressionModel : IGameUnitProgressionModel
    {
        public KingmakerGameUnitProgressionModel(UnitEntityModel unit)
        {
            Unit = unit;
            Ultimates = Array.Empty<IGameUnitUltimateProgressionEntry>();
            Classes = unit.Descriptor.Progression.Classes.Select(c => new KingmakerGameUnitClassProgressionEntry(c)).ToArray();
        }

        public UnitEntityModel Unit { get; }
        public int Experience { get => Unit.Descriptor.Progression.Experience; set => Unit.Descriptor.Progression.Experience = value; }

        public int CurrentLevel {
            get => Unit.Descriptor.Progression.GetAccessor().Value<int>("CharacterLevel");
            set => Unit.Descriptor.Progression.GetAccessor().Value(value, "CharacterLevel"); }
        public bool IsLevelReadOnly => false;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; }
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; }
        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; } = GameModelCollection<IGameUnitSelectionProgressionEntry>.Empty;
        public IGameDataObject Data { get; }

        public bool IsSupported => true;

        public bool IsSelectionsRepairable => false;

        public void RepairSelections()
        {
        }
    }
}
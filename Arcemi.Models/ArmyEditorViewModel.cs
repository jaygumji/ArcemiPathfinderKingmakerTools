using System;
using System.Collections.Generic;
using System.Linq;
using Arcemi.Models.PathfinderWotr;

namespace Arcemi.Models
{
    public class ArmyEditorUnitViewModel
    {
        private readonly IGameResourcesProvider _res;

        public ArmyEditorUnitViewModel(IGameResourcesProvider res, PlayerArmySquadModel squad, ArmyUnitDataMapping mapping, int positionIndex)
        {
            _res = res;
            Squad = squad;
            Mapping = mapping;
            PositionIndex = positionIndex;
        }

        public PlayerArmySquadModel Squad { get; }
        public ArmyUnitDataMapping Mapping { get; }
        public int PositionIndex { get; }
        public string Name => Mapping.Name.OrIfEmpty(_res.GetArmyUnitName(Squad.Unit));
    }

    public class ArmyEditorViewModel
    {
        private readonly List<ArmyEditorUnitViewModel> _units;
        private readonly ArmyEditorUnitViewModel[][] _defaultSizePosition;
        private readonly ArmyEditorUnitViewModel[] _largeSizePosition;
        private readonly IGameResourcesProvider _res;

        public ArmyEditorViewModel(IGameResourcesProvider res, PlayerArmyModel army)
        {
            _res = res;
            Army = army;
            _units = army.Data.Squads
                .Select(squad => new ArmyEditorUnitViewModel(res, squad, GetMapping(res, army, squad), army.Data.SquadsPosition.IndexOf(squad)))
                .ToList();

            _defaultSizePosition = new ArmyEditorUnitViewModel[2][];
            _defaultSizePosition[0] = new ArmyEditorUnitViewModel[7];
            _defaultSizePosition[1] = new ArmyEditorUnitViewModel[7];
            _largeSizePosition = new ArmyEditorUnitViewModel[6];
            foreach (var unit in _units) {
                if (unit.Mapping.Size == ArmyUnitSize.Large) {
                    _largeSizePosition[unit.PositionIndex] = unit;
                }
                else {
                    var row = unit.PositionIndex / 7;
                    var col = unit.PositionIndex % 7;
                    _defaultSizePosition[row][col] = unit;
                }
            }
        }

        public PlayerArmyModel Army { get; }
        public IReadOnlyList<ArmyEditorUnitViewModel> Units => _units;
        public IReadOnlyList<IReadOnlyList<ArmyEditorUnitViewModel>> DefaultSizePosition => _defaultSizePosition;
        public IReadOnlyList<ArmyEditorUnitViewModel> LargeSizePosition => _largeSizePosition;

        public bool IsPositionAvailable(int pos, ArmyUnitSize size)
        {
            if (size == ArmyUnitSize.Large) {
                return LargeSizePosition[pos] == null
                    && (pos == 0 || LargeSizePosition[pos - 1] == null)
                    && (pos == 5 || LargeSizePosition[pos + 1] == null)
                    && DefaultSizePosition[0][pos] == null
                    && DefaultSizePosition[0][pos + 1] == null
                    && DefaultSizePosition[1][pos] == null
                    && DefaultSizePosition[1][pos + 1] == null;
            }
            var row = pos / 7;
            var col = pos % 7;
            return DefaultSizePosition[row][col] == null
                && (col == 6 || LargeSizePosition[col] == null)
                && (col == 0 || LargeSizePosition[col - 1] == null);
        }

        public int FindFirstAvailablePosition(ArmyUnitSize size)
        {
            if (size == ArmyUnitSize.Large) {
                for (var i = 0; i < 6; i++) {
                    if (IsPositionAvailable(i, size)) return i;
                }
                return -1;
            }
            for (var i = 0; i < 7; i++) {
                if (IsPositionAvailable(i + 7, size)) return i + 7;
                if (IsPositionAvailable(i, size)) return i;
            }
            return -1;
        }

        public bool AddArmyUnit(string blueprint, ArmyUnitSize size, int count)
        {
            var emptyIndex = FindFirstAvailablePosition(size);
            if (emptyIndex < 0) return false;

            var squad = Army.Data.Squads.Add();
            squad.Guid = Guid.NewGuid().ToString();
            squad.Count = count;
            squad.Unit = blueprint;

            if (size == ArmyUnitSize.Large) {
                Army.Data.SquadsPosition.SetRef(emptyIndex, squad);
                Army.Data.SquadsPosition.SetRef(emptyIndex + 1, squad);
                Army.Data.SquadsPosition.SetRef(emptyIndex + 7, squad);
                Army.Data.SquadsPosition.SetRef(emptyIndex + 8, squad);
            }
            else {
                Army.Data.SquadsPosition.SetRef(emptyIndex, squad);
            }

            var vm = new ArmyEditorUnitViewModel(_res, squad, GetMapping(_res, Army, squad), emptyIndex);
            if (size == ArmyUnitSize.Large) {
                _largeSizePosition[emptyIndex] = vm;
            }
            else {
                var row = emptyIndex / 7;
                var col = emptyIndex % 7;
                _defaultSizePosition[row][col] = vm;
            }

            _units.Add(vm);
            return true;
        }

        public void RemoveArmyUnit(ArmyEditorUnitViewModel armyUnit)
        {
            var pos = armyUnit.PositionIndex;
            var size = FindSizeFromPosition(Army, armyUnit.Squad);
            Army.Data.Squads.Remove(armyUnit.Squad);
            if (size == ArmyUnitSize.Large) {
                Army.Data.SquadsPosition[pos] = null;
                Army.Data.SquadsPosition[pos + 1] = null;
                Army.Data.SquadsPosition[pos + 7] = null;
                Army.Data.SquadsPosition[pos + 8] = null;
                _largeSizePosition[pos] = null;
            }
            else {
                Army.Data.SquadsPosition[armyUnit.PositionIndex] = null;
                var row = pos / 7;
                var col = pos % 7;
                _defaultSizePosition[row][col] = null;
            }
            _units.Remove(armyUnit);
        }

        private ArmyUnitDataMapping GetMapping(IGameResourcesProvider res, PlayerArmyModel army, PlayerArmySquadModel squad)
        {
            if (_res.Mappings.ArmyUnits.TryGetValue(squad.Unit, out var m)) {
                if (m.Size == ArmyUnitSize.Unknown) {
                    var detectedSize = FindSizeFromPosition(army, squad);
                    return new ArmyUnitDataMapping { Id = m.Id, Name = m.Name, Size = detectedSize };
                }
                return m;
            }
            var size = FindSizeFromPosition(army, squad);
            if (res.Blueprints.TryGet(squad.Unit, out var bp)) {
                return new ArmyUnitDataMapping {
                    Id = bp.Id,
                    Name = bp.DisplayName,
                    Size = size
                };
            }

            return new ArmyUnitDataMapping {
                Id = squad.Unit,
                Name = squad.Unit,
                Size = size
            };
        }

        private static ArmyUnitSize FindSizeFromPosition(PlayerArmyModel army, PlayerArmySquadModel squad)
        {
            var count = army.Data.SquadsPosition.Count(x => x == squad);
            if (count == 4) return ArmyUnitSize.Large;
            if (count == 1) return ArmyUnitSize.Default;
            return ArmyUnitSize.Unknown;
        }
    }
}

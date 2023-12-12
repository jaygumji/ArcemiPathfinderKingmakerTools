using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitCompanionModel : IGameUnitCompanionModel
    {
        public WotrGameUnitCompanionModel(UnitEntityModel unit)
        {
            Unit = unit;
            Part = Unit?.Parts?.Items?.OfType<CompanionPartItemModel>()?.FirstOrDefault();
            AllStates = CompanionPartState.All.Select(x => new GameEnumValue(x.Key, x.Value)).ToArray();
        }

        public UnitEntityModel Unit { get; }
        public CompanionPartItemModel Part { get; }

        public IReadOnlyList<GameEnumValue> AllStates { get; }
        public string State { get => Part.State; set => Part.State = value; }

        public bool IsSupported => Part is object;
    }
}
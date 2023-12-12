using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitCompanionModel : IGameUnitCompanionModel
    {
        public W40KRTGameUnitCompanionModel(UnitCompanionPartItemModel model)
        {
            Model = model;
            AllStates = CompanionPartState.All.Select(x => new GameEnumValue(x.Key, x.Value)).ToArray();
        }

        public UnitCompanionPartItemModel Model { get; }
        public IReadOnlyList<GameEnumValue> AllStates { get; }
        public string State { get => Model.State; set => Model.State = value; }
        public bool IsSupported => Model is object;
    }
}
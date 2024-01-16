using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitCompanionModel : IGameUnitCompanionModel
    {
        public W40KRTGameUnitCompanionModel(IGameUnitModel owner, UnitCompanionPartItemModel model)
        {
            Owner = owner;
            Model = model;
            AllStates = CompanionPartState.All.Select(x => new GameEnumValue(x.Value, x.Key)).ToArray();
        }

        public IGameUnitModel Owner { get; }
        public UnitCompanionPartItemModel Model { get; }
        public IReadOnlyList<GameEnumValue> AllStates { get; }
        public string State { get => Model.State; set => Model.State = value; }
        public bool IsSupported => Model is object;
        public bool IsReadOnly => true;
        public bool IsDialogEnabled
        {
            get {
                switch (Owner.Type) {
                    case UnitEntityType.Player:
                    case UnitEntityType.Starship:
                        return false;
                }
                return true;
            }
        }

        public bool IsInParty => State.Eq(CompanionPartState.InParty);
        public bool IsExCompanion => State.Eq(CompanionPartState.ExCompanion);
    }
}
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
        public string State
        {
            get => Model.State;
            set {
                Model.State = value;
                if (!value.Eq("ExCompanion")) {
                    var combatGroup = Owner.Ref.Parts.Container.OfType<CombatGroupPartItemModel>().FirstOrDefault();
                    const string controllableId = "<directly-controllable-unit>";
                    if (combatGroup is object && !combatGroup.ControlId.Eq(controllableId)) {
                        combatGroup.ControlId = controllableId;
                    }
                }
            }
        }
        public bool IsSupported => Model is object;
        public bool IsReadOnly
        {
            get {
                switch (Owner.Type) {
                    case UnitEntityType.Player:
                    case UnitEntityType.Starship:
                        return true;
                }
                return false;
            }
        }
    }
}
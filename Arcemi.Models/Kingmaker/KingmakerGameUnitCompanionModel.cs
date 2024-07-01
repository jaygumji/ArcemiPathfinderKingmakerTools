using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitCompanionModel : IGameUnitCompanionModel
    {
        public KingmakerGameUnitCompanionModel(IGameUnitModel owner, UnitEntityModel unit)
        {
            Owner = owner;
            Unit = unit;
            Part = Unit?.Parts?.Items?.OfType<CompanionPartItemModel>()?.FirstOrDefault();
            AllStates = CompanionPartState.All.Select(x => new GameEnumValue(x.Value, x.Key)).ToArray();
        }

        public IGameUnitModel Owner { get; }
        public UnitEntityModel Unit { get; }
        public CompanionPartItemModel Part { get; }

        public IReadOnlyList<GameEnumValue> AllStates { get; }
        public string State { get => Part?.State; set => Part.State = value; }

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
        public bool IsDialogEnabled => false;
        public bool IsInParty => true; // State.Eq(CompanionPartState.InParty);
        public bool IsExCompanion => State.Eq(CompanionPartState.ExCompanion);

        public bool IsSupported => Part is object;
    }
}
using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHGameUnitBodyModel : IGameUnitBodyModel
    {
        internal static IReadOnlyList<string> BodySlotFields { get; } = new[] {
            nameof(Armor), nameof(Shirt), nameof(Belt), nameof(Head), nameof(Feet), nameof(Gloves),
            nameof(Neck), nameof(Glasses), nameof(Ring1), nameof(Ring2), nameof(Wrist), nameof(Shoulders)
        };

        internal static W40KDHGameUnitBodyModel Empty { get; } = new(null,
            Array.Empty<IGameUnitHandsEquippedEntry>(), Array.Empty<IGameUnitEquippedEntry>(), null);

        private readonly IReadOnlyDictionary<string, IGameUnitEquippedEntry> _bodySlots;

        public W40KDHGameUnitBodyModel(IReadOnlyDictionary<string, IGameUnitEquippedEntry> bodySlots,
            IReadOnlyList<IGameUnitHandsEquippedEntry> hands, IReadOnlyList<IGameUnitEquippedEntry> quickSlots, int? activeHandsIndex)
        {
            _bodySlots = bodySlots;
            HandsEquipmentSets = hands;
            QuickSlots = quickSlots;
            ActiveHandsEquipmentSetIndex = activeHandsIndex is int index && index >= 0 && index < hands.Count ? index : null;
        }

        public bool IsSupported => _bodySlots is not null;
        public IReadOnlyList<IGameUnitHandsEquippedEntry> HandsEquipmentSets { get; }
        public IReadOnlyList<IGameUnitEquippedEntry> QuickSlots { get; }
        public int? ActiveHandsEquipmentSetIndex { get; }
        public IGameUnitEquippedEntry Armor => GetSlot(nameof(Armor));
        public IGameUnitEquippedEntry Shirt => GetSlot(nameof(Shirt));
        public IGameUnitEquippedEntry Belt => GetSlot(nameof(Belt));
        public IGameUnitEquippedEntry Head => GetSlot(nameof(Head));
        public IGameUnitEquippedEntry Feet => GetSlot(nameof(Feet));
        public IGameUnitEquippedEntry Gloves => GetSlot(nameof(Gloves));
        public IGameUnitEquippedEntry Neck => GetSlot(nameof(Neck));
        public IGameUnitEquippedEntry Glasses => GetSlot(nameof(Glasses));
        public IGameUnitEquippedEntry Ring1 => GetSlot(nameof(Ring1));
        public IGameUnitEquippedEntry Ring2 => GetSlot(nameof(Ring2));
        public IGameUnitEquippedEntry Wrist => GetSlot(nameof(Wrist));
        public IGameUnitEquippedEntry Shoulders => GetSlot(nameof(Shoulders));

        private IGameUnitEquippedEntry GetSlot(string name) => _bodySlots?.GetValueOrDefault(name);
    }

    internal sealed record W40KDHGameUnitHandsEquippedEntry(IGameUnitEquippedEntry Primary, IGameUnitEquippedEntry Secondary)
        : IGameUnitHandsEquippedEntry;

    internal sealed record W40KDHGameUnitEquippedEntry(string ItemId) : IGameUnitEquippedEntry
    {
        public object ItemRef => ItemId;
        public bool IsReadOnly => true;
    }
}

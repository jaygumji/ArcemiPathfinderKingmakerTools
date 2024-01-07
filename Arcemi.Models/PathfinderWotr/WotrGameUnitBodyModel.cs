using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitBodyModel : IGameUnitBodyModel
    {
        public WotrGameUnitBodyModel(UnitEntityModel unit)
        {
            Ref = unit;
            if (unit.Descriptor?.Body is null) return;
            HandsEquipmentSets = unit.Descriptor.Body.HandsEquipmentSets.Select(x => new WotrGameUnitHandsEquippedEntry(x)).ToArray();
            QuickSlots = unit.Descriptor.Body.QuickSlots;
            Armor = unit.Descriptor.Body.Armor;
            Shirt = unit.Descriptor.Body.Shirt;
            Belt = unit.Descriptor.Body.Belt;
            Head = unit.Descriptor.Body.Head;
            Feet = unit.Descriptor.Body.Feet;
            Gloves = unit.Descriptor.Body.Gloves;
            Neck = unit.Descriptor.Body.Neck;
            Glasses = unit.Descriptor.Body.Glasses;
            Ring1 = unit.Descriptor.Body.Ring1;
            Ring2 = unit.Descriptor.Body.Ring2;
            Wrist = unit.Descriptor.Body.Wrist;
            Shoulders = unit.Descriptor.Body.Shoulders;
        }
        public IReadOnlyList<IGameUnitHandsEquippedEntry> HandsEquipmentSets { get; }
        public IReadOnlyList<IGameUnitEquippedEntry> QuickSlots { get; }
        public IGameUnitEquippedEntry Armor { get; }
        public IGameUnitEquippedEntry Shirt { get; }
        public IGameUnitEquippedEntry Belt { get; }
        public IGameUnitEquippedEntry Head { get; }
        public IGameUnitEquippedEntry Feet { get; }
        public IGameUnitEquippedEntry Gloves { get; }
        public IGameUnitEquippedEntry Neck { get; }
        public IGameUnitEquippedEntry Glasses { get; }
        public IGameUnitEquippedEntry Ring1 { get; }
        public IGameUnitEquippedEntry Ring2 { get; }
        public IGameUnitEquippedEntry Wrist { get; }
        public IGameUnitEquippedEntry Shoulders { get; }
        public bool IsSupported => Ref is object;
        public UnitEntityModel Ref { get; }
    }
}
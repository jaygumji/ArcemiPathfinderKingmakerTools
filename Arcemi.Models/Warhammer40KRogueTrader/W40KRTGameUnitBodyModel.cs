using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitBodyModel : IGameUnitBodyModel
    {
        public W40KRTGameUnitBodyModel(IGameUnitModel owner, UnitBodyPartItemModel body)
        {
            if (owner.Type == UnitEntityType.Starship) return;
            Ref = body;
            if (body is null) return;
            HandsEquipmentSets = body.HandsEquipmentSets.Select(x => new W40KRTGameUnitHandsEquippedEntry(x)).ToArray();
            QuickSlots = body.QuickSlots;
            Armor = body.Armor;
            Shirt = body.Shirt;
            Belt = body.Belt;
            Head = body.Head;
            Feet = body.Feet;
            Gloves = body.Gloves;
            Neck = body.Neck;
            Glasses = body.Glasses;
            Ring1 = body.Ring1;
            Ring2 = body.Ring2;
            Wrist = body.Wrist;
            Shoulders = body.Shoulders;
        }
        public IReadOnlyList<IGameUnitHandsEquippedEntry> HandsEquipmentSets { get; }
        public IReadOnlyList<IGameUnitEquippedEntry> QuickSlots { get; }
        public IEnumerable<(string Name, IGameUnitEquippedEntry Slot, BlueprintTypeId ItemType)> AdditionalSlots
        {
            get {
                var slots = Ref?.GetAccessor().Object<RefModel>("Augments")?.GetAccessor().List<RefModel>("m_Slots");
                if (slots is null) yield break;
                var blueprints = GameDefinition.Warhammer40K_RogueTrader.Resources.Blueprints;
                foreach (var entry in slots) {
                    var accessor = entry.GetAccessor();
                    var slot = accessor.Object<RefModel>("Value");
                    if (slot is null) continue;
                    var key = accessor.Value<string>("Key");
                    var name = W40KRTForgeWorldAugments.Slot.Eq(key) ? "Forge World" : blueprints.GetNameOrBlueprint(key);
                    yield return (name, new AugmentSlotEntry(slot), BlueprintTypeId.ItemAugment);
                }
            }
        }

        private sealed class AugmentSlotEntry : IGameUnitEquippedEntry
        {
            private readonly RefModel _slot;

            public AugmentSlotEntry(RefModel slot) => _slot = slot;

            public object ItemRef => _slot.GetAccessor().Value<string>("m_ItemRef");
            public bool IsReadOnly => true;
        }

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
        public UnitBodyPartItemModel Ref { get; }
    }
}
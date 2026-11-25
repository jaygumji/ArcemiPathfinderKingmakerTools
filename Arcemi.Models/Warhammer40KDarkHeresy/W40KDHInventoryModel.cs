using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHInventoryModel : IGameInventoryModel, IGameItemSection
    {
        private readonly InventoryItem[] _items;
        private readonly IReadOnlyDictionary<string, IGameItemEntry> _itemsById;

        public W40KDHInventoryModel(string name, W40KDHInventoryData data, IGameResourcesProvider resources, bool writable)
        {
            Name = name;
            IsSupported = data is not null;
            Sections = IsSupported ? new IGameItemSection[] { this } : Array.Empty<IGameItemSection>();
            _items = data is null ? Array.Empty<InventoryItem>() : CreateItems(data, resources, writable);
            _itemsById = _items.Where(item => item.UniqueId.HasValue()).GroupBy(item => item.UniqueId, StringComparer.Ordinal)
                .Where(group => group.Count() == 1)
                .ToDictionary(group => group.Key, group => (IGameItemEntry)group.Single(), StringComparer.Ordinal);
            Items = IsSupported
                ? new GameModelCollection<IGameItemEntry>(_items, GameModelCollectionWriter<IGameItemEntry>.ReadOnly)
                : GameModelCollection<IGameItemEntry>.Empty;
        }

        public string Name { get; }
        public bool IsSupported { get; }
        public IReadOnlyList<IGameItemSection> Sections { get; }
        public IGameModelCollection<IGameItemEntry> Items { get; }
        public IReadOnlyList<BlueprintType> AddableTypes { get; } = Array.Empty<BlueprintType>();
        internal bool CanEdit => _items.Any(item => item.CanEditQuantity || item.CanEditCharges);
        internal bool HasChanges => _items.Any(item => item.HasChanges);

        public IGameItemEntry FindEquipped(object itemRef) => itemRef is string id ? _itemsById.GetValueOrDefault(id) : null;
        public IEnumerable<IBlueprintMetadataEntry> GetAddableItems(string typeFullName) => Array.Empty<IBlueprintMetadataEntry>();

        internal void WriteTo(byte[] entry)
        {
            foreach (var item in _items) item.WriteTo(entry);
        }

        internal void AcceptChanges()
        {
            foreach (var item in _items) item.AcceptChanges();
        }

        private static InventoryItem[] CreateItems(W40KDHInventoryData data, IGameResourcesProvider resources, bool writable)
        {
            // A saved stack establishes stackability in this collection, whose rules can override the blueprint.
            var stackedBlueprints = data.Items.Where(item => item.Count > 1 && item.Blueprint.HasValue())
                .Select(item => item.Blueprint).ToHashSet(StringComparer.OrdinalIgnoreCase);
            // OWL stores a Charges field even for items that never spend charges.
            var chargedBlueprints = data.Items.Where(item => item.Charges > 0 && item.Blueprint.HasValue())
                .Select(item => item.Blueprint).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var sharedOffsets = data.Items.SelectMany(item => new[] { item.CountOffset, item.ChargesOffset })
                .Where(offset => offset.HasValue).GroupBy(offset => offset.Value)
                .Where(group => group.Count() > 1).Select(group => group.Key).ToHashSet();

            return data.Items.Select((item, index) => {
                var stackable = data.ForceStackable || stackedBlueprints.Contains(item.Blueprint);
                var chargeable = item.Charges.HasValue && (item.Charges > 0 || chargedBlueprints.Contains(item.Blueprint));
                var canEditQuantity = writable && stackable && item.Count > 0
                    && item.CountOffset is int countOffset && !sharedOffsets.Contains(countOffset);
                var canEditCharges = writable && chargeable && item.Count > 0 && item.Charges >= 0
                    && item.ChargesOffset is int chargesOffset && !sharedOffsets.Contains(chargesOffset);
                return new InventoryItem(item, index, resources, stackable, chargeable, canEditQuantity, canEditCharges);
            }).ToArray();
        }

        private sealed class InventoryItem : IGameItemEntry
        {
            private readonly W40KDHItem _item;
            private int _count;
            private int _savedCount;
            private int _charges;
            private int _savedCharges;

            public InventoryItem(W40KDHItem item, int index, IGameResourcesProvider resources, bool stackable, bool chargeable,
                bool canEditQuantity, bool canEditCharges)
            {
                _item = item;
                Index = index;
                const string itemEntityPrefix = "ItemEntity";
                var typeName = item.Type[(item.Type.LastIndexOf('.') + 1)..];
                if (typeName.IStart(itemEntityPrefix)) {
                    typeName = typeName[itemEntityPrefix.Length..];
                }
                Type = typeName.AsDisplayable().OrIfEmpty("Item");
                Name = Blueprint.HasValue() ? resources.Blueprints.GetNameOrBlueprint(Blueprint) : Type;
                IsStackable = stackable && item.Count.HasValue;
                CanEditQuantity = canEditQuantity;
                IsChargable = chargeable;
                CanEditCharges = canEditCharges;
                _savedCount = _count = item.Count.GetValueOrDefault();
                _savedCharges = _charges = item.Charges.GetValueOrDefault();
            }

            public string Name { get; }
            public string Blueprint => _item.Blueprint;
            public string UniqueId => _item.Id;
            public string Type { get; }
            public string Description => string.Empty;
            public int Index { get; }
            public bool IsChargable { get; }
            public bool CanEditCharges { get; }
            public int? MinimumCharges => 1;
            public int Charges
            {
                get => _charges;
                set {
                    if (!CanEditCharges) throw new NotSupportedException();
                    if (value < MinimumCharges) throw new ArgumentOutOfRangeException(nameof(value));
                    _charges = value;
                }
            }
            public bool IsStackable { get; }
            public bool CanEditQuantity { get; }
            public int? MinimumCount => 1;
            public int Count
            {
                get => _count;
                set {
                    if (!CanEditQuantity) throw new NotSupportedException();
                    if (value < MinimumCount) throw new ArgumentOutOfRangeException(nameof(value));
                    _count = value;
                }
            }
            public bool CanEdit => false;
            public string IconUrl => null;
            public bool IsLocked { get => true; set => throw new NotSupportedException(); }
            public bool HasChanges => _count != _savedCount || _charges != _savedCharges;

            public void WriteTo(byte[] entry)
            {
                if (CanEditQuantity && _count != _item.Count) {
                    WriteField(entry, _item.CountOffset.Value, _item.Count.Value, _count, "stack size");
                }
                if (CanEditCharges && _charges != _item.Charges) {
                    WriteField(entry, _item.ChargesOffset.Value, _item.Charges.Value, _charges, "charges");
                }
            }

            private void WriteField(byte[] entry, int offset, int original, int value, string fieldName)
            {
                var field = entry.AsSpan(offset, sizeof(int));
                if (BinaryPrimitives.ReadInt32LittleEndian(field) != original) {
                    throw new InvalidDataException($"The OWL {fieldName} for {Name} differs from the value originally read.");
                }
                BinaryPrimitives.WriteInt32LittleEndian(field, value);
            }

            public void AcceptChanges()
            {
                _savedCount = _count;
                _savedCharges = _charges;
            }
        }
    }
}

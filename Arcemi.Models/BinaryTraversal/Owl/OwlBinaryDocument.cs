using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcemi.Models.BinaryTraversal.Owl
{
    public sealed class OwlBinaryDocument : IBinaryTraversalDocument
    {
        private static readonly byte[] OwlMagic = { (byte)'O', (byte)'W', (byte)'L' };

        private readonly byte[] _bytes;
        private readonly Dictionary<string, BinarySlotDefinition> _slots;

        public OwlBinaryDocument(byte[] bytes, IEnumerable<BinarySlotDefinition> slots)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            EnsureOwlHeader(_bytes);

            if (slots is null) {
                throw new ArgumentNullException(nameof(slots));
            }

            _slots = slots.ToDictionary(s => s.Path.Value, StringComparer.Ordinal);
            foreach (var slot in _slots.Values) {
                ValidateSlot(slot);
            }
        }

        public bool TryGet(BinaryPath path, out object value)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (!_slots.TryGetValue(path.Value, out var slot)) {
                value = null;
                return false;
            }

            value = ReadValue(slot);
            return true;
        }

        public void Set(BinaryPath path, object value)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (!_slots.TryGetValue(path.Value, out var slot)) {
                throw new KeyNotFoundException("No binary slot mapping exists for path '" + path.Value + "'.");
            }

            switch (slot.Kind) {
                case BinarySlotValueKind.Int32:
                    WriteInt32(slot, value);
                    break;
                case BinarySlotValueKind.Utf8String:
                    WriteUtf8String(slot, value);
                    break;
                default:
                    throw new NotSupportedException("Binary slot kind '" + slot.Kind + "' is not supported.");
            }
        }

        public byte[] Save()
        {
            var output = new byte[_bytes.Length];
            Buffer.BlockCopy(_bytes, 0, output, 0, _bytes.Length);
            return output;
        }

        private object ReadValue(BinarySlotDefinition slot)
        {
            return slot.Kind switch {
                BinarySlotValueKind.Int32 => BitConverter.ToInt32(_bytes, slot.Offset),
                BinarySlotValueKind.Utf8String => ReadUtf8String(slot),
                _ => throw new NotSupportedException("Binary slot kind '" + slot.Kind + "' is not supported."),
            };
        }

        private string ReadUtf8String(BinarySlotDefinition slot)
        {
            var raw = Encoding.UTF8.GetString(_bytes, slot.Offset, slot.Length);
            return raw.TrimEnd('\0');
        }

        private void WriteInt32(BinarySlotDefinition slot, object value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            var intValue = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
            var bytes = BitConverter.GetBytes(intValue);
            if (bytes.Length != slot.Length) {
                throw new InvalidOperationException("Mapped Int32 slot length is invalid for path '" + slot.Path.Value + "'.");
            }
            Buffer.BlockCopy(bytes, 0, _bytes, slot.Offset, slot.Length);
        }

        private void WriteUtf8String(BinarySlotDefinition slot, object value)
        {
            var stringValue = value?.ToString() ?? string.Empty;
            var encoded = Encoding.UTF8.GetBytes(stringValue);
            if (encoded.Length > slot.Length) {
                throw new InvalidOperationException(
                    "String value for path '" + slot.Path.Value + "' exceeds mapped slot length " + slot.Length + ".");
            }

            Array.Clear(_bytes, slot.Offset, slot.Length);
            Buffer.BlockCopy(encoded, 0, _bytes, slot.Offset, encoded.Length);
        }

        private void ValidateSlot(BinarySlotDefinition slot)
        {
            if (slot.Offset < 0 || slot.Length <= 0) {
                throw new ArgumentOutOfRangeException(nameof(slot), "Binary slot offset/length must be positive.");
            }

            if (slot.Offset + slot.Length > _bytes.Length) {
                throw new ArgumentOutOfRangeException(nameof(slot), "Binary slot exceeds document size.");
            }
        }

        private static void EnsureOwlHeader(byte[] bytes)
        {
            if (bytes.Length < OwlMagic.Length) {
                throw new InvalidOperationException("Input data is too short to be an OWL file.");
            }

            if (!bytes.AsSpan(0, OwlMagic.Length).SequenceEqual(OwlMagic)) {
                throw new InvalidOperationException("Input data does not start with OWL magic header.");
            }
        }
    }
}

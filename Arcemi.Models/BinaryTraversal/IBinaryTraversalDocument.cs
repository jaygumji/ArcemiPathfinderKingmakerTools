using System;

namespace Arcemi.Models.BinaryTraversal
{
    public enum BinarySlotValueKind
    {
        Int32,
        Utf8String
    }

    public sealed class BinarySlotDefinition
    {
        public BinarySlotDefinition(BinaryPath path, BinarySlotValueKind kind, int offset, int length)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Kind = kind;
            Offset = offset;
            Length = length;
        }

        public BinaryPath Path { get; }
        public BinarySlotValueKind Kind { get; }
        public int Offset { get; }
        public int Length { get; }
    }

    public interface IBinaryTraversalDocument
    {
        bool TryGet(BinaryPath path, out object value);
        void Set(BinaryPath path, object value);
        byte[] Save();
    }
}

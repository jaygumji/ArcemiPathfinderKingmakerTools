using Arcemi.Models.BinaryTraversal;
using Arcemi.Models.BinaryTraversal.Owl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Arcemi.Tests
{
    public class OwlBinaryDocumentTests
    {
        [Fact]
        public void GetAndSet_IntAndStringSlots_Works()
        {
            var bytes = CreateSample();
            var slots = new List<BinarySlotDefinition> {
                new BinarySlotDefinition(BinaryPath.Parse("Party.Index"), BinarySlotValueKind.Int32, 8, 4),
                new BinarySlotDefinition(BinaryPath.Parse("Party.Name"), BinarySlotValueKind.Utf8String, 12, 8),
            };

            var doc = new OwlBinaryDocument(bytes, slots);

            Assert.True(doc.TryGet(BinaryPath.Parse("Party.Index"), out var index));
            Assert.Equal(42, (int)index);
            Assert.True(doc.TryGet(BinaryPath.Parse("Party.Name"), out var name));
            Assert.Equal("ALPHA", (string)name);

            doc.Set(BinaryPath.Parse("Party.Index"), 99);
            doc.Set(BinaryPath.Parse("Party.Name"), "BETA");
            var saved = doc.Save();

            var reparsed = new OwlBinaryDocument(saved, slots);
            Assert.True(reparsed.TryGet(BinaryPath.Parse("Party.Index"), out index));
            Assert.Equal(99, (int)index);
            Assert.True(reparsed.TryGet(BinaryPath.Parse("Party.Name"), out name));
            Assert.Equal("BETA", (string)name);
        }

        [Fact]
        public void Set_StringTooLong_Throws()
        {
            var bytes = CreateSample();
            var slots = new[] {
                new BinarySlotDefinition(BinaryPath.Parse("Party.Name"), BinarySlotValueKind.Utf8String, 12, 8),
            };
            var doc = new OwlBinaryDocument(bytes, slots);

            Assert.Throws<InvalidOperationException>(() => doc.Set(BinaryPath.Parse("Party.Name"), "TOO-LONG-NAME"));
        }

        [Fact]
        public void Constructor_InvalidHeader_Throws()
        {
            var bytes = Encoding.UTF8.GetBytes("NOTOWL");
            var slots = Array.Empty<BinarySlotDefinition>();

            Assert.Throws<InvalidOperationException>(() => new OwlBinaryDocument(bytes, slots));
        }

        private static byte[] CreateSample()
        {
            var bytes = new byte[64];
            bytes[0] = (byte)'O';
            bytes[1] = (byte)'W';
            bytes[2] = (byte)'L';
            Buffer.BlockCopy(BitConverter.GetBytes(42), 0, bytes, 8, 4);
            var text = Encoding.UTF8.GetBytes("ALPHA");
            Buffer.BlockCopy(text, 0, bytes, 12, text.Length);
            return bytes;
        }
    }
}

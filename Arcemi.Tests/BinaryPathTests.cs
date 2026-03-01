using Arcemi.Models.BinaryTraversal;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    public class BinaryPathTests
    {
        [Fact]
        public void Parse_PathWithIndexes_ReturnsExpectedSegments()
        {
            var path = BinaryPath.Parse("Party.Units[2].Stats.Values[10]");

            Assert.Equal(6, path.Segments.Count);
            Assert.Equal(BinaryPathSegmentKind.Property, path.Segments[0].Kind);
            Assert.Equal("Party", path.Segments[0].Value);
            Assert.Equal(BinaryPathSegmentKind.Property, path.Segments[1].Kind);
            Assert.Equal("Units", path.Segments[1].Value);
            Assert.Equal(BinaryPathSegmentKind.Index, path.Segments[2].Kind);
            Assert.Equal("2", path.Segments[2].Value);
            Assert.Equal(BinaryPathSegmentKind.Property, path.Segments[3].Kind);
            Assert.Equal("Stats", path.Segments[3].Value);
            Assert.Equal(BinaryPathSegmentKind.Property, path.Segments[4].Kind);
            Assert.Equal("Values", path.Segments[4].Value);
            Assert.Equal(BinaryPathSegmentKind.Index, path.Segments[5].Kind);
            Assert.Equal("10", path.Segments[5].Value);
        }

        [Fact]
        public void Parse_InvalidIndex_ThrowsFormatException()
        {
            Assert.Throws<System.FormatException>(() => BinaryPath.Parse("Party.Units[abc]"));
        }
    }
}

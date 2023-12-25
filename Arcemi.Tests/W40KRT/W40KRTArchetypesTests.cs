using Arcemi.Models.Warhammer40KRogueTrader;
using System.Collections.Generic;
using Xunit;

namespace Arcemi.Tests.W40KRT
{
    public class W40KRTArchetypesTests
    {
        public static IEnumerable<object[]> ActualLevelData()
        {
            yield return new object[] { "68eaf96bad9748739ca44fedc7b5c7c4", 0, 0 };
            yield return new object[] { "06f4f78a9c1a472b85cd79a9a142153d", 6, 6 };
            yield return new object[] { "6f276e8a8e2c4a548504ae39d2a7f22a", 2, 17 };
            yield return new object[] { "bcefe9c41c7841c9a99b1dbac1793025", 3, 3 };
        }

        [Theory]
        [MemberData(nameof(ActualLevelData))]
        public void ActualLevel(string path, int level, int expected)
        {
            var actual = W40KRTArchetypes.ActualLevel(path, level);
            Assert.Equal(expected, actual);
        }
    }
}

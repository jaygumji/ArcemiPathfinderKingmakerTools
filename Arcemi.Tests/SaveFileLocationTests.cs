using Arcemi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Arcemi.Tests
{
    public class SaveFileLocationTests
    {
        private static readonly string directory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestAreaDirectory");

        private static string P(string name)
        {
            return Path.Combine(directory, name);
        }
        public static IEnumerable<object[]> SaveFileLocationData()
        {
            yield return new object[] { P("Test.zks"), P("Manual_3_Test.zks"), "Manual_3_Test", "Test", ".zks" };
            yield return new object[] { P("Test"), P("Manual_3_Test.zks"), "Manual_3_Test", "Test", ".zks" };
            yield return new object[] { P("Test.bck"), P("Manual_3_Test_bck.zks"), "Manual_3_Test_bck", "Test.bck", ".zks" };
            yield return new object[] { P("Test with space.zks"), P("Manual_3_Test_with_space.zks"), "Manual_3_Test_with_space", "Test with space", ".zks" };
        }

        [Theory]
        [MemberData(nameof(SaveFileLocationData))]
        public void Test(string path, string expectedFilePath, string expectedFileName, string expectedName, string expectedExtension)
        {
            var target = new SaveFileLocation(path);
            Assert.Equal(expectedFilePath, target.FilePath);
            Assert.Equal(expectedFileName, target.FileName);
            Assert.Equal(expectedName, target.Name);
            Assert.Equal(expectedExtension, target.Extension);
            Assert.Equal(directory, target.Directory);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Arcemi.Tests
{
    public static class Res
    {
        private static readonly Assembly Assembly = typeof(Res).Assembly;
        private static readonly string[] Names = Assembly.GetManifestResourceNames();

        public static string Get(string name)
        {
            var fullName = Names.Single(n => n.EndsWith(name, StringComparison.Ordinal));
            using (var stream = Assembly.GetManifestResourceStream(fullName))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class GenderOption
    {
        public string Id { get; }
        public string Name { get; }

        public GenderOption(string id, string name = null)
        {
            Id = id;
            Name = name ?? id;
        }

        public static IReadOnlyList<GenderOption> All { get; } = new[] {
            new GenderOption("Female"),
            new GenderOption("Male"),
        };

        public static IReadOnlyList<GenderOption> Get(string id, out GenderOption current)
        {
            current = All.FirstOrDefault(o => o.Id.Eq(id));
            if (current is object) return All;

            current = new GenderOption(id);
            var options = new List<GenderOption>(All);
            options.Insert(0, current);
            return options;
        }
    }
}
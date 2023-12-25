using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class DataOption
    {
        public string Id { get; }
        public string Name { get; }

        public DataOption(string id, string name = null)
        {
            Id = id;
            Name = name ?? id;
        }

        public static IReadOnlyList<DataOption> Get(IReadOnlyList<DataOption> all, string id, out DataOption current)
        {
            current = all.FirstOrDefault(o => o.Id.Eq(id));
            if (current is object) return all;

            current = new DataOption(id);
            var options = new List<DataOption>(all);
            options.Insert(0, current);
            return options;
        }
    }
}
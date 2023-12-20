using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class BlueprintOption
    {
        public string Id { get; }
        public string Name { get; }

        public BlueprintOption(string id, string name = null)
        {
            Id = id;
            Name = name ?? id;
        }

        public static IReadOnlyList<BlueprintOption> GetOptions(IGameResourcesProvider res, BlueprintTypeId type, string id, out BlueprintOption current)
        {
            var entries = res.Blueprints.GetEntries(type).Select(e => new BlueprintOption(e.Id, e.Name.DisplayName)).OrderBy(o => o.Name).ToList();
            current = entries.FirstOrDefault(e => e.Id.Eq(id));
            if (current is null) {
                current = new BlueprintOption(id);
                entries.Insert(0, current);
            }
            return entries;
        }
    }
}
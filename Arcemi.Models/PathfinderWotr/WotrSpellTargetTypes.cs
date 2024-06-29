using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    internal static class WotrSpellTargetTypes
    {
        public static IReadOnlyList<DataOption> Options { get; } = new[] {
            new DataOption("SingleTarget"),
            new DataOption("Cone"),
            new DataOption("Burst"),
            new DataOption("Line"),
        };

        public static IReadOnlyList<DataOption> Get(string id)
        {
            return DataOption.Get(Options, id, out _);
        }
    }
    public enum SpellTargetType
    {
        SingleTarget,
        Cone,
        Burst,
        Line
    }
}
using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrSavingThrowTypes
    {
        public static IReadOnlyList<DataOption> Options { get; } = new[] {
            new DataOption("Fortitude"),
            new DataOption("Reflex"),
            new DataOption("Will")
        };

        public static IReadOnlyList<DataOption> Get(string id)
        {
            return DataOption.Get(Options, id, out _);
        }
    }
    public enum SavingThrowType
    {
        Unknown,
        Fortitude,
        Reflex,
        Will
    }
}
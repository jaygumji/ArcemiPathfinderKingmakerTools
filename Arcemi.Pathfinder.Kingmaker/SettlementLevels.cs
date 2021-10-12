using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class SettlementLevels
    {
        public const string Village = "Village";
        public const string Town = "Town";
        public const string City = "City";

        public static IEnumerable<EnumModel<string>> All { get; } = new[] {
            new EnumModel<string>(Village, Village),
            new EnumModel<string>(Town, Town),
            new EnumModel<string>(City, City)
        };
    }
}
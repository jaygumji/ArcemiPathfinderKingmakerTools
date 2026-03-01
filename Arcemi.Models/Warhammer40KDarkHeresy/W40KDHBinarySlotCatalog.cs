using Arcemi.Models.BinaryTraversal;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    // This catalog is intentionally explicit: each path is mapped to a concrete byte slot.
    // Start with a tiny set and expand as Dark Heresy fields are validated.
    public static class W40KDHBinarySlotCatalog
    {
        public static IReadOnlyList<BinarySlotDefinition> PartySlots { get; } = new List<BinarySlotDefinition>();
        public static IReadOnlyList<BinarySlotDefinition> PlayerSlots { get; } = new List<BinarySlotDefinition>();

        public static IReadOnlyList<BinarySlotDefinition> GetSlotsForEntry(string entryName)
        {
            if (string.Equals(entryName, "party.owl", System.StringComparison.OrdinalIgnoreCase)) {
                return PartySlots;
            }
            if (string.Equals(entryName, "player.owl", System.StringComparison.OrdinalIgnoreCase)) {
                return PlayerSlots;
            }
            return System.Array.Empty<BinarySlotDefinition>();
        }
    }
}

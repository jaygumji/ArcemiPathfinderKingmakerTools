using System;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public static class CompanionPartState
    {
        public static IReadOnlyDictionary<string, string> All => new Dictionary<string, string>(StringComparer.Ordinal) {
            {InParty, "In party"},
            {Remote, "Remote"},
            {ExCompanion, "Ex companion"}
        };

        public const string InParty = "InParty";
        public const string Remote = "Remote";
        public const string ExCompanion = "ExCompanion";
    }
}
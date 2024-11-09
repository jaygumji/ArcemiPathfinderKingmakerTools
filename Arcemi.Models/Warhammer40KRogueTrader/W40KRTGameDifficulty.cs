using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameDifficulty
    {
        public static IEnumerable<string> Presets { get; } = Enum.GetNames(typeof(Enumerations.W40KRTGameDifficultyPresets));
        public static IEnumerable<string> AutoLevelUp { get; } = Enum.GetNames(typeof(Enumerations.W40KRTGameDifficultyAutoLevelUp));
        public static IEnumerable<string> CombatEncountersCapacity { get; } = Enum.GetNames(typeof(Enumerations.W40KRTGameDifficultyCombatEncountersCapacity));
        public static IEnumerable<string> HardCrowdControlOnPartyMaxDurationRounds { get; } = Enum.GetNames(typeof(Enumerations.W40KRTGameDifficultyHardCrowdControlDurationLimit));
        public static IEnumerable<string> SpaceCombat { get; } = Enum.GetNames(typeof(Enumerations.W40KRTGameDifficultySpaceCombatDifficulty));

        private class Enumerations
        {
            public enum W40KRTGameDifficultyPresets
            {
                Custom = -1,
                Story,
                Casual,
                Normal,
                Daring,
                Core,
                Hard,
                Unfair
            }
            public enum W40KRTGameDifficultyAutoLevelUp
            {
                Off,
                Companions,
                AllPossible
            }
            public enum W40KRTGameDifficultyCombatEncountersCapacity
            {
                Reduced = -1,
                Standard,
                Enlarged
            }
            public enum W40KRTGameDifficultyHardCrowdControlDurationLimit
            {
                OneRound = 1,
                TwoRounds = 2,
                ThreeRounds = 3,
                Unlimited = int.MaxValue
            }
            public enum W40KRTGameDifficultySpaceCombatDifficulty
            {
                Easy,
                Normal,
                Core,
                Hard
            }
        }
    }
}
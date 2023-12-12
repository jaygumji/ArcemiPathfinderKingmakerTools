using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitStatsModel : IGameUnitStatsModel
    {
        public WotrGameUnitStatsModel(UnitEntityModel unit)
        {
            Unit = unit;
            Groupings = new[] {
                Create("General", "Total", Stats.General, a => new WotrGameStatsGeneralEntry(a)),
                Create("Attributes", "Modifiers", Stats.Attributes, a => new WotrGameStatsAttributesEntry(a)),
                Create("Skills", "Total", Stats.Skills, a => new WotrGameStatsSkillsEntry(a)),
                Create("Combat", "Total", Stats.Combat, a => new WotrGameStatsCombatEntry(a)),
                Create("Saves", "Total", Stats.Saves, a => new WotrGameStatsSavesEntry(a))
            };
        }

        private static GameStatsGrouping Create(string name, string infoHeader, IEnumerable<CharacterAttributeModel> atts, Func<CharacterAttributeModel, IGameUnitStatsEntry> factory)
            => new GameStatsGrouping(name, infoHeader, atts.Select(factory).ToArray());

        public UnitEntityModel Unit { get; }
        public StatsModel Stats => Unit.Descriptor.Stats;

        public IReadOnlyList<GameStatsGrouping> Groupings { get; }
        public bool IsSupported => true;
    }
}
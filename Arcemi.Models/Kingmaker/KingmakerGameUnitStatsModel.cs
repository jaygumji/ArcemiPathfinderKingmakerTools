using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitStatsModel : IGameUnitStatsModel
    {
        public KingmakerGameUnitStatsModel(UnitEntityModel unit)
        {
            Unit = unit;
            Groupings = new[] {
                Create("General", "Total", Stats.General, a => new KingmakerGameStatsGeneralEntry(a)),
                Create("Attributes", "Modifiers", Stats.Attributes, a => new KingmakerGameStatsAttributesEntry(a)),
                Create("Skills", "Total", Stats.Skills, a => new KingmakerGameStatsSkillsEntry(a)),
                Create("Combat", "Total", Stats.Combat, a => new KingmakerGameStatsCombatEntry(a)),
                Create("Saves", "Total", Stats.Saves, a => new KingmakerGameStatsSavesEntry(a))
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
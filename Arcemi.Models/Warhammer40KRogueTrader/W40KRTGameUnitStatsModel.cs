using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitStatsModel : IGameUnitStatsModel
    {
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _general = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _ship = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _attributes = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _skills = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _saves = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _combat = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        private readonly List<KeyValuePairObjectModel<StatsContainerConverterModel>> _other = new List<KeyValuePairObjectModel<StatsContainerConverterModel>>();
        public W40KRTGameUnitStatsModel(StatsContainerPartItemModel stats)
        {
            Stats = stats;
            foreach (var stat in stats.Container.ContainerConverter) {
                GroupStat(stat);
            }
            Groupings = new[] {
                Create("General", _general),
                Create("Ship", _ship),
                Create("Attributes", _attributes),
                Create("Skills", _skills),
                Create("Saves", _saves),
                Create("Combat", _combat),
                Create("Other", _other)
            };
        }

        private static GameStatsGrouping Create(string name, IEnumerable<KeyValuePairObjectModel<StatsContainerConverterModel>> atts)
            => new GameStatsGrouping(name, "", atts.Select(x => new W40KRTGameUnitStatsEntry(x)).ToArray());

        private void GroupStat(KeyValuePairObjectModel<StatsContainerConverterModel> stat)
        {
            if (stat.Key.Eq("HitPoints")) _general.Add(stat);
            else if (stat.Key.Eq("Resolve")) _general.Add(stat);
            else if (stat.Key.Eq("PsyRating")) _general.Add(stat);
            else if (stat.Key.Eq("Inertia")) _ship.Add(stat);
            else if (stat.Key.Eq("Morale")) _ship.Add(stat);
            else if (stat.Key.Eq("Crew")) _ship.Add(stat);
            else if (stat.Key.Eq("MilitaryRating")) _ship.Add(stat);
            else if (stat.Key.StartsWith("Turret")) _ship.Add(stat);
            else if (stat.Key.StartsWith("Armour")) _ship.Add(stat);
            else if (stat.Key.Eq("AttackOfOpportunityCount")) _combat.Add(stat);
            else if (stat.Key.Eq("Initiative")) _combat.Add(stat);
            else if (stat.Key.Eq("Speed")) _combat.Add(stat);
            else if (stat.Key.Eq("AttackOfOpportunityCount")) _combat.Add(stat);
            else if (stat.Key.StartsWith("Skill")) _skills.Add(stat);
            else if (stat.Key.StartsWith("Warhammer")) _attributes.Add(stat);
            else if (stat.Key.StartsWith("Save")) _saves.Add(stat);
            else _other.Add(stat);
        }

        public StatsContainerPartItemModel Stats { get; }
        public IReadOnlyList<GameStatsGrouping> Groupings { get; }

        public bool IsSupported => Stats is object;
    }
}
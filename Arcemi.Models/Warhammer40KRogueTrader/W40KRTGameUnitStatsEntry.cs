using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitStatsEntry : IGameUnitStatsEntry
    {
        public W40KRTGameUnitStatsEntry(KeyValuePairObjectModel<StatsContainerConverterModel> model)
        {
            Model = model;
            Name = Model.Key;
            foreach (var prefix in Prefixes) {
                if (Name.StartsWith(prefix)) {
                    Name = Name.Remove(0, prefix.Length);
                    break;
                }
            }
            Name = Name.AsDisplayable();
        }
        public KeyValuePairObjectModel<StatsContainerConverterModel> Model { get; }

        private static readonly IReadOnlyList<string> Prefixes = new[] { "Skill", "Warhammer", "Save" };
        public string Name { get; }
        public int Value { get => Model.Value.BaseValue; set => Model.Value.BaseValue = value; }
        public string Info => null;
    }
}
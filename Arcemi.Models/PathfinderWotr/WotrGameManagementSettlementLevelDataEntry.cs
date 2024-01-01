using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameManagementSettlementLevelDataEntry : IGameDataOptions
    {
        private SettlementStateModel model;

        public WotrGameManagementSettlementLevelDataEntry(SettlementStateModel model)
        {
            this.model = model;
        }

        public IReadOnlyList<DataOption> Options { get; } = SettlementLevels.All.Select(x => new DataOption(x.Value, x.Name)).ToArray();
        public string Value { get => model.Level; set => model.Level = value; }
        public string Label => "Level";
        public GameDataSize Size => GameDataSize.Large;
        public bool IsReadOnly => false;
    }
}
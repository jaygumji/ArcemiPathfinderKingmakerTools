using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementPlaceModelEntry : IGameManagementPlaceModelEntry
    {
        public WotrGameManagementPlaceModelEntry(SettlementStateModel model)
        {
            Model = model;
            DataGroupings = new[] {
                new GameManagementPlaceModelDataGrouping(null, new[] {
                    new WotrGameManagementSettlementLevelDataEntry(model)
                })
            };
        }

        public SettlementStateModel Model { get; }
        public string Name => Model.Name;
        public string Blueprint => Model.BlueprintRef;

        public IReadOnlyList<GameManagementPlaceModelDataGrouping> DataGroupings { get; }
    }

    internal class WotrGameManagementSettlementLevelDataEntry : IGameManagementPlaceModelDataGroupingOptionsEntry
    {
        private SettlementStateModel model;

        public WotrGameManagementSettlementLevelDataEntry(SettlementStateModel model)
        {
            this.model = model;
        }

        public IReadOnlyList<DataOption> Options { get; } = SettlementLevels.All.Select(x => new DataOption(x.Value, x.Name)).ToArray();
        public string Value { get => model.Level; set => model.Level = value; }
        public string Label => "Level";
    }
}
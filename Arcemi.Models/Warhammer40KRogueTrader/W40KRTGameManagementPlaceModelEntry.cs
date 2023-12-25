using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameManagementPlaceModelEntry : IGameManagementPlaceModelEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameManagementPlaceModelEntry(RefModel model)
        {
            Ref = model;
            MA = model.GetAccessor();
            CA = MA.Object<RefModel>("Colony").GetAccessor();

            Name = Res.Blueprints.GetNameOrBlueprint(Blueprint);

            DataGroupings = new[] {
                new GameManagementPlaceModelDataGrouping(null, new [] {
                    new W40KRTGameManagementPlaceIntDataEntry(CA.Object<RefModel>("Contentment"), "Contentment"),
                    new W40KRTGameManagementPlaceIntDataEntry(CA.Object<RefModel>("Efficiency"), "Efficiency"),
                    new W40KRTGameManagementPlaceIntDataEntry(CA.Object<RefModel>("Security"), "Security")
                })
            };
        }

        public RefModel Ref { get; }
        public ModelDataAccessor MA { get; }
        public ModelDataAccessor CA { get; }
        public string Name { get; }
        public string Planet => MA.Value<string>();
        public string Area => MA.Value<string>();
        public string Blueprint => CA.Value<string>();
        public IReadOnlyList<GameManagementPlaceModelDataGrouping> DataGroupings { get; }
    }
}
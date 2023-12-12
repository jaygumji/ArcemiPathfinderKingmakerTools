namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitFeatEntry : IGameUnitFeatEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameUnitFeatEntry(FactItemModel model)
        {
            Model = (W40KRTFeatFactItemModel)model;
            DisplayName = Res.Blueprints.GetNameOrBlueprint(model.Blueprint);
        }
        public W40KRTFeatFactItemModel Model { get; }

        public string DisplayName { get; }
        public string Blueprint => Model.Blueprint;
        public string Category => null;
        public bool IsRanked => Model.IsRanked;
        public int Rank { get => Model.Rank; set => Model.Rank = value; }
        public int SourceLevel => 0;

        public string Export()
        {
            return Model.Export();
        }
    }
}
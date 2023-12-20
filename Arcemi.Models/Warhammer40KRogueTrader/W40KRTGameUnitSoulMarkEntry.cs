namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSoulMarkEntry : IGameUnitFactEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTGameUnitSoulMarkEntry(FactItemModel model)
        {
            Ref = model;
        }

        public FactItemModel Ref { get; }

        public string DisplayName => Ref.DisplayName(Res);
        public string Blueprint => Ref.Blueprint;
    }
}
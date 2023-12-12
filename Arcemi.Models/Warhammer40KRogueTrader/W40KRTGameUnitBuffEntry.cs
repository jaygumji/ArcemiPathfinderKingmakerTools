namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitBuffEntry : IGameUnitBuffEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTGameUnitBuffEntry(FactItemModel model)
        {
            Model = (W40KRTBuffFactItemModel)model;
            DisplayName = Res.Blueprints.GetNameOrBlueprint(model.Blueprint);
        }

        public W40KRTBuffFactItemModel Model { get; }

        public string DisplayName { get; }
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
        public TimeParts Duration => null;
    }
}
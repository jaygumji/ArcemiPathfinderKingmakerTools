namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitAbilityEntry : IGameUnitAbilityEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameUnitAbilityEntry(FactItemModel model)
        {
            Model = (W40KRTAbilityFactItemModel)model;
            DisplayName = Res.Blueprints.GetNameOrBlueprint(model.Blueprint);
        }

        public W40KRTAbilityFactItemModel Model { get; }

        public string DisplayName { get; }
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
    }
}
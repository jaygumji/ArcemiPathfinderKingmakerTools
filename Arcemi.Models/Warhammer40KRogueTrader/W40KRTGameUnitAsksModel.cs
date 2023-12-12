namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitAsksModel : IGameUnitAsksModel
    {
        public W40KRTGameUnitAsksModel(UnitAsksPartItemModel model)
        {
            Model = model;
        }

        public UnitAsksPartItemModel Model { get; }
        public string Custom { get => Model.CustomAsks; set => Model.CustomAsks = value; }

        public bool IsSupported => Model is object;
    }
}
using System;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitAlignmentHistoryEntryModel : IGameUnitAlignmentHistoryEntryModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTGameUnitAlignmentHistoryEntryModel(AlignmentHistoryOnPartItemModel model)
        {
            Model = model;
        }

        public AlignmentHistoryOnPartItemModel Model { get; }

        public string Direction => Model.Direction;
        public string Provider => Res.Blueprints.GetNameOrBlueprint(Model.Provider);

        public int X => (int)Math.Round(Model.Position.X);
        public int Y => (int)Math.Round(Model.Position.Y);
    }
}
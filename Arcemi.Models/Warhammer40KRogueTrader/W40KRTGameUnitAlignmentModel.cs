using System;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitAlignmentModel : IGameUnitAlignmentModel
    {
        public W40KRTGameUnitAlignmentModel(UnitAlignmentPartItemModel model)
        {
            Model = model;
            History = new GameModelCollection<IGameUnitAlignmentHistoryEntryModel, AlignmentHistoryOnPartItemModel>(null, a => new W40KRTGameUnitAlignmentHistoryEntryModel(a));
            //History = new GameModelCollection<IGameUnitAlignmentHistoryEntryModel, AlignmentHistoryOnPartItemModel>(model?.History, a => new W40KRTGameUnitAlignmentHistoryEntryModel(a));
        }

        public UnitAlignmentPartItemModel Model { get; }

        public string DisplayName => Direction.ToString().AsDisplayable();

        public int X { get => (int)Math.Round(Model?.Vector?.X ?? 0); set => Model.Vector.X = value; }
        public int Y { get => (int)Math.Round(Model?.Vector?.Y ?? 0); set => Model.Vector.Y = value; }

        public Alignment Direction => AlignmentExtensions.Detect(X, Y);

        public string LockedAlignmentMask { get => Model?.LockedAlignmentMask; set => Model.LockedAlignmentMask = value; }

        public IGameModelCollection<IGameUnitAlignmentHistoryEntryModel> History { get; }

        public bool IsSupported => false;
    }
}
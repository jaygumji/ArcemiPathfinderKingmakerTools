namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitProgressionCollectionWriter : GameModelCollectionWriter<IGameDataObject, UnitProgressionSelectionOfPartModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
        }

        public override bool IsAddEnabled => false;
        public override bool IsRemoveEnabled => false;
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTCargoItemWriter : GameModelCollectionWriter<IGameItemEntry, RefModel>
    {
        private readonly RefModel Ref;

        public W40KRTCargoItemWriter(RefModel @ref)
        {
            this.Ref = @ref;
        }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
        }
    }
}
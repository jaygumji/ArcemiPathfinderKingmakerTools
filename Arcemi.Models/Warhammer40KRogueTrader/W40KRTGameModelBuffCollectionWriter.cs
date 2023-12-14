namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelBuffCollectionWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            W40KRTBuffFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
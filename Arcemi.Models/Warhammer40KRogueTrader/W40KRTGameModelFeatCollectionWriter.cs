namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelFeatCollectionWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            W40KRTFeatFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
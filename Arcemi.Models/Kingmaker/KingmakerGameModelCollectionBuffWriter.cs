namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionBuffWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            BuffFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
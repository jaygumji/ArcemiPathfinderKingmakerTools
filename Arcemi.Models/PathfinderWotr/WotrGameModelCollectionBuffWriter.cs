namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionBuffWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            BuffFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionFeatWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            FeatureFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
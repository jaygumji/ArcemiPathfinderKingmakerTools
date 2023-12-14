namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionFeatWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            FeatureFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
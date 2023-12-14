namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionAbilityWriter : GameModelCollectionWriter<IGameUnitAbilityEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            AbilityFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionAbilityWriter : GameModelCollectionWriter<IGameUnitAbilityEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            AbilityFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
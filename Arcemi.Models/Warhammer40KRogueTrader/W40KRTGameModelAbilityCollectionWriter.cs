namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelAbilityCollectionWriter : GameModelCollectionWriter<IGameUnitAbilityEntry, FactItemModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            W40KRTAbilityFactItemModel.Prepare(args.References, args.Obj);
        }
    }
}
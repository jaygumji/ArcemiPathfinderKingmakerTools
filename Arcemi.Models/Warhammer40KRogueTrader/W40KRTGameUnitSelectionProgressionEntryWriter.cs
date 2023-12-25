namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSelectionProgressionEntryWriter : GameModelCollectionWriter<IGameUnitSelectionProgressionEntry, UnitProgressionSelectionOfPartModel>
    {
        private readonly IGameUnitModel owner;

        public W40KRTGameUnitSelectionProgressionEntryWriter(IGameUnitModel owner)
        {
            this.owner = owner;
        }
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("Path", GetPathFor(owner.Type));
            args.Obj.Add("Level", 0);
            args.Obj.Add("Selection", args.Blueprint);
            args.Obj.Add("Feature", args.Data is string str ? str : "");
            args.Obj.Add("Rank", 1);
        }

        private string GetPathFor(UnitEntityType type)
        {
            switch (type) {
                case UnitEntityType.Player:
                case UnitEntityType.Mercenary:
                    return "45181a40472441a8904a5282f83693f4";
                case UnitEntityType.Companion:
                    return "68eaf96bad9748739ca44fedc7b5c7c4";
            }
            return null;
        }
    }
}
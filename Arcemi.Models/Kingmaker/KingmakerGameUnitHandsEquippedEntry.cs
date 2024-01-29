namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitHandsEquippedEntry : IGameUnitHandsEquippedEntry
    {
        public KingmakerGameUnitHandsEquippedEntry(HandsEquipmentSetModel @ref)
        {
            Ref = @ref;
            Primary = @ref?.PrimaryHand;
            Secondary = @ref?.SecondaryHand;
        }

        public HandsEquipmentSetModel Ref { get; }
        public IGameUnitEquippedEntry Primary { get; }
        public IGameUnitEquippedEntry Secondary { get; }
    }
}
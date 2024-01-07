using System;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitHandsEquippedEntry : IGameUnitHandsEquippedEntry
    {
        public WotrGameUnitHandsEquippedEntry(HandsEquipmentSetModel @ref)
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
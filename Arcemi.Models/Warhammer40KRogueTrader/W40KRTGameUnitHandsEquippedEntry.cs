using System;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitHandsEquippedEntry : IGameUnitHandsEquippedEntry
    {
        public W40KRTGameUnitHandsEquippedEntry(HandsEquipmentSetModel @ref)
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
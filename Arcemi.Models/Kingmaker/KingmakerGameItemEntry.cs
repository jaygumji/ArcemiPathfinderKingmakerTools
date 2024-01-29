namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameItemEntry : IGameItemEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public KingmakerGameItemEntry(ItemModel item)
        {
            Ref = item;
            UniqueId = System.Guid.NewGuid().ToString("N");
        }

        public ItemModel Ref { get; }
        public string Name => Ref.DisplayName(Res);
        public string Blueprint => Ref.Blueprint;
        public string UniqueId { get; }
        public string Type => Ref.DisplayType;
        public string Description => Ref.DisplayDescription;
        public int Index => Ref.InventorySlotIndex;
        public bool IsLocked { get => Ref.IsNonRemovable; set => Ref.IsNonRemovable = value; }

        public bool IsChargable => Ref.IsChargable;
        public int Charges { get => Ref.Charges; set => Ref.Charges = value; }
        public bool IsStackable => Ref.IsStackable;
        public int Count { get => Ref.Count; set => Ref.Count = value; }
        public bool CanEdit => false;

        public string IconUrl
        {
            get {
                if (Res.Blueprints.TryGet(Ref.Blueprint, out var blueprint)) {
                    if (blueprint.Type.IsItemWeapon) {
                        return "/images/ItemTypes/Weapons.png";
                    }
                    if (blueprint.Type.IsItemArmorOrShield) {
                        return "/images/ItemTypes/ArmorsAndShields.png";
                    }
                    if (blueprint.Type.IsItemAccessory) {
                        return "/images/ItemTypes/Accessories.png";
                    }
                    if (blueprint.Type.IsItemIngredient) {
                        return "/images/ItemTypes/Ingredients.png";
                    }
                    if (blueprint.Type.IsItemUsable) {
                        return "/images/ItemTypes/Usable.png";
                    }
                    if (blueprint.Type.IsItemNotable) {
                        return "/images/ItemTypes/Notable.png";
                    }
                    return "/images/ItemTypes/Other.png";
                }
                switch (Ref.ItemType) {
                    case ItemType.Weapon:
                        return "/images/ItemTypes/Weapons.png";
                    case ItemType.Armor:
                    case ItemType.Shield:
                        return "/images/ItemTypes/ArmorsAndShields.png";
                    case ItemType.Usable:
                        return "/images/ItemTypes/Usable.png";
                }
                return "/images/ItemTypes/Other.png";

            }
        }
    }
}
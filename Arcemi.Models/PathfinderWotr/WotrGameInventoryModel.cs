using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameInventoryModel : IGameInventoryModel, IGameItemSection
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        private readonly Dictionary<string, IGameItemEntry> _equippedLookup;

        public string Name { get; }
        public IReadOnlyList<IGameItemSection> Sections { get; }
        public bool IsSupported => true;

        private class WotrGameInventoryItemWriter : GameModelCollectionWriter<IGameItemEntry, ItemModel>
        {
            private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

            private InventoryModel Ref { get; }

            public WotrGameInventoryItemWriter(InventoryModel inventory)
            {
                Ref = inventory;
            }
            public override void BeforeAdd(BeforeAddCollectionItemArgs args)
            {
                if (args.Blueprint.HasValue()) {
                    if (!(args.Data is ItemType itemType)) {
                        var type = Res.Blueprints.Get(args.Blueprint).Type;
                        itemType = GetItemType(type);
                    }
                    ItemModel.Prepare(Ref, args.References, args.Obj, itemType);
                }
            }

            public override void AfterAdd(AfterAddCollectionItemArgs<IGameItemEntry, ItemModel> args)
            {
                if (args.Blueprint.HasValue()) {
                    var template = Res.GetItemTemplate(args.Blueprint);
                    if (template is object) {
                        args.Model.Import(template);
                    }
                    Ref.SetInventorySlotIndexToLast(args.Model);
                    args.Model.Blueprint = args.Blueprint;
                }
            }

            private static ItemType GetItemType(BlueprintType type)
            {
                if (ReferenceEquals(type, WotrBlueprintProvider.ItemWeapon)) {
                    return ItemType.Weapon;
                }
                if (ReferenceEquals(type, WotrBlueprintProvider.ItemShield)) {
                    return ItemType.Shield;
                }
                if (ReferenceEquals(type, WotrBlueprintProvider.ItemArmor)) {
                    return ItemType.Armor;
                }
                if (ReferenceEquals(type, WotrBlueprintProvider.ItemEquipmentUsable)) {
                    return ItemType.Usable;
                }
                return ItemType.Simple;
            }
        }
        public IGameModelCollection<IGameItemEntry> Items { get; }

        public WotrGameInventoryModel(InventoryModel model, string name)
        {
            Name = name;
            Sections = new IGameItemSection[] { this };
            Items = new GameModelCollection<IGameItemEntry, ItemModel>(model.Items, x => new WotrGameItemEntry(x), IsValidItem, new WotrGameInventoryItemWriter(model));
            _equippedLookup = model.Items.Where(i => i.InventorySlotIndex < 0 || i.WielderRef.HasValue() || i.HoldingSlot is object)
                .ToDictionary(i => i.UniqueId, i => (IGameItemEntry)new WotrGameItemEntry(i), StringComparer.Ordinal);
        }

        public IGameItemEntry FindEquipped(object itemRef)
        {
            if (!(itemRef is string uniqueId)) return null;
            if (string.IsNullOrEmpty(uniqueId)) return null;
            if (_equippedLookup.TryGetValue(uniqueId, out var item)) return item;
            return null;
        }
        
        private static readonly HashSet<string> ItemFilter = new HashSet<string>(StringComparer.Ordinal) {
            "95c126deb99ba054aa5b84710520c035" // Finnean Base Item
        };

        private bool IsValidItem(ItemModel i)
        {
            if (!string.IsNullOrEmpty(i.WielderRef)) return false;
            if (i.HoldingSlot != null) return false;
            if (ItemFilter.Contains(i.Blueprint)) return false;
            return true;
        }

        public IEnumerable<IBlueprintMetadataEntry> GetAddableItems(string typeFullName = null)
        {
            return typeFullName.HasValue()
                ? Res.Blueprints.GetEntries(typeFullName)
                : AddableTypes.SelectMany(t => Res.Blueprints.GetEntries(t));
        }

        public IReadOnlyList<BlueprintType> AddableTypes { get; } = new[] {
            WotrBlueprintProvider.ItemArmor,
            WotrBlueprintProvider.ItemShield,
            WotrBlueprintProvider.ItemEquipmentBelt,
            WotrBlueprintProvider.ItemEquipmentFeet,
            WotrBlueprintProvider.ItemEquipmentGlasses,
            WotrBlueprintProvider.ItemEquipmentGloves,
            WotrBlueprintProvider.ItemEquipmentHead,
            WotrBlueprintProvider.Ingredient,
            WotrBlueprintProvider.ItemEquipmentNeck,
            WotrBlueprintProvider.ItemEquipmentRing,
            WotrBlueprintProvider.ItemEquipmentShirt,
            WotrBlueprintProvider.ItemEquipmentShoulders,
            WotrBlueprintProvider.ItemEquipmentUsable,
            WotrBlueprintProvider.ItemEquipmentWrist,
            WotrBlueprintProvider.ItemKey,
            WotrBlueprintProvider.ItemNote,
            WotrBlueprintProvider.ItemThiefTool,
            WotrBlueprintProvider.ItemWeapon,
            WotrBlueprintProvider.Item
        };
    }
}

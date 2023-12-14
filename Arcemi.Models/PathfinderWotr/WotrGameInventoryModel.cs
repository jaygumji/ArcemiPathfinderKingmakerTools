using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameInventoryModel : IGameInventoryModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        private readonly InventoryModel _model;

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
                    ItemModel item = null;
                    Ref.SetInventorySlotIndexToLast(item);
                    item.Blueprint = args.Blueprint;
                }
            }

            private static ItemType GetItemType(BlueprintType type)
            {
                if (ReferenceEquals(type, WotrBlueprintTypeProvider.ItemWeapon)) {
                    return ItemType.Weapon;
                }
                if (ReferenceEquals(type, WotrBlueprintTypeProvider.ItemShield)) {
                    return ItemType.Shield;
                }
                if (ReferenceEquals(type, WotrBlueprintTypeProvider.ItemArmor)) {
                    return ItemType.Armor;
                }
                if (ReferenceEquals(type, WotrBlueprintTypeProvider.ItemEquipmentUsable)) {
                    return ItemType.Usable;
                }
                return ItemType.Simple;
            }
        }
        public IGameModelCollection<IGameItemEntry> Items { get; }

        public WotrGameInventoryModel(InventoryModel model)
        {
            _model = model;
            Items = new GameModelCollection<IGameItemEntry, ItemModel>(model.Items, x => new WotrGameItemEntry(x), IsValidItem, new WotrGameInventoryItemWriter(model));
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
            WotrBlueprintTypeProvider.ItemArmor,
            WotrBlueprintTypeProvider.ItemShield,
            WotrBlueprintTypeProvider.ItemEquipmentBelt,
            WotrBlueprintTypeProvider.ItemEquipmentFeet,
            WotrBlueprintTypeProvider.ItemEquipmentGlasses,
            WotrBlueprintTypeProvider.ItemEquipmentGloves,
            WotrBlueprintTypeProvider.ItemEquipmentHead,
            WotrBlueprintTypeProvider.Ingredient,
            WotrBlueprintTypeProvider.ItemEquipmentNeck,
            WotrBlueprintTypeProvider.ItemEquipmentRing,
            WotrBlueprintTypeProvider.ItemEquipmentShirt,
            WotrBlueprintTypeProvider.ItemEquipmentShoulders,
            WotrBlueprintTypeProvider.ItemEquipmentUsable,
            WotrBlueprintTypeProvider.ItemEquipmentWrist,
            WotrBlueprintTypeProvider.ItemKey,
            WotrBlueprintTypeProvider.ItemNote,
            WotrBlueprintTypeProvider.ItemThiefTool,
            WotrBlueprintTypeProvider.ItemWeapon,
            WotrBlueprintTypeProvider.Item
        };
    }
}

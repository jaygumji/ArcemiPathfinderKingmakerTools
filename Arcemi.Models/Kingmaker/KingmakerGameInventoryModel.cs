using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;

namespace Arcemi.Models.Kingmaker
{
    public class KingmakerGameInventoryModel : IGameInventoryModel, IGameItemSection
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        private readonly Dictionary<string, IGameItemEntry> _equippedLookup;

        public string Name { get; }
        public IReadOnlyList<IGameItemSection> Sections { get; }
        public bool IsSupported => true;

        private class KingmakerGameInventoryItemWriter : GameModelCollectionWriter<IGameItemEntry, ItemModel>
        {
            private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;

            private InventoryModel Ref { get; }
            public TimeSpan GameTime { get; }

            public KingmakerGameInventoryItemWriter(InventoryModel inventory, TimeSpan gameTime)
            {
                Ref = inventory;
                GameTime = gameTime;
            }
            public override void BeforeAdd(BeforeAddCollectionItemArgs args)
            {
                if (args.Blueprint.HasValue()) {
                    if (!(args.Data is ItemType itemType)) {
                        var type = Res.Blueprints.Get(args.Blueprint).Type;
                        itemType = GetItemType(type);
                    }
                    switch (itemType) {
                        case ItemType.Weapon:
                            args.Obj.Add("$type", WeaponItemModel.TypeRef);
                            args.Obj.Add("Charges", 0);
                            break;
                        case ItemType.Armor:
                            args.Obj.Add("$type", ArmorItemModel.TypeRef);
                            args.Obj.Add("Charges", 0);
                            break;
                        case ItemType.Shield:
                            args.Obj.Add("$type", ShieldItemModel.TypeRef);
                            args.Obj.Add("Charges", 0);
                            break;
                        case ItemType.Usable:
                            args.Obj.Add("$type", UsableItemModel.TypeRef);
                            args.Obj.Add("Charges", 1);
                            break;
                        default:
                            args.Obj.Add("$type", SimpleItemModel.TypeRef);
                            args.Obj.Add("Charges", 0);
                            break;
                    }
                    args.Obj.Add("m_Blueprint", args.Blueprint);
                    args.Obj.Add("Count", 1);
                    args.Obj.Add("Time", TimeSpan.Zero);

                    //switch (itemType) {
                    //    case ItemType.Weapon:
                    //    case ItemType.Simple:
                    //        var enchantments = args.References.Create();
                    //        enchantments.Add("m_Facts", new JArray());
                    //        enchantments.Add("Owner", args.References.CreateReference(enchantments, args.Obj));
                    //        enchantments.Add("ActiveByDefault", true);
                    //        args.Obj.Add("m_Enchantments", enchantments);
                    //        break;
                    //}

                    args.Obj.Add("m_FactsAppliedToWielder", new JArray());
                    args.Obj.Add("m_IdentifyRolls", new JArray());
                    args.Obj.Add("Collection", args.References.CreateReference(args.Obj, Ref.Id));

                    args.Obj.Add("IsIdentified", true);
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
                    args.Model.Time = GameTime;
                }
            }

            private static ItemType GetItemType(BlueprintType type)
            {
                if (ReferenceEquals(type, KingmakerBlueprintProvider.ItemWeapon)) {
                    return ItemType.Weapon;
                }
                if (ReferenceEquals(type, KingmakerBlueprintProvider.ItemShield)) {
                    return ItemType.Shield;
                }
                if (ReferenceEquals(type, KingmakerBlueprintProvider.ItemArmor)) {
                    return ItemType.Armor;
                }
                if (ReferenceEquals(type, KingmakerBlueprintProvider.ItemEquipmentUsable)) {
                    return ItemType.Usable;
                }
                return ItemType.Simple;
            }
        }
        public IGameModelCollection<IGameItemEntry> Items { get; }

        public KingmakerGameInventoryModel(InventoryModel model, TimeSpan gameTime, string name)
        {
            Name = name;
            Sections = new IGameItemSection[] { this };
            Items = new GameModelCollection<IGameItemEntry, ItemModel>(model.Items, x => new KingmakerGameItemEntry(x), IsValidItem, new KingmakerGameInventoryItemWriter(model, gameTime));
            _equippedLookup = model.Items.Where(i => i.InventorySlotIndex < 0 || i.WielderRef.HasValue() || i.HoldingSlot is object)
                .Select(i => new KingmakerGameItemEntry(i))
                .ToDictionary(i => i.UniqueId, i => (IGameItemEntry)i, StringComparer.Ordinal);
        }

        public IGameItemEntry FindEquipped(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId)) return null;
            if (_equippedLookup.TryGetValue(uniqueId, out var item)) return item;
            return null;
        }

        private static readonly HashSet<string> ItemFilter = new HashSet<string>(StringComparer.Ordinal) {
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
            KingmakerBlueprintProvider.ItemArmor,
            KingmakerBlueprintProvider.ItemShield,
            KingmakerBlueprintProvider.ItemEquipmentBelt,
            KingmakerBlueprintProvider.ItemEquipmentFeet,
            KingmakerBlueprintProvider.ItemEquipmentGlasses,
            KingmakerBlueprintProvider.ItemEquipmentGloves,
            KingmakerBlueprintProvider.ItemEquipmentHead,
            KingmakerBlueprintProvider.Ingredient,
            KingmakerBlueprintProvider.ItemEquipmentNeck,
            KingmakerBlueprintProvider.ItemEquipmentRing,
            KingmakerBlueprintProvider.ItemEquipmentShirt,
            KingmakerBlueprintProvider.ItemEquipmentShoulders,
            KingmakerBlueprintProvider.ItemEquipmentUsable,
            KingmakerBlueprintProvider.ItemEquipmentWrist,
            KingmakerBlueprintProvider.ItemKey,
            KingmakerBlueprintProvider.ItemNote,
            KingmakerBlueprintProvider.ItemThiefTool,
            KingmakerBlueprintProvider.ItemWeapon,
            KingmakerBlueprintProvider.Item
        };
    }
}
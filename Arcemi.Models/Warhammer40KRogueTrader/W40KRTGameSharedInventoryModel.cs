using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameSharedInventoryModel : IGameInventoryModel, IGameItemSection
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        private readonly Dictionary<string, IGameItemEntry> _equippedLookup;
        private readonly ListAccessor<RefModel> _itemModels;
        public W40KRTGameSharedInventoryModel(RefModel inventory)
        {
            Ref = inventory?.GetAccessor().Object<RefModel>("CollectionConverter");
            if (IsSupported) {
                A = Ref?.GetAccessor();
                Sections = new IGameItemSection[] { this };
                var itemModels = _itemModels = A.List<RefModel>("m_Items");
                Items = new GameModelCollection<IGameItemEntry, RefModel>(itemModels, x => new W40KRTGameItemEntry(x), IsValidItem, new W40KRTGameInventoryItemWriter(Ref));

                _equippedLookup = itemModels.Where(i => i.GetAccessor().Value<int>("m_InventorySlotIndex") < 0)
                    .ToDictionary(i => i.GetAccessor().Value<string>("UniqueId"), i => (IGameItemEntry)new W40KRTGameItemEntry(i), StringComparer.Ordinal);
            }
        }
        public string Name => "Party";
        public RefModel Ref { get; }
        private ModelDataAccessor A { get; }

        private bool IsValidItem(RefModel i)
        {
            if (i.GetAccessor().Value<string>("m_WielderRef").HasValue()) return false;
            return true;
        }

        public IGameItemEntry FindEquipped(object itemRef)
        {
            if (!(itemRef is string uniqueId)) return null;
            if (string.IsNullOrEmpty(uniqueId)) return null;
            if (_equippedLookup.TryGetValue(uniqueId, out var item)) return item;
            return null;
        }

        internal RefModel AddEquippedAugment(string blueprint, string wielder)
        {
            var writer = new W40KRTGameInventoryItemWriter(Ref);
            var item = _itemModels.Add((references, obj) => {
                writer.BeforeAdd(new BeforeAddCollectionItemArgs(references, obj, blueprint, W40KRTItemType.Get(W40KRTBlueprintProvider.ItemAugment)));
                obj["m_InventorySlotIndex"] = -1;
                obj.Add("m_WielderRef", wielder);
                obj.Add("IsNonRemovable", true);
            });
            _equippedLookup.Add(item.GetAccessor().Value<string>("UniqueId"), new W40KRTGameItemEntry(item));
            return item;
        }

        internal void RemoveEquipped(string id)
        {
            var item = _itemModels.FirstOrDefault(i => i.GetAccessor().Value<string>("UniqueId").Eq(id));
            if (item is null) return;
            _itemModels.Remove(item);
            _equippedLookup.Remove(id);
        }

        public bool IsSupported => Ref is object;
        public IReadOnlyList<IGameItemSection> Sections { get; }

        public IGameModelCollection<IGameItemEntry> Items { get; }
        public IReadOnlyList<BlueprintType> AddableTypes { get; } = new[] {
           W40KRTBlueprintProvider.ItemEquipmentFeet,
           W40KRTBlueprintProvider.ItemEquipmentGloves,
           W40KRTBlueprintProvider.ItemEquipmentHead,
           W40KRTBlueprintProvider.ItemEquipmentNeck,
           W40KRTBlueprintProvider.ItemEquipmentPetProtocol,
           W40KRTBlueprintProvider.ItemAugment,
           W40KRTBlueprintProvider.ItemEquipmentRing,
           W40KRTBlueprintProvider.ItemEquipmentShoulders,
           W40KRTBlueprintProvider.ItemEquipmentUsable,
           W40KRTBlueprintProvider.ItemKey,
           W40KRTBlueprintProvider.ItemNote,
           W40KRTBlueprintProvider.Item,
           W40KRTBlueprintProvider.ItemArmor,
           W40KRTBlueprintProvider.ItemWeapon,
           W40KRTBlueprintProvider.ItemShield,
           W40KRTBlueprintProvider.ItemArmorPlating,
           W40KRTBlueprintProvider.ItemAugerArray,
           W40KRTBlueprintProvider.ItemLifeSustainer,
           W40KRTBlueprintProvider.ItemMechadendrite,
           W40KRTBlueprintProvider.ItemPlasmaDrives,
           W40KRTBlueprintProvider.ItemResourceMiner,
           W40KRTBlueprintProvider.ItemVoidShieldGenerator,
           W40KRTBlueprintProvider.StarshipWeapon,
           W40KRTBlueprintProvider.StarshipAmmo,
        };

        public IEnumerable<IBlueprintMetadataEntry> GetAddableItems(string typeFullName = null)
        {
            return typeFullName.HasValue()
                ? Res.Blueprints.GetEntries(typeFullName)
                : AddableTypes.SelectMany(t => Res.Blueprints.GetEntries(t));
        }
    }
}
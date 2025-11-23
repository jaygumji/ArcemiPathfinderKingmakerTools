using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameSharedInventoryModel : IGameInventoryModel, IGameItemSection
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        private readonly Dictionary<string, IGameItemEntry> _equippedLookup;
        public W40KRTGameSharedInventoryModel(RefModel inventory)
        {
            Ref = inventory?.GetAccessor().Object<RefModel>("CollectionConverter");
            if (IsSupported) {
                A = Ref?.GetAccessor();
                Sections = new IGameItemSection[] { this };
                var itemModels = A.List<RefModel>("m_Items");
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

        public bool IsSupported => Ref is object;
        public IReadOnlyList<IGameItemSection> Sections { get; }

        public IGameModelCollection<IGameItemEntry> Items { get; }
        public IReadOnlyList<BlueprintType> AddableTypes { get; } = new[] {
           W40KRTBlueprintProvider.ItemEquipmentFeet,
           W40KRTBlueprintProvider.ItemEquipmentGloves,
           W40KRTBlueprintProvider.ItemEquipmentHead,
           W40KRTBlueprintProvider.ItemEquipmentNeck,
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
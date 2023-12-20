using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTCargoEntityModel : RefModel, IGameItemSection
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTCargoEntityModel(ModelDataAccessor accessor) : base(accessor)
        {
            if (Res.Blueprints.TryGet(Blueprint, out var entry)) {
                var subNameIndex = entry.Name.Original.IndexOf('[');
                if (subNameIndex >= 0) {
                    Name = entry.Name.Original.Substring(subNameIndex + 1, entry.Name.Original.Length - subNameIndex - 2).AsDisplayable();
                }
                else {
                    Name = entry.Name.DisplayName;
                }
            }
            else {
                Name = Blueprint;
            }
            var parts = A.Object<RefModel>("Parts");
            var container = parts.GetAccessor().List<RefModel>("Container");
            var inventory = container.FirstOrDefault(x => x.GetAccessor().TypeValue().Eq("Kingmaker.UnitLogic.Parts.PartInventory, Code"));
            var items = inventory?.GetAccessor().Object<RefModel>("CollectionConverter")?.GetAccessor().List<RefModel>("m_Items");
            Items = new GameModelCollection<IGameItemEntry, RefModel>(items, x => new W40RTGameCargoItemEntry(x), IsValidItem); //, new W40KRTCargoItemWriter(this));
        }

        private bool IsValidItem(RefModel i)
        {
            return true;
        }

        public string Name { get; }
        public string Blueprint => A.Value<string>();

        public IGameModelCollection<IGameItemEntry> Items { get; }

        public IReadOnlyList<BlueprintType> AddableTypes { get; } = Array.Empty<BlueprintType>();

        public IEnumerable<IBlueprintMetadataEntry> GetAddableItems(string typeFullName = null)
        {
            return Array.Empty<IBlueprintMetadataEntry>();
        }
    }
}
using Arcemi.Pathfinder.Kingmaker;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class ItemViewModel
    {

        public RawItemData RawData { get; }
        public DescriptiveItemData DescriptiveData { get; }

        public string DisplayName => (DescriptiveData?.Name).OrIfEmpty(RawData.Name.AsDisplayable());
        public string DisplayType => (DescriptiveData?.Type).OrIfEmpty(RawData.TypeName);
        public string DisplayDescription => (DescriptiveData?.Description).OrIfEmpty(null);

        public ItemViewModel(RawItemData rawItem)
        {
            RawData = rawItem;
            DescriptiveData = Mappings.DescriptiveItems.GetByBlueprint(rawItem.Blueprint);
        }

    }
}

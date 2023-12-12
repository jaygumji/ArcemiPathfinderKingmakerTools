using System;
using System.Linq;

namespace Arcemi.Models
{
    public class VendorTableModel : RefModel
    {
        public VendorTableModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName(IGameResourcesProvider res) => res.Blueprints.GetNameOrBlueprint(Table);
        public string Table { get => A.Value<string>(); set => A.Value(value); }
        public ListAccessor<VendorTableEntryModel> Entries => A.List(factory: a => new VendorTableEntryModel(a));
        public ListValueAccessor<string> Loot => A.ListValue<string>();
        public ListAccessor<KeyValuePairModel<int>> KnownItems => A.List(factory: a => new KeyValuePairModel<int>(a));

        public bool HasItem(string blueprint)
        {
            return Entries.Any(e => string.Equals(e.Item, blueprint, StringComparison.Ordinal));
        }
    }
}
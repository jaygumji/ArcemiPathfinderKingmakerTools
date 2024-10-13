namespace Arcemi.Models
{
    public class SelectableFeat
    {
        public IBlueprintMetadataEntry Entry { get; }
        public FeatSpec Spec { get; }

        public SelectableFeat(IBlueprintMetadataEntry entry)
        {
            Entry = entry;
        }

        public SelectableFeat(FeatSpec spec)
        {
            Spec = spec;
        }

        public string Id => Entry is object ? Entry.Id : Spec.Id;
        public string DisplayName => Entry is object ? Entry.DisplayName : Spec?.Name;
        public string Tooltip => Entry is object ? string.Concat(Entry.Name.Original, " (", Entry.Id, ")") : Spec?.Blueprint;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SelectableFeat other) return string.Equals(Id, other.Id, System.StringComparison.Ordinal);
            return ReferenceEquals(this, obj);
        }
    }
}

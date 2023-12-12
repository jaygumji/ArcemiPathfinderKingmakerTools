namespace Arcemi.Models
{
    public class BlueprintMetadataEntry : IBlueprintMetadataEntry, IBlueprintMetadataEntrySetup
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string TypeFullName { get; set; }
        public string Path { get; set; }

        string IBlueprintMetadataEntry.Id => Guid;

        public BlueprintType Type { get; private set; }
        BlueprintType IBlueprintMetadataEntrySetup.Type { get => Type; set => Type = value; }

        private BlueprintName _name;
        BlueprintName IBlueprintMetadataEntry.Name => _name;

        BlueprintName IBlueprintMetadataEntrySetup.Name { get => _name; set => _name = value; }

        public string DisplayName => ((IBlueprintMetadataEntry)this).Name.DisplayName;
    }

    internal interface IBlueprintMetadataEntrySetup
    {
        BlueprintType Type { get; set; }
        BlueprintName Name { get; set; }
    }
}

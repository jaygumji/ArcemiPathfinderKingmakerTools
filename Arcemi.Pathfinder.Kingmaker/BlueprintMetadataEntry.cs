namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintMetadataEntry : IBlueprintMetadataEntry
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string TypeFullName { get; set; }
        public string Path { get; set; }

        string IBlueprintMetadataEntry.Id => Guid;

        private BlueprintType _type;
        public BlueprintType Type => _type ?? (_type = BlueprintTypes.Resolve(TypeFullName));

        private BlueprintName _name;
        BlueprintName IBlueprintMetadataEntry.Name => _name ?? (_name = BlueprintName.Detect(Guid, ((IBlueprintMetadataEntry)this).Type, Name));

        public string DisplayName => ((IBlueprintMetadataEntry)this).Name.DisplayName;
    }
}

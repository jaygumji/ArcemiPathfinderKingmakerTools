namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintEntry : IBlueprint
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string TypeFullName { get; set; }
        string IBlueprint.Id => Guid;

        private BlueprintType _type;
        public BlueprintType Type => _type ?? (_type = BlueprintTypes.Resolve(TypeFullName));

        private BlueprintName _name;
        BlueprintName IBlueprint.Name => _name ?? (_name = BlueprintName.Detect(Guid, ((IBlueprint)this).Type, Name));

        public string DisplayName => ((IBlueprint)this).Name.DisplayName;
    }
}

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ResourceMapping : IBlueprint
    {
        public string Id { get; set; }
        public ResourceMappingType Type { get; set; }
        public string Name { get; set; }

        string IBlueprint.TypeFullName
        {
            get {
                switch (Type) {
                    case ResourceMappingType.ArmyUnit:
                        return BlueprintTypes.Unit;
                    default: return null;
                }
            }
        }

        string IBlueprint.DisplayName => Name;
    }

    public enum ResourceMappingType
    {
        ArmyUnit,
        Feature
    }
}
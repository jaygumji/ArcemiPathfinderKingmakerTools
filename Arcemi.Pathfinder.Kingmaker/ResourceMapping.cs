namespace Arcemi.Pathfinder.Kingmaker
{
    public class ResourceMapping
    {
        public string Id { get; set; }
        public ResourceMappingType Type { get; set; }
        public string Name { get; set; }
    }

    public enum ResourceMappingType
    {
        ArmyUnit,
        Feature
    }
}
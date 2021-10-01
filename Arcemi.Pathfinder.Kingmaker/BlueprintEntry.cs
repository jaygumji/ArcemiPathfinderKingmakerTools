namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintEntry : IBlueprint
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string TypeFullName { get; set; }
        string IBlueprint.Id => Guid;
        public string DisplayName => BlueprintDisplayName.Process(this);
    }
}

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IBlueprint
    {
        string Id { get; }
        BlueprintName Name { get; }
        BlueprintType Type { get; }
        string DisplayName { get; }
    }
}

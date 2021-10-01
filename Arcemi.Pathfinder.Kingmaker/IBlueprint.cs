namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IBlueprint
    {
        string Id { get; }
        string Name { get; }
        string TypeFullName { get; }
        string DisplayName { get; }
    }
}

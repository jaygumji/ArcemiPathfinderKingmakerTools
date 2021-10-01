using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class BlueprintDisplayName
    {
        public static string Process(IBlueprint blueprint)
        {
            if (string.Equals(blueprint.TypeFullName, BlueprintTypes.Unit, StringComparison.OrdinalIgnoreCase)) {
                return Unit(blueprint.Name);
            }
            if (string.Equals(blueprint.TypeFullName, BlueprintTypes.Feature, StringComparison.OrdinalIgnoreCase)) {
                return Feature(blueprint.Name);
            }
            return blueprint.Name.AsDisplayable();
        }

        public static string Unit(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (name.StartsWith("Army", StringComparison.Ordinal)) {
                name = name.Substring(4);
            }
            return name.AsDisplayable();
        }

        public static string Feature(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (name.EndsWith("Feature", StringComparison.Ordinal)) {
                name = name.Remove(name.Length - 7, 7);
            }
            return name.AsDisplayable();
        }
    }
}

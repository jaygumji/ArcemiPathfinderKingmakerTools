using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class BlueprintDisplayName
    {
        public static string Process(IBlueprint blueprint)
        {
            if (string.Equals(blueprint.TypeFullName, BlueprintTypes.Unit, StringComparison.OrdinalIgnoreCase)) {
                return Transform(blueprint.Name, prefix: "Army");
            }
            if (string.Equals(blueprint.TypeFullName, BlueprintTypes.Feature, StringComparison.OrdinalIgnoreCase)) {
                return Transform(blueprint.Name, suffix: "Feature");
            }
            if (string.Equals(blueprint.TypeFullName, BlueprintTypes.AbilityResource, StringComparison.OrdinalIgnoreCase)) {
                return Transform(blueprint.Name, suffix: "Resource");
            }
            return blueprint.Name.AsDisplayable();
        }

        public static string Transform(string name, string prefix = null, string suffix = null)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (prefix != null && name.StartsWith(prefix, StringComparison.Ordinal)) {
                name = name.Substring(prefix.Length);
            }
            if (suffix != null && name.EndsWith(suffix, StringComparison.Ordinal)) {
                name = name.Remove(name.Length - suffix.Length, suffix.Length);
            }
            return name.AsDisplayable();
        }
    }
}

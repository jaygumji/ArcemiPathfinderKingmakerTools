using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintName
    {
        public BlueprintName(BlueprintType type, string displayName, string original)
        {
            Type = type;
            DisplayName = displayName;
            Original = original;
        }

        public BlueprintType Type { get; }
        public string DisplayName { get; }
        public string Original { get; }

        public static BlueprintName Detect(BlueprintType type, string name)
        {
            if (type.Category == BlueprintTypeCategory.Item) {
                return BlueprintItemName.Detect(type, name);
            }
            if (ReferenceEquals(type, BlueprintTypes.Unit)) {
                return Simple(type, name, prefix: "Army");
            }
            if (ReferenceEquals(type, BlueprintTypes.Feature)) {
                return Simple(type, name, suffix: "Feature");
            }
            if (ReferenceEquals(type, BlueprintTypes.AbilityResource)) {
                return Simple(type, name, suffix: "Resource");
            }
            if (ReferenceEquals(type, BlueprintTypes.UnitAsksList)) {
                return Simple(type, name, suffix: "Barks");
            }
            if (ReferenceEquals(type, BlueprintTypes.Unit)) {
                return Simple(type, name, suffix: "Companion");
            }
            return new BlueprintName(type, name.AsDisplayable(), name);
        }

        private static BlueprintName Simple(BlueprintType type, string name, string prefix = null, string suffix = null)
        {
            return new BlueprintName(type, Transform(name, prefix, suffix), name);
        }

        private static string Transform(string name, string prefix = null, string suffix = null)
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

        public bool StartsWith(string value)
        {
            return Original.StartsWith(value, StringComparison.Ordinal);
        }
    }
}
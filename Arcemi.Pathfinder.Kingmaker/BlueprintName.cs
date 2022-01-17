using System;
using System.Collections.Generic;
using System.Linq;

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

        public static BlueprintName Detect(string id, BlueprintType type, string name)
        {
            if (type.Category == BlueprintTypeCategory.Item) {
                return BlueprintItemName.Detect(id, type, name);
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
            if (ReferenceEquals(type, BlueprintTypes.RaceVisualPreset)) {
                return RParts(type, name, "Visual", "Preset");
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

        public static BlueprintName RParts(BlueprintType type, string name, params string[] removableParts)
        {
            var parts = name.AsDisplayable().Split(' ');
            var list = (IList<string>)removableParts;
            var displayName = string.Join(" ", parts.Where(p => !string.IsNullOrEmpty(p) && !list.Contains(p)));
            return new BlueprintName(type, displayName, name);
        }

        public bool StartsWith(string value)
        {
            return Original.StartsWith(value, StringComparison.Ordinal);
        }
    }
}
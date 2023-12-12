using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
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

        public static BlueprintName Detect(IBlueprintTypeProvider typeProvider, string id, BlueprintType type, string name)
        {
            if (type.Category == BlueprintTypeCategory.Item) {
                return BlueprintItemName.Detect(id, type, name);
            }
            if (ReferenceEquals(type, typeProvider.Get(BlueprintTypeId.Unit))) {
                return new BlueprintName(type, Transform(name, suffix: new[] { "Army", "Companion" }), name);
            }
            if (ReferenceEquals(type, typeProvider.Get(BlueprintTypeId.Feature))) {
                return Simple(type, name, suffix: "Feature");
            }
            if (ReferenceEquals(type, typeProvider.Get(BlueprintTypeId.AbilityResource))) {
                return Simple(type, name, suffix: "Resource");
            }
            if (ReferenceEquals(type, typeProvider.Get(BlueprintTypeId.UnitAsksList))) {
                return Simple(type, name, suffix: "Barks");
            }
            if (ReferenceEquals(type, typeProvider.Get(BlueprintTypeId.RaceVisualPreset))) {
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
            return Transform(name, prefix.HasValue() ? new[] { prefix } : null, suffix.HasValue() ? new[] { suffix } : null);
        }
        private static string Transform(string name, string[] prefix = null, string[] suffix = null)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (prefix != null) {
                for (var i = 0; i < prefix.Length; i++) {
                    var p = prefix[i];
                    if (name.StartsWith(p, StringComparison.Ordinal)) {
                        name = name.Substring(p.Length);
                        break;
                    }
                }
            }
            if (suffix != null) {
                for (var i = 0; i < suffix.Length; i++) {
                    var s = suffix[i];
                    if (name.EndsWith(s, StringComparison.Ordinal)) {
                        name = name.Remove(name.Length - s.Length, s.Length);
                        break;
                    }
                }
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
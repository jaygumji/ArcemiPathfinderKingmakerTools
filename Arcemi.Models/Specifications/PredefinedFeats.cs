using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class FeatSpec : IBlueprintMetadataEntry
    {
        private readonly BlueprintName _name;
        public FeatSpec(string name, string blueprint, BlueprintType type, IReadOnlyDictionary<string, string> parameters)
        {
            Id = CreateIdentifier(blueprint, parameters.Select(p => p.Value));
            Name = name;
            Blueprint = blueprint;
            Parameters = parameters;
            _name = new BlueprintName(type, name, name);
        }

        public static string CreateIdentifier(string blueprint, params string[] paraValues) => CreateIdentifier(blueprint, (IEnumerable<string>)paraValues);
        private static string CreateIdentifier(string blueprint, IEnumerable<string> paraValues)
        {
            var actParaValues = paraValues.Where(v => v.HasValue()).ToArray();
            return actParaValues.Length > 0 ? string.Concat(blueprint, '_', string.Join("_", actParaValues)) : blueprint;
        }

        public string Id { get; }
        public string Name { get; }
        public string Blueprint { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        BlueprintName IBlueprintMetadataEntry.Name => _name;
        BlueprintType IBlueprintMetadataEntry.Type => _name.Type;
        string IBlueprintMetadataEntry.DisplayName => Name;
        string IBlueprintMetadataEntry.Path => null;

        public void ApplyOn(JObject obj)
        {
            obj["Blueprint"] = Blueprint;
            obj.Add("m_Context", new JObject {
                { "AssociatedBlueprint", Blueprint }
            });
            if (Parameters.Count > 0) {
                var param = new JObject();
                obj.Add("Param", param);

                foreach (var p in Parameters) {
                    param.Add(p.Key, p.Value);
                }
            }
        }

        public static FeatSpec New(string name, string blueprint, BlueprintType type, string paraName, string paraValue)
        {
            return new FeatSpec(name, blueprint, type, new Dictionary<string, string> { { paraName, paraValue } });
        }
    }
}

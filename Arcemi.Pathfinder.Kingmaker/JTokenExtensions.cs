using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class JTokenExtensions
    {
        public static JToken Export(this JToken token, bool deep, bool incSys)
        {
            if (token == null || token.Type == JTokenType.Null) {
                return JToken.FromObject(null);
            }
            if (token.Type == JTokenType.Array) {
                var arr = new JArray();
                foreach (var item in token) {
                    if (!deep && item.Type == JTokenType.Object) {
                        continue;
                    }
                    arr.Add(Export(item, deep, incSys));
                }
                return arr;
            }
            if (token.Type == JTokenType.Object) {
                var obj = new JObject();
                foreach (var property in ((JObject)token).Properties()) {
                    if (!incSys && property.IsSystemProperty()) {
                        continue;
                    }
                    if (!deep && property.Type == JTokenType.Object) continue;

                    obj.Add(property.Name, Export(property.Value, deep, incSys));
                }
                return obj;
            }
            return token;
        }

        public static void ImportTo(this JToken src, JToken dest, ImportOptions options)
        {
            if (src == null || src.Type == JTokenType.Null) return;
            if (dest == null || dest.Type == JTokenType.Null) return;
            if (src.Type == JTokenType.Array && dest.Type == JTokenType.Array) {
                var destArr = (JArray)dest;
                if (options.Arrays == ImportArrayOptions.Replace) { destArr.Clear(); }
                var i = 0;
                foreach (var item in src) {
                    if (!options.Deep && item.Type == JTokenType.Object) continue;
                    if (options.Arrays == ImportArrayOptions.Overwrite && i < destArr.Count) {
                        destArr[i] = item;
                    }
                    else {
                        destArr.Add(item);
                    }
                }
                return;
            }
            if (src.Type == JTokenType.Object && dest.Type == JTokenType.Object) {
                var destObj = (JObject)dest;
                if (options.Objects == ImportObjectOptions.Replace) {
                    var destObjPropertyNames = destObj.Properties()
                        .Where(p => !p.Value.IsReference() && (options.IncludeSystemProperties || !p.IsSystemProperty()))
                        .Select(p => p.Name).ToArray();
                    foreach (var destName in destObjPropertyNames) {
                        destObj.Remove(destName);
                    }
                }
                foreach (var srcProp in ((JObject)src).Properties()) {
                    if (!options.IncludeSystemProperties && srcProp.IsSystemProperty()) {
                        continue;
                    }

                    if (!options.Deep && srcProp.Type == JTokenType.Object) continue;

                    var destProp = destObj.Property(srcProp.Name);
                    if (destProp == null) {
                        destObj.Add(srcProp.Name, srcProp.Value);
                    }
                    else if (srcProp.Value.IsReference()) {
                        ImportTo(srcProp.Value, destProp.Value, options);
                    }
                    else {
                        destProp.Value = srcProp.Value;
                    }
                }
            }
        }

        public static bool IsReference(this JToken token)
        {
            if (token == null) return false;
            return token.Type == JTokenType.Object || token.Type == JTokenType.Array;
        }

        public static bool IsSystemProperty(this JProperty property)
        {
            return property.Name.StartsWith("$", StringComparison.Ordinal);
        }
    }
}

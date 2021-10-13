using Newtonsoft.Json.Linq;
using System;

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
                    if (!incSys && property.Name.StartsWith("$", StringComparison.Ordinal)) {
                        continue;
                    }
                    if (!deep && property.Type == JTokenType.Object) continue;

                    obj.Add(property.Name, Export(property.Value, deep, incSys));
                }
                return obj;
            }
            return token;
        }

        public static void ImportTo(this JToken src, JToken dest, bool deep, bool incSys, MergeArrayHandling arrayHandling)
        {
            if (src == null || src.Type == JTokenType.Null) return;
            if (dest == null || dest.Type == JTokenType.Null) return;
            if (src.Type == JTokenType.Array && dest.Type == JTokenType.Array) {
                if (arrayHandling == MergeArrayHandling.Replace) { ((JArray)dest).Clear(); }
                foreach (var item in src) {
                    if (!deep && item.Type == JTokenType.Object) continue;
                    ((JArray)dest).Add(item);
                }
                return;
            }
            if (src.Type == JTokenType.Object && dest.Type == JTokenType.Object) {
                var destObj = (JObject)dest;
                foreach (var srcProp in ((JObject)src).Properties()) {
                    if (!incSys && srcProp.Name.StartsWith("$", StringComparison.Ordinal)) {
                        continue;
                    }

                    if (!deep && srcProp.Type == JTokenType.Object) continue;

                    var destProp = destObj.Property(srcProp.Name);
                    if (destProp == null) {
                        destObj.Add(srcProp.Name, srcProp.Value);
                    }
                    else if (srcProp.Type == JTokenType.Object || srcProp.Type == JTokenType.Integer) {
                        ImportTo(srcProp.Value, destProp.Value, deep, incSys, arrayHandling);
                    }
                    else {
                        destProp.Value = srcProp.Value;
                    }
                }
            }
        }
    }
}

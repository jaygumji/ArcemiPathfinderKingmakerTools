using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public static class JObjectExtensions
    {
        public static bool RemovePath(this JObject obj, params string[] path)
        {
            if (path.Length == 0) throw new ArgumentException("Atleast one path part must be supplied");
            var target = obj;
            string propertyName;
            for (var i = 0; i < path.Length - 1; i++) {
                propertyName = path[i];
                if (!target.TryGetValue(propertyName, out var token)) {
                    return false;
                }
                if (token == null || token.Type == JTokenType.Null) return false;
                target = token as JObject;
                if (target == null) return false;
            }

            propertyName = path[path.Length - 1];
            return target.Remove(propertyName);
        }
    }
}

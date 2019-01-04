#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintIdentifier : TextIdentifier
    {
        public BlueprintIdentifier(string value) : base(value)
        {
        }

        public static BlueprintIdentifier From(JProperty property)
        {
            var value = property.Value.Value<string>();
            return new BlueprintIdentifier(value);
        }

        public static BlueprintIdentifier From(JObject obj)
        {
            var property = obj.Property("Blueprint");
            if (property == null) return new BlueprintIdentifier(null);
            return From(property);
        }
    }
}
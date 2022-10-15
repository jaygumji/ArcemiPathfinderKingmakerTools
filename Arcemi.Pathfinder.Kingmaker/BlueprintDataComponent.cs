using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintDataComponent: Model
    {
        public BlueprintDataComponent(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name { get => A.Value<string>("name"); set => A.Value(value, "name"); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }

        public static BlueprintDataComponent Factory(ModelDataAccessor accessor)
        {
            return new BlueprintDataComponent(accessor);
        }
    }
}

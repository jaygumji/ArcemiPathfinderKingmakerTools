using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintData : Model
    {
        public BlueprintData(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintDataComponent> Components => A.List<BlueprintDataComponent>(factory: BlueprintDataComponent.Factory);

        public static BlueprintData Factory(ModelDataAccessor modelDataAccessor)
        {
            return new BlueprintData(modelDataAccessor);
        }
    }
}

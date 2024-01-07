using System.Collections.Generic;

namespace Arcemi.Models
{
    public class EmptyBlueprintTypeProvider : BlueprintProvider
    {
        public EmptyBlueprintTypeProvider() : base(new Dictionary<string, BlueprintType>(), new Dictionary<BlueprintTypeId, BlueprintType>())
        {
            
        }
    }
}

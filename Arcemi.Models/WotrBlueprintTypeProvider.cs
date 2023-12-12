using Arcemi.Models.PathfinderWotr;
using System;

namespace Arcemi.Models
{
    public class EmptyBlueprintTypeProvider : IBlueprintTypeProvider
    {
        private readonly WotrBlueprintTypeProvider _fallback;

        public EmptyBlueprintTypeProvider()
        {
            _fallback = new WotrBlueprintTypeProvider();
        }
        public BlueprintType Get(BlueprintTypeId id)
        {
            return _fallback.Get(id);
        }

        public BlueprintType Get(string fullName)
        {
            return _fallback.Get(fullName);
        }
    }
}

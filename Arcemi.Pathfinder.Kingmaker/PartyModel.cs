using System;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PartyModel : RefModel
    {
        public PartyModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<UnitEntityModel> UnitEntities => A.List<UnitEntityModel>("m_EntityData");

        public UnitEntityModel Find(string uniqueId)
        {
            return UnitEntities.FirstOrDefault(e => string.Equals(e.UniqueId, uniqueId, StringComparison.Ordinal));
        }

    }
}

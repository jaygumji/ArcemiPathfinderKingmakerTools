using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerGlobalMapsModel : RefModel
    {
        public PlayerGlobalMapsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<PlayerArmyModel> Armies => A.List<PlayerArmyModel>("m_Armies", a => new PlayerArmyModel(a));
    }
}
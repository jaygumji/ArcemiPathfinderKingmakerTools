using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerKingdomChangeModel : RefModel
    {
        public PlayerKingdomChangeModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<int> Changes => A.ListValue<int>("m_Changes");
        public int BPPerTurn { get => A.Value<int>(); set => A.Value(value); }
        public int BPOneTime { get => A.Value<int>(); set => A.Value(value); }
    }
}
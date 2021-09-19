using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerLeadersManagerModel : RefModel
    {
        public PlayerLeadersManagerModel(ModelDataAccessor a) : base(a)
        {
        }

        public int RecruitedLeaders { get => A.Value<int>("m_recruitedLeaders"); set => A.Value(value, "m_recruitedLeaders"); }
        public IReadOnlyList<PlayerLeaderModel> Leaders => A.List<PlayerLeaderModel>("m_Leaders", a => new PlayerLeaderModel(a));
        public bool RecruitmentIsForbidden { get => A.Value<bool>("m_RecruitmentIsForbidden"); set => A.Value(value, "m_RecruitmentIsForbidden"); }

    }
}
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class PlayerLeadersManagerModel : RefModel
    {
        public PlayerLeadersManagerModel(ModelDataAccessor a) : base(a)
        {
        }

        public int RecruitedLeaders { get => A.Value<int>("m_recruitedLeaders"); set => A.Value(value, "m_recruitedLeaders"); }
        public ListAccessor<PlayerLeaderModel> Leaders => A.List<PlayerLeaderModel>("m_Leaders", a => new PlayerLeaderModel(a));
        public bool RecruitmentIsForbidden { get => A.Value<bool>("m_RecruitmentIsForbidden"); set => A.Value(value, "m_RecruitmentIsForbidden"); }

    }
}
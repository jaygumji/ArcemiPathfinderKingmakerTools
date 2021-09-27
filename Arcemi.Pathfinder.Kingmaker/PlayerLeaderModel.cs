namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerLeaderModel : RefModel, IModelWithFaction
    {
        private Portraits _portraits;

        public PlayerLeaderModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => Mappings.GetLeaderName(BlueprintRef);

        public void Init(Portraits portraits)
        {
            _portraits = portraits;
        }

        public string PortraitPath
        {
            get {
                if (string.IsNullOrEmpty(BlueprintRef)) {
                    return _portraits.GetUnknownUri();
                }
                return _portraits.GetLeaderPortraitUri(BlueprintRef);
            }
        }

        public string BlueprintRef { get => A.Value<string>("m_BlueprintRef"); set => A.Value(value, "m_BlueprintRef"); }
        public ListValueAccessor<string> Skills => A.ListValue<string>("m_Skills");
        public ListValueAccessor<string> RitualSlots => A.ListValue<string>("m_RitualSlots");
        public int Level { get => A.Value<int>("m_Level"); set => A.Value(value, "m_Level"); }
        public int Experience { get => A.Value<int>("m_Experience"); set => A.Value(value, "m_Experience"); }
        public PlayerLeaderNameModel Name => A.Object("m_LeaderName", a => new PlayerLeaderNameModel(a));
        public string LeaderGuid => A.Value<string>("m_LeaderGuid");
        public PlayerLeaderStatsModel Stats => A.Object("m_Stats", a => new PlayerLeaderStatsModel(a));
        public string Faction { get => A.Value<string>("m_Faction"); set => A.Value(value, "m_Faction"); }
        public string NextAvailableTime { get => A.Value<string>("m_NextAvailableTime"); set => A.Value(value, "m_NextAvailableTime"); }
    }
}
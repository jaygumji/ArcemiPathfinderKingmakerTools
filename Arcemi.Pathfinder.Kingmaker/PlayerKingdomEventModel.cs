namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerKingdomEventModel : RefModel
    {
        public PlayerKingdomEventModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string EventBlueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Region { get => A.Value<string>(); set => A.Value(value); }
        public string LocationUniqueId { get => A.Value<string>("m_LocationUniqueId"); set => A.Value(value, "m_LocationUniqueId"); }
        public int StartedOn {
            get => A.Value<int>("m_StartedOn");
            set {
                A.Value(value, "m_StartedOn");
                if (HistoryEntry != null) {
                    HistoryEntry.TriggeredOn = value;
                }
            }
        }
        public bool IsPlanned { get => A.Value<bool>("m_IsPlanned"); set => A.Value(value, "m_IsPlanned"); }
        public int SuccessCount { get => A.Value<int>("m_SuccessCount"); set => A.Value(value, "m_SuccessCount"); }
        public int DCModifier { get => A.Value<int>(); set => A.Value(value); }
        public bool IsFinished { get => A.Value<bool>(); set => A.Value(value); }
        public bool ContinueRecurrentSolution { get => A.Value<bool>(); set => A.Value(value); }
        public int IgnoredOn { get => A.Value<int>(); set => A.Value(value); }
        public bool CheckTriggerOnStart { get => A.Value<bool>(); set => A.Value(value); }

        public PlayerKingdomEventHistoryModel HistoryEntry => A.Object<PlayerKingdomEventHistoryModel>("m_HistoryEntry");
    }
}
namespace Arcemi.Models
{
    public class SettlementStateModel : RefModel
    {
        public SettlementStateModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string LocationRef { get => A.Value<string>("m_LocationRef"); set => A.Value(value, "m_LocationRef"); }
        public string Level { get => A.Value<string>("m_Level"); set => A.Value(value, "m_Level"); }
        public string Name { get => A.Value<string>("m_Name"); set => A.Value(value, "m_Name"); }
        public string BlueprintRef { get => A.Value<string>("m_BlueprintRef"); set => A.Value(value, "m_BlueprintRef"); }
        public bool HasWaterSlot { get => A.Value<bool>("m_HasWaterSlot"); set => A.Value(value, "m_HasWaterSlot"); }
    }
}
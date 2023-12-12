using System;

namespace Arcemi.Models
{
    public class UnitWearinessPartItemModel : PartItemModel
    {
        public const string TypeRef = "Models.UnitLogic.Parts.UnitPartWeariness, Assembly-CSharp";
        public UnitWearinessPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public double ExtraWearinessHours { get => A.Value<double>("m_ExtraWearinessHours"); set => A.Value(value, "m_ExtraWearinessHours"); }
        public TimeSpan LastStackTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeSpan LastBuffApplyTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public int SuppressCount { get => A.Value<int>("m_SuppressCount"); set => A.Value(value, "m_SuppressCount"); }
        public int WearinessStacks { get => A.Value<int>(); set => A.Value(value); }
    }
}
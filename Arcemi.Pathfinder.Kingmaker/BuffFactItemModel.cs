using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BuffFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Buffs.Buff, Assembly-CSharp";
        public BuffFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int RoundNumber { get => A.Value<int>(); set => A.Value(value); }
        public TimeSpan TickTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeSpan NextTickTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeSpan EndTime { get => A.Value<TimeSpan>("m_EndTime"); set => A.Value(value, "m_EndTime"); }
        public bool IsFromSpell { get => A.Value<bool>(); set => A.Value(value); }
        public TimeSpan Duration
        {
            get {
                if (EndTime == TimeSpan.Zero || AttachTime == TimeSpan.Zero) return TimeSpan.Zero;
                var duration = EndTime.Subtract(AttachTime);
                if (duration < TimeSpan.Zero) return TimeSpan.Zero;
                return duration;
            }
            set {
                EndTime = AttachTime + value;
            }
        }

        private TimeParts _durationParts;
        public TimeParts DurationParts => _durationParts ?? (_durationParts = new TimeParts(() => Duration, v => Duration = v));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
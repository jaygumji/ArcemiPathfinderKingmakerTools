using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
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
        
        public DurationProvider GetDurationProvider(IGameTimeProvider gameTimeProvider)
        {
            return new DurationProvider(() => EndTime, v => EndTime = v, gameTimeProvider);
        }

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
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

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class W40KRTBuffFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Buffs.Buff, Code";

        public W40KRTBuffFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Rank { get => A.Value<int>(); set => A.Value(value); }
        public int RoundNumber { get => A.Value<int>(); set => A.Value(value); }
        public bool PlayedFirstHitSound { get => A.Value<bool>(); set => A.Value(value); }

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
            //obj.Add(nameof(Rank), 0);
            //obj.Add(nameof(RoundNumber), 0);
            //obj.Add(nameof(PlayedFirstHitSound), false);
        }
    }
}
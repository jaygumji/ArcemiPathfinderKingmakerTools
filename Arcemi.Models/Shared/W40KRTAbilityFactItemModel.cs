using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class W40KRTAbilityFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Abilities.Ability, Code";

        public W40KRTAbilityFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        //public bool m_Hidden { get => A.Value<bool>(); set => A.Value(value); }
        //public int UsagesPerDayResource { get => A.Value<int>(); set => A.Value(value); }

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
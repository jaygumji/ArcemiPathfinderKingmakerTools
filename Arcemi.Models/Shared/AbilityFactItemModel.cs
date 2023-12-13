using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public class AbilityFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Abilities.Ability, Assembly-CSharp";
        public AbilityFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public AbilityDataModel Data => A.Object(factory: a => new AbilityDataModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
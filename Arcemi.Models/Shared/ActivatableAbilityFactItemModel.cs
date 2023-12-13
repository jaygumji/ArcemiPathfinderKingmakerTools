using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public class ActivatableAbilityFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbility, Assembly-CSharp";
        public ActivatableAbilityFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ActivatableAbilityDataModel m_AppliedBuff => A.Object(factory: a => new ActivatableAbilityDataModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}
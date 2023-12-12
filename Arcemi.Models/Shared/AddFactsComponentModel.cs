using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Models
{
    public class AddFactsComponentModel : ComponentModel
    {
        public const string TypeRef = "Models.EntitySystem.EntityFactComponentDelegate`2+ComponentRuntime[[Models.EntitySystem.Entities.UnitEntityData, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[Models.UnitLogic.FactLogic.AddFactsData, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        public AddFactsComponentModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public AddFactsComponentDataModel Data => A.Object("m_Data", a => new AddFactsComponentDataModel(a));

        public static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add("m_Data", refs.Create());
        }
    }
}

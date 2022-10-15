using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class AddFactsComponentModel : ComponentModel
    {
        public const string TypeRef = "Kingmaker.EntitySystem.EntityFactComponentDelegate`2+ComponentRuntime[[Kingmaker.EntitySystem.Entities.UnitEntityData, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[Kingmaker.UnitLogic.FactLogic.AddFactsData, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

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

using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class RuntimeComponentModel : ComponentModel
    {
        public const string TypeRef = "Kingmaker.EntitySystem.EntityFactComponentDelegate`2+ComponentRuntime[[Kingmaker.Items.ItemEntity, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[Kingmaker.Designers.Mechanics.EquipmentEnchants.AddUnitFactEquipmentData, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        public RuntimeComponentModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public RuntimeComponentDataModel Data => A.Object("m_Data", a => new RuntimeComponentDataModel(a));

        public static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add("m_Data", refs.Create());
        }
    }
}
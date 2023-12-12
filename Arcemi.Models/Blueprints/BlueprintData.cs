using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Models
{
    public class BlueprintData : Model
    {
        public BlueprintData(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintDataComponent> Components => A.List<BlueprintDataComponent>(factory: BlueprintDataComponent.Factory);
        public int Ranks { get => A.Value<int>(); set => A.Value(value); }
        public ListValueAccessor<string> Enchantments => A.ListValue<string>("m_Enchantments");
        public string WeaponComponent { get => A.Value<string>("m_WeaponComponent"); set => A.Value(value, "m_WeaponComponent"); }
        public string ArmorComponent { get => A.Value<string>("m_ArmorComponent"); set => A.Value(value, "m_ArmorComponent"); }

        public static BlueprintData Factory(ModelDataAccessor modelDataAccessor)
        {
            return new BlueprintData(modelDataAccessor);
        }
    }
}

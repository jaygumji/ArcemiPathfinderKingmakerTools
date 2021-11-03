using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class AbilityFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Abilities.Ability, Assembly-CSharp";
        public AbilityFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }
        public AbilityDataModel Data => A.Object(factory: a => new AbilityDataModel(a));
    }
}
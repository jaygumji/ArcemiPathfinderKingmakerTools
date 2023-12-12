namespace Arcemi.Models
{
    public class AbilityExecutionParentContextModel : ParentContextModel
    {
        public AbilityExecutionParentContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<int> Ranks => A.ListValue<int>("m_Ranks");
        public AbilityExecutionParamModel Params => A.Object<AbilityExecutionParamModel>("m_Params");
        public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
    }
}
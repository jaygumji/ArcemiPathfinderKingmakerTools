namespace Arcemi.Pathfinder.Kingmaker
{
    public class AbilityExecutionParamModel : RefModel
    {
        public AbilityExecutionParamModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CasterLevel { get => A.Value<int>(); set => A.Value(value); }
        public int DC { get => A.Value<int>(); set => A.Value(value); }
        public int Concentration { get => A.Value<int>(); set => A.Value(value); }
        public string Metamagic { get => A.Value<string>(); set => A.Value(value); }
    }
}
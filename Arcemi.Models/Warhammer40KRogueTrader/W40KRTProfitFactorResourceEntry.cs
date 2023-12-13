namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTProfitFactorResourceEntry : RefModel, IGamePartyResourceEntry
    {
        public W40KRTProfitFactorResourceEntry(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name => "Profit Factor";
        public int Value { get => A.Value<int>("InitialValue"); set => A.Value(value, "InitialValue"); }
        public bool IsSmall => true;
        public bool IsReadOnly => false;
    }
}
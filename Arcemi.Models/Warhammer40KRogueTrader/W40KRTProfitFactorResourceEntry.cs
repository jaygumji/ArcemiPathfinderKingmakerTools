namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTProfitFactorResourceEntry : RefModel, IGameDataInteger
    {
        public W40KRTProfitFactorResourceEntry(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Label => "Profit Factor";
        public int Value { get => A.Value<int>("InitialValue"); set => A.Value(value, "InitialValue"); }
        public GameDataSize Size => GameDataSize.Small;
        public bool IsReadOnly => false;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
    }
}
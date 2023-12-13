namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTScrapResourceEntry : RefModel, IGamePartyResourceEntry
    {
        public W40KRTScrapResourceEntry(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string Name => "Scrap";
        public int Value { get => A.Value<int>("m_Value"); set => A.Value(value, "m_Value"); }
        public bool IsSmall => false;
        public bool IsReadOnly => false;
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTScrapResourceEntry : RefModel, IGameDataInteger
    {
        public W40KRTScrapResourceEntry(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string Label => "Scrap";
        public int Value { get => A.Value<int>("m_Value"); set => A.Value(value, "m_Value"); }
        public GameDataSize Size => GameDataSize.Medium;
        public bool IsReadOnly => false;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
    }
}
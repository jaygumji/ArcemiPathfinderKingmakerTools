namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTNavigatorResourceEntry : IGameDataInteger
    {
        public W40KRTNavigatorResourceEntry(RefModel refModel)
        {
            Ref = refModel;
        }

        public string Label => "Nav Insights";

        public int Value { get => A?.Value<int>("NavigatorResource") ?? 0; set => A?.Value(value, "NavigatorResource"); }

        public GameDataSize Size => GameDataSize.Small;
        public bool IsReadOnly => false;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;

        public RefModel Ref { get; }
        private ModelDataAccessor A => Ref?.GetAccessor();
    }
}
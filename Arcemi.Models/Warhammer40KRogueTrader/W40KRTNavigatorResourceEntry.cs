namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTNavigatorResourceEntry : IGamePartyResourceEntry
    {
        public W40KRTNavigatorResourceEntry(RefModel refModel)
        {
            Ref = refModel;
        }

        public string Name => "Navigation";

        public int Value { get => A?.Value<int>("NavigatorResource") ?? 0; set => A?.Value(value, "NavigatorResource"); }

        public bool IsSmall => true;

        public bool IsReadOnly => Ref is null;

        public RefModel Ref { get; }
        private ModelDataAccessor A => Ref?.GetAccessor();
    }
}
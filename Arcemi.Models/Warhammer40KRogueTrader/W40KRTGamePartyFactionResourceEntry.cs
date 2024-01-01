namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyFactionResourceEntry : IGameDataInteger
    {
        public W40KRTGamePartyFactionResourceEntry(KeyValuePairModel<int> model)
        {
            Model = model;
        }

        public KeyValuePairModel<int> Model { get; }

        public string Label
        {
            get {
                switch (Model.Key.ToLowerInvariant()) {
                    case "shipvendor": return "Imperial Navy";
                    default:
                        return Model.Key;
                }
            }
        }
        public int Value { get => Model.Value; set => Model.Value = value; }
        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
        public GameDataSize Size => GameDataSize.Small;
        public bool IsReadOnly => false;
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyFactionResourceEntry : IGamePartyResourceEntry
    {
        public W40KRTGamePartyFactionResourceEntry(KeyValuePairModel<int> model)
        {
            Model = model;
        }

        public KeyValuePairModel<int> Model { get; }

        public string Name
        {
            get {
                switch (Model.Key.ToLowerInvariant()) {
                    case "pirates": return "Void";
                    case "shipvendor": return "Imperial Navy";
                    default:
                        return Model.Key;
                }
            }
        }
        public int Value { get => Model.Value; set => Model.Value = value; }
        public bool IsSmall => true;
        public bool IsReadOnly => false;
    }
}
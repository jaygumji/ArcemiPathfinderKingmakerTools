namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTSoulMarkFactSection : IGameUnitFactSection
    {
        public W40KRTSoulMarkFactSection(UnitEntityModel model)
        {
            Ref = model;
            Items = new GameModelCollection<IGameUnitFactEntry, FactItemModel>(model.Facts.Items, x => new W40KRTGameUnitSoulMarkEntry(x),
                x => x.Type.Eq("Kingmaker.UnitLogic.SoulMark, Code"), new W40KRTGameUnitSoulMarkCollectionWriter(model.UniqueId));
        }

        public string Name => "Soul Mark";
        public IGameModelCollection<IGameUnitFactEntry> Items { get; }
        public UnitEntityModel Ref { get; }
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSelectionProgressionEntry : IGameUnitSelectionProgressionEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameUnitSelectionProgressionEntry(UnitProgressionSelectionOfPartModel selection)
        {
            Ref = selection;
        }

        public string Name => NameWithout(Res.Blueprints.GetNameOrBlueprint(Ref.Selection), "Selection");
        public string Feature => NameWithout(Res.Blueprints.GetNameOrBlueprint(Ref.Feature), Name);
        public UnitProgressionSelectionOfPartModel Ref { get; }

        private string NameWithout(string name, string suffix)
        {
            if (name == null) return name;

            suffix = " " + suffix;
            if (name.IEnd(suffix)) return name.Remove(name.Length - suffix.Length);

            return name;
        }
    }
}
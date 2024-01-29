namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStatsAttributesEntry : IGameUnitStatsEntry
    {
        public KingmakerGameStatsAttributesEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.PairedValue; set => Model.PairedValue = value; }
        public string Info => Model.ModifiersSum;
    }
}
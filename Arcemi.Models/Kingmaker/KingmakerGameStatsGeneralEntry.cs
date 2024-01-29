namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStatsGeneralEntry : IGameUnitStatsEntry
    {
        public KingmakerGameStatsGeneralEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.PairedValue; set => Model.PairedValue = value; }
        public string Info => Model.TotalValue.ToString();
    }
}
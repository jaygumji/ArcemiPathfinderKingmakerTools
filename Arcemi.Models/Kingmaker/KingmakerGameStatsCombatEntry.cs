namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStatsCombatEntry : IGameUnitStatsEntry
    {
        public KingmakerGameStatsCombatEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.BaseValue; set => Model.BaseValue = value; }
        public string Info => Model.PermanentValue.ToString();
    }
}
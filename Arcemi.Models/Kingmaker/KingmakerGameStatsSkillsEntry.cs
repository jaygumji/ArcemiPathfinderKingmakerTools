namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStatsSkillsEntry : IGameUnitStatsEntry
    {
        public KingmakerGameStatsSkillsEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.BaseValue; set => Model.BaseValue = value; }
        public string Info => Model.PermanentValue.ToString();
    }
}
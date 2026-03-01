namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameStatsGeneralEntry : IGameUnitStatsEntry
    {
        public WotrGameStatsGeneralEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.PairedValue; set => Model.PairedValue = value; }
        public string Info => Model.TotalValue.ToString();
        public bool IsReadOnly => false;
    }
    internal class WotrGameStatsAttributesEntry : IGameUnitStatsEntry
    {
        public WotrGameStatsAttributesEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.PairedValue; set => Model.PairedValue = value; }
        public string Info => Model.ModifiersSum;
        public bool IsReadOnly => false;
    }
    internal class WotrGameStatsSkillsEntry : IGameUnitStatsEntry
    {
        public WotrGameStatsSkillsEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.BaseValue; set => Model.BaseValue = value; }
        public string Info => Model.PermanentValue.ToString();
        public bool IsReadOnly => false;
    }
    internal class WotrGameStatsCombatEntry : IGameUnitStatsEntry
    {
        public WotrGameStatsCombatEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.BaseValue; set => Model.BaseValue = value; }
        public string Info => Model.PermanentValue.ToString();
        public bool IsReadOnly => false;
    }
    internal class WotrGameStatsSavesEntry : IGameUnitStatsEntry
    {
        public WotrGameStatsSavesEntry(CharacterAttributeModel model)
        {
            Model = model;
        }

        public CharacterAttributeModel Model { get; }

        public string Name => Model.Name;
        public int Value { get => Model.BaseValue; set => Model.BaseValue = value; }
        public string Info => Model.PermanentValue.ToString();
        public bool IsReadOnly => false;
    }
}

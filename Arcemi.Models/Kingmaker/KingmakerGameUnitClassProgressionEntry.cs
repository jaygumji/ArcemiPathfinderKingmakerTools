namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitClassProgressionEntry : IGameUnitClassProgressionEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitClassProgressionEntry(ClassModel model)
        {
            Model = model;
        }

        public ClassModel Model { get; }
        public string Name => Res.GetClassTypeName(Model.CharacterClass);
        public string SpecializationName => Res.GetClassArchetypeName(Model.Archetypes);
        public int Level => Model.Level;
    }
}
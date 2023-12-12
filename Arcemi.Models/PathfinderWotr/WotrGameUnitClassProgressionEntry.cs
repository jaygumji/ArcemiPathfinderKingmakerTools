namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitClassProgressionEntry : IGameUnitClassProgressionEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameUnitClassProgressionEntry(ClassModel model)
        {
            Model = model;
        }

        public ClassModel Model { get; }
        public string Name => Res.GetClassTypeName(Model.CharacterClass);
        public string SpecializationName => Res.GetClassArchetypeName(Model.Archetypes);
        public int Level => Model.Level;
    }
}
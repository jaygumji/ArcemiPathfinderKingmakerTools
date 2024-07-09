namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitAbilityEntry : IGameUnitAbilityEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameUnitAbilityEntry(FactItemModel model)
        {
            Model = model;
        }

        public FactItemModel Model { get; }
        public string DisplayName => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
    }
}
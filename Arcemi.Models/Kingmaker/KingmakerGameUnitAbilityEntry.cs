namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAbilityEntry : IGameUnitAbilityEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitAbilityEntry(FactItemModel model)
        {
            Model = (AbilityFactItemModel)model;
        }

        public AbilityFactItemModel Model { get; }
        public string DisplayName => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
    }
}
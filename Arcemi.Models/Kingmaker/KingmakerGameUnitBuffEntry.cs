namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitBuffEntry : IGameUnitBuffEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitBuffEntry(FactItemModel model)
        {
            Model = (BuffFactItemModel)model;
        }

        public BuffFactItemModel Model { get; }

        public string DisplayName => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
        public TimeParts Duration => Model.DurationParts;
    }
}
namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitBuffEntry : IGameUnitBuffEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitBuffEntry(FactItemModel model, IGameTimeProvider gameTimeProvider)
        {
            Model = (BuffFactItemModel)model;
            DurationProvider = Model.GetDurationProvider(gameTimeProvider);
        }

        public BuffFactItemModel Model { get; }
        public DurationProvider DurationProvider { get; }

        public string DisplayName => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
        public TimeParts Duration => DurationProvider.DurationParts;
    }
}
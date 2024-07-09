using System;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitBuffEntry : IGameUnitBuffEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameUnitBuffEntry(FactItemModel model, IGameTimeProvider gameTimeProvider)
        {
            Model = model;
            DurationProvider = Model is BuffFactItemModel buff
                ? buff.GetDurationProvider(gameTimeProvider)
                : new DurationProvider(() => model.GetAccessor().Value<TimeSpan>("EndTime"), v => model.GetAccessor().Value(v, "EndTime"), gameTimeProvider);
        }

        public FactItemModel Model { get; }
        public DurationProvider DurationProvider { get; }

        public string DisplayName => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => Model.Blueprint;
        public bool IsActive { get => Model.IsActive; set => Model.IsActive = value; }
        public TimeParts Duration => DurationProvider.DurationParts;
    }
}
using System;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitBuffEntry : IGameUnitBuffEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameUnitBuffEntry(FactItemModel model)
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
namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitAlignmentHistoryEntryModel : IGameUnitAlignmentHistoryEntryModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public string Direction => Model.Direction;
        public string Provider => Res.Blueprints.GetNameOrBlueprint(Model.Provider);
        public int X => Model.Vector.X;
        public int Y => Model.Vector.Y;

        public WotrGameUnitAlignmentHistoryEntryModel(AlignmentHistoryModel model)
        {
            Model = model;
        }

        public AlignmentHistoryModel Model { get; }
    }
}
namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAlignmentHistoryEntryModel : IGameUnitAlignmentHistoryEntryModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public string Direction => Model.Direction;
        public string Provider => Res.Blueprints.GetNameOrBlueprint(Model.Provider);
        public int X => Model.Vector.X;
        public int Y => Model.Vector.Y;

        public KingmakerGameUnitAlignmentHistoryEntryModel(AlignmentHistoryModel model)
        {
            Model = model;
        }

        public AlignmentHistoryModel Model { get; }
    }
}
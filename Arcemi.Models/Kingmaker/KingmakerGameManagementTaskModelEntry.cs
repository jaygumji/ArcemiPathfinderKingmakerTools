namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameManagementTaskModelEntry : IGameManagementTaskModelEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;

        public KingmakerGameManagementTaskModelEntry(RefModel @ref)
        {
            Ref = @ref;
            A = @ref.GetAccessor();
            EvtA = A.Object<RefModel>("Event").GetAccessor();
        }

        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }
        public ModelDataAccessor EvtA { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(EvtA.Value<string>("EventBlueprint"));
        public string Description => "";
        public string Type => A.Object<PlayerKingdomLeaderModel>("AssignedLeader")?.Type;
        public int StartedOn { get => A.Value<int>(); set => A.Value(value); }
    }
}
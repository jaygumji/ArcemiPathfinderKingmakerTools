namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameManagementAttributeCollectionWriter : GameModelCollectionWriter<IGameDataObject, PlayerKingdomStatsAttributeModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
        }

        public override bool IsAddEnabled => false;
        public override bool IsRemoveEnabled => false;
    }
}
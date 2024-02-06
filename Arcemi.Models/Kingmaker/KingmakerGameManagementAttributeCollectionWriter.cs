namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameManagementAttributeCollectionWriter : GameModelCollectionWriter<IGameDataObject, PlayerKingdomStatsAttributeModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
        }

        public override bool IsAddEnabled => false;
        public override bool IsRemoveEnabled => false;
    }
}
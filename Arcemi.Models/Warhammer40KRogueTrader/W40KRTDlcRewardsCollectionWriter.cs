namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTDlcRewardsCollectionWriter : GameModelCollectionWriter<IGameDataObject, RefModel>
    {
        private readonly PlayerModel player;
        public override bool IsAddEnabled => false;

        public W40KRTDlcRewardsCollectionWriter(PlayerModel player)
        {
            this.player = player;
        }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
        }

        public override void AfterRemove(AfterRemoveCollectionItemArgs<IGameDataObject, RefModel> args)
        {
            player.GetAccessor().ListValue<string>("ClaimedDlcRewards").Remove(args.Model.GetAccessor().Value<string>("guid"));
        }
    }
}
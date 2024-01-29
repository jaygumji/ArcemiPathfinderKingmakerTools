namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStateModel : IGameStateModel
    {
        public KingmakerGameStateModel(PlayerModel player)
        {
            Ref = player;
        }

        public PlayerModel Ref { get; }

        public bool IsSupported => false;
    }
}
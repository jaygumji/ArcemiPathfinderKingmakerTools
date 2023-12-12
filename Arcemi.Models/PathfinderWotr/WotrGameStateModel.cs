namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameStateModel : IGameStateModel
    {
        public WotrGameStateModel(PlayerModel player)
        {
            Player = player;
        }

        public PlayerModel Player { get; }

        public bool IsSupported => true;
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameStateModel : IGameStateModel
    {
        public W40KRTGameStateModel(PlayerModel player)
        {
            Player = player;
        }

        public PlayerModel Player { get; }

        public bool IsSupported => false;
    }
}
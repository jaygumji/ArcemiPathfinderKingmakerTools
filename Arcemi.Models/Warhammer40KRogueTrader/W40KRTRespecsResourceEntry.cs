namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTRespecsResourceEntry : IGamePartyResourceEntry
    {
        public W40KRTRespecsResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Name => "Respecs";
        public int Value { get => Player.RespecsUsed; set => Player.RespecsUsed = value; }
        public bool IsSmall => true;
        public bool IsReadOnly => false;
    }
}
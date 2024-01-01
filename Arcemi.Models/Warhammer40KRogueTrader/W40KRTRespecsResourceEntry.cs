namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTRespecsResourceEntry : IGameDataInteger
    {
        public W40KRTRespecsResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Label => "Respecs";
        public int Value { get => Player.RespecsUsed; set => Player.RespecsUsed = value; }
        public GameDataSize Size => GameDataSize.Small;
        public bool IsReadOnly => false;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
    }
}
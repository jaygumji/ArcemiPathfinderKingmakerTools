namespace Arcemi.Models.Kingmaker
{
    public class KingmakerGamePartyModel : IGamePartyModel
    {
        public KingmakerGamePartyModel(PlayerModel player)
        {
            Player = player;
            Data = GameDataModels.Object(new IGameData[] {
                new KingmakerMoneyResourceEntry(player),
                new KingmakerRespecsResourceEntry(player)
            });
        }

        public PlayerModel Player { get; }
        public IGameDataObject Data { get; }
        public bool IsSupported => true;

    }
    internal class KingmakerMoneyResourceEntry : IGameDataInteger
    {
        public KingmakerMoneyResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Label => "Money";
        public int Value { get => Player.Money; set => Player.Money = value; }
        public GameDataSize Size => GameDataSize.Medium;
        public bool IsReadOnly => false;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
    }
    internal class KingmakerRespecsResourceEntry : IGameDataInteger
    {
        public KingmakerRespecsResourceEntry(PlayerModel player)
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
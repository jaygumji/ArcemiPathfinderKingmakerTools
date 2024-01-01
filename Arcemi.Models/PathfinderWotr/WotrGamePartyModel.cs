using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGamePartyModel : IGamePartyModel
    {
        public WotrGamePartyModel(PlayerModel player)
        {
            Player = player;
            Data = GameDataModels.Object(new IGameData[] {
                new WotrMoneyResourceEntry(player),
                new WotrCorruptionResourceEntry(player),
                new WotrRespecsResourceEntry(player)
            });
        }

        public PlayerModel Player { get; }
        public IGameDataObject Data { get; }
        public bool IsSupported => true;
    }

    internal class WotrMoneyResourceEntry : IGameDataInteger
    {
        public WotrMoneyResourceEntry(PlayerModel player)
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

    internal class WotrCorruptionResourceEntry : IGameDataInteger
    {
        public WotrCorruptionResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Label => "Corruption";
        public int Value { get => Player.Corruption?.CurrentValue ?? 0; set => Player.Corruption.CurrentValue = value; }
        public bool IsReadOnly => Player.Corruption is null;
        public GameDataSize Size => GameDataSize.Small;

        public int MinValue => 0;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
    }

    internal class WotrRespecsResourceEntry : IGameDataInteger
    {
        public WotrRespecsResourceEntry(PlayerModel player)
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
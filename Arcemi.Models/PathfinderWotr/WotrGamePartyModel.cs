using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGamePartyModel : IGamePartyModel
    {
        public WotrGamePartyModel(PlayerModel player)
        {
            Player = player;
            Resources = new IGamePartyResourceEntry[] {
                new WotrMoneyResourceEntry(player),
                new WotrCorruptionResourceEntry(player),
                new WotrRespecsResourceEntry(player)
            };
        }

        public PlayerModel Player { get; }

        public IReadOnlyList<IGamePartyResourceEntry> Resources { get; }

        public bool IsSupported => true;
    }

    internal class WotrMoneyResourceEntry : IGamePartyResourceEntry
    {
        public WotrMoneyResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Name => "Money";
        public int Value { get => Player.Money; set => Player.Money = value; }
        public bool IsSmall => false;
        public bool IsReadOnly => false;
    }

    internal class WotrCorruptionResourceEntry : IGamePartyResourceEntry
    {
        public WotrCorruptionResourceEntry(PlayerModel player)
        {
            Player = player;
        }
        public PlayerModel Player { get; }
        public string Name => "Corruption";
        public int Value { get => Player.Corruption?.CurrentValue ?? 0; set => Player.Corruption.CurrentValue = value; }
        public bool IsSmall => true;
        public bool IsReadOnly => Player.Corruption is null;
    }

    internal class WotrRespecsResourceEntry : IGamePartyResourceEntry
    {
        public WotrRespecsResourceEntry(PlayerModel player)
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
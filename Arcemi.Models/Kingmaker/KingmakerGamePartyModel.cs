using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    public class KingmakerGamePartyModel : IGamePartyModel
    {
        public KingmakerGamePartyModel(PlayerModel player, IGameModelCollection<IGameUnitModel> characters)
        {
            Player = player;
            var wearinessDebuffBlueprints = new HashSet<string>(StringComparer.Ordinal) {
                "e6f2fc5d73d88064583cb828801212f4", // Fatigued
                "46d1b9cc3d0fd36469a471b047d773a2", // Exhausted
            };
            Data = GameDataModels.Object(new IGameData[] {
                new KingmakerMoneyResourceEntry(player),
                new KingmakerRespecsResourceEntry(player),
                GameDataModels.Action("Reset Weariness", () => {
                    foreach (KingmakerGameUnitModel character in characters) {
                        if (character.Weariness is object) {
                            character.Weariness.WearinessStacks = 0;
                            if (character.Weariness.ExtraWearinessHours > 0)
                                character.Weariness.ExtraWearinessHours = 0;
                            character.Weariness.LastStackTime = player.GameTime;
                            character.Weariness.LastBuffApplyTime = player.GameTime;
                        }

                        // Remove the debuffs
                        var debuffs = character.Buffs.Where(b => wearinessDebuffBlueprints.Contains(b.Blueprint)).ToArray();
                        foreach (var debuff in debuffs) {
                            character.Buffs.Remove(debuff);
                        }
                    }
                }, isEnabled: () => characters.Cast<KingmakerGameUnitModel>().Any(c => c.Weariness?.WearinessStacks > 0))
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
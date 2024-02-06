using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementModel : IGameManagementModel
    {
        public WotrGameManagementModel(PlayerModel player)
        {
            // Where(l => l.IsFactionCrusaders()) ?? Array.Empty<PlayerLeaderModel>()
            Ref = player;
            if (player.Kingdom is null) return;
            Members = new GameModelCollection<IGameManagementMemberModelEntry, PlayerLeaderModel>(player.LeadersManager?.Leaders, a => new WotrGameManagementMemberModelEntry(a), l => l.IsFactionCrusaders());
            Armies = player?.GlobalMaps?.SelectMany(m => m.Armies).Where(a => a.Data.IsFactionCrusaders()).Select(a => new WotrGameManagementArmyModelEntry(a)).ToArray();
            Tasks = player.Kingdom?.Leaders?.Where(l => l.AssignedTask is object).Select(l => new WotrGameManagementTaskModelEntry(l.AssignedTask)).ToArray();
            Places = GameDataModels.Object("Settlements", new IGameData[] {
                GameDataModels.List("Settlement", player.Kingdom?.SettlementsManager?.SettlementStates, a => GameDataModels.Object(a.Name, new IGameData[] {
                    new WotrGameManagementSettlementLevelDataEntry(a)
                }))
            });
            Overview = GameDataModels.Object("Crusade", new IGameData[] {
                GameDataModels.Integer("Current turn", player.Kingdom, k => k.CurrentTurn, (k, v) => k.CurrentTurn = v),
                GameDataModels.Integer("Current day", player.Kingdom, k => k.CurrentDay, (k, v) => k.CurrentDay = v),
                GameDataModels.Object("Attributes", new IGameData[] {
                    GameDataModels.RowList(player.Kingdom.Stats.Attributes, a => GameDataModels.Object(a.Type, new IGameData[] {
                        GameDataModels.Integer("Rank", a, x => x.Rank, (x, v) => x.Rank = v, minValue: 0, size: GameDataSize.Small),
                        GameDataModels.Integer("Value", a, x => x.Value, (x, v) => x.Value = v, minValue: 0),
                    }), writer: new WotrGameManagementAttributeCollectionWriter())
                }),
                GameDataModels.Object("Morale", new IGameData[] {
                    GameDataModels.Integer("Current", player.Kingdom.MoraleState, m => m.CurrentValue, (m, v) => m.CurrentValue = v),
                    GameDataModels.Integer("Min", player.Kingdom.MoraleState, m => m.MinValue),
                    GameDataModels.Integer("Max", player.Kingdom.MoraleState, m => m.MaxValue),
                }),
                GameDataModels.Object("Resources", new IGameData[] {
                    GameDataModels.Integer("Finances", player.Kingdom.Resources, m => m.Finances, (m, v) => m.Finances = v),
                    GameDataModels.Integer("Materials", player.Kingdom.Resources, m => m.Materials, (m, v) => m.Materials = v),
                    GameDataModels.Integer("Energy", player.Kingdom.Resources, m => m.Favors, (m, v) => m.Favors = v),
                })
            });
        }

        public PlayerModel Ref { get; }

        public string DisplayName => "Crusade";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Generals", "General");

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; } = GameModelCollection<IGameManagementMemberModelEntry>.Empty;
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = Array.Empty<IGameManagementArmyModelEntry>();
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = Array.Empty<IGameManagementTaskModelEntry>();
        public IGameDataObject Overview { get; }
        public IGameDataObject Resources { get; }
        public IGameDataObject Places { get; }

        public bool IsSupported => Ref.Kingdom is object;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameManagementModel : IGameManagementModel
    {
        public KingmakerGameManagementModel(PlayerModel player, IReadOnlyList<IGameUnitModel> units)
        {
            Ref = player;
            if (player.Kingdom is null) return;
            Resources = GameDataModels.Object("Kingdom", new IGameData[] {
                GameDataModels.Text("Name", player.Kingdom, k => k.KingdomName, (k, v) => k.KingdomName = v),
                GameDataModels.Options("Unrest", new [] {"Crumbling", "Unrest", "Troubled", "Worried", "Stable", "Metastable"}, player.Kingdom, k => k.Unrest, (k, v) => k.Unrest = v),
                GameDataModels.Integer("Current turn", player.Kingdom, k => k.CurrentTurn, (k,v) => k.CurrentTurn = v, minValue: 0),
                GameDataModels.Integer("Start day", player.Kingdom, k => k.StartDay, (k,v) => k.StartDay = v, minValue: 0),
                GameDataModels.Integer("Current day", player.Kingdom, k => k.CurrentDay, (k,v) => k.CurrentDay = v, minValue: 0),
                GameDataModels.Integer("BP", player.Kingdom, k => k.BP, (k,v) => k.BP = v, minValue: 0),
                GameDataModels.Integer("BP per turn", player.Kingdom, k => k.BPPerTurn, (k, v) => k.BPPerTurn = v, maxValue: 0),
                GameDataModels.Object("Attributes", new IGameData[] {
                    GameDataModels.RowList(player.Kingdom.Stats.Attributes, a => GameDataModels.Object(a.Type, new IGameData[] {
                        GameDataModels.Integer("Rank", a, x => x.Rank, (x, v) => x.Rank = v, minValue: 0, size: GameDataSize.Small),
                        GameDataModels.Integer("Value", a, x => x.Value, (x, v) => x.Value = v, minValue: 0),
                    }), writer: new KingmakerGameManagementAttributeCollectionWriter())
                })
            });

            Members = new GameModelCollection<IGameManagementMemberModelEntry, PlayerKingdomLeaderModel>(player.Kingdom.GetAccessor().List<PlayerKingdomLeaderModel>("Leaders"),
                a => new KingmakerGameManagementMemberModelEntry(a, units));

            Tasks = new GameModelCollection<IGameManagementTaskModelEntry, RefModel>(player.Kingdom.GetAccessor().List<RefModel>("ActiveTasks"),
                a => new KingmakerGameManagementTaskModelEntry(a), r => r.GetAccessor().Object<RefModel>("AssignedLeader") is object);
        }

        public string DisplayName => "Kingdom";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Leaders", "Leader");

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; } = Array.Empty<IGameManagementArmyModelEntry>();
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; } = Array.Empty<IGameManagementTaskModelEntry>();

        public IGameDataObject Overview { get; }
        public IGameDataObject Resources { get; }
        public IGameDataObject Places { get; }

        public bool IsSupported => Ref.Kingdom is object;

        public PlayerModel Ref { get; }
    }
}
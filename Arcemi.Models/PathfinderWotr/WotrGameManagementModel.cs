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
            Members = new GameModelCollection<IGameManagementMemberModelEntry, PlayerLeaderModel>(player.LeadersManager?.Leaders, a => new WotrGameManagementMemberModelEntry(a), l => l.IsFactionCrusaders());
            Armies = player?.GlobalMaps?.SelectMany(m => m.Armies).Where(a => a.Data.IsFactionCrusaders()).Select(a => new WotrGameManagementArmyModelEntry(a)).ToArray();
            Tasks = player.Kingdom?.Leaders?.Where(l => l.AssignedTask is object).Select(l => new WotrGameManagementTaskModelEntry(l.AssignedTask)).ToArray();
            Places = GameDataModels.Object("Settlements", new IGameData[] {
                GameDataModels.List("Settlement", player.Kingdom?.SettlementsManager?.SettlementStates, a => GameDataModels.Object(a.Name, new IGameData[] {
                    new WotrGameManagementSettlementLevelDataEntry(a)
                }))
            });
        }

        public PlayerModel Ref { get; }

        public string DisplayName => "Crusade";
        public ModelTypeName MemberTypeName { get; } = new ModelTypeName("Generals", "General");

        public bool IsOverviewEnabled => true;
        public bool IsMembersEnabled => true;
        public bool IsTasksEnabled => true;
        public bool IsArmiesEnabled => true;

        public IGameModelCollection<IGameManagementMemberModelEntry> Members { get; }
        public IReadOnlyList<IGameManagementArmyModelEntry> Armies { get; }
        public IReadOnlyList<IGameManagementTaskModelEntry> Tasks { get; }
        public IGameDataObject Resources { get; }
        public IGameDataObject Places { get; }

        public bool IsSupported => Ref.Kingdom is object;
    }
}
namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementMemberModelEntry : IGameManagementMemberModelEntry
    {
        public WotrGameManagementMemberModelEntry(PlayerLeaderModel model)
        {
            Model = model;
            Overview = GameDataModels.Object(new IGameData[] {
                GameDataModels.Text("Blueprint", model, m => m.BlueprintRef),
                GameDataModels.Text("Identifier", model, m => m.LeaderGuid),
                GameDataModels.Integer("Experience", model, m => m.Experience, (m, v) => m.Experience = v),
                GameDataModels.Integer("Level", model, m => m.Level, (m, v) => m.Level = v),
                GameDataModels.Integer("CurrentMana", model, m => m.Stats.CurrentMana, (m, v) => m.Stats.CurrentMana = v),
            });
        }

        public PlayerLeaderModel Model { get; }
        private IGameResourcesProvider Res => GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public string Name => Res.GetLeaderName(Model.BlueprintRef);
        public string UniqueId => Model.LeaderGuid;

        public string PortraitPath
        {
            get {
                if (string.IsNullOrEmpty(Model.BlueprintRef)) {
                    return Res.AppData.Portraits.GetUnknownUri();
                }
                return Res.AppData.Portraits.GetPortraitsUri(Res.GetPortraitId(Model.BlueprintRef));
            }
        }

        public string Blueprint => Model.BlueprintRef;
        public IGameDataObject Overview { get; }
    }
}
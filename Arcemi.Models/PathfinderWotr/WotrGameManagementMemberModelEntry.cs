namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementMemberModelEntry : IGameManagementMemberModelEntry
    {
        public WotrGameManagementMemberModelEntry(PlayerLeaderModel model)
        {
            Model = model;
        }

        public PlayerLeaderModel Model { get; }
        private IGameResourcesProvider Res => GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public string Name => Res.GetLeaderName(Model.BlueprintRef);

        public string PortraitPath
        {
            get {
                if (string.IsNullOrEmpty(Model.BlueprintRef)) {
                    return Res.AppData.Portraits.GetUnknownUri();
                }
                return Res.AppData.Portraits.GetPortraitsUri(Res.GetPortraitId(Model.BlueprintRef));
            }
        }

        public string UniqueId => Model.LeaderGuid;
        public string Blueprint => Model.BlueprintRef;
        public int Experience { get => Model.Experience; set => Model.Experience = value; }
        public int Level { get => Model.Level; set => Model.Level = value; }
        public int CurrentMana { get => Model.Stats.CurrentMana; set => Model.Stats.CurrentMana = value; }
    }
}
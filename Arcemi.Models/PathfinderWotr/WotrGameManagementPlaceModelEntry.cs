namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementPlaceModelEntry : IGameManagementPlaceModelEntry
    {
        public WotrGameManagementPlaceModelEntry(SettlementStateModel model)
        {
            Model = model;
        }

        public SettlementStateModel Model { get; }
    }
}
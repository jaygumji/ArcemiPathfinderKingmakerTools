namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementTaskModelEntry : IGameManagementTaskModelEntry
    {
        public WotrGameManagementTaskModelEntry(PlayerKingdomTaskModel model)
        {
            Model = model;
        }

        public PlayerKingdomTaskModel Model { get; }
    }
}
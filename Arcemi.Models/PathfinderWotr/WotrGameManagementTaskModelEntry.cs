namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementTaskModelEntry : IGameManagementTaskModelEntry
    {
        public WotrGameManagementTaskModelEntry(PlayerKingdomTaskModel model)
        {
            Model = model;
        }

        public PlayerKingdomTaskModel Model { get; }

        public string Name => Model.Name;
        public string Description => Model.Description;
        public string Type => Model.AssignedLeader?.Type;
        public int StartedOn { get => Model.StartedOn; set => Model.StartedOn = value; }
    }
}
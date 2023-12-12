namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameManagementArmyModelEntry : IGameManagementArmyModelEntry
    {
        public WotrGameManagementArmyModelEntry(PlayerArmyModel model)
        {
            Model = model;
        }

        public PlayerArmyModel Model { get; }
    }
}
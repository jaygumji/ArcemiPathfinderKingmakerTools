namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitProgressionMythicModel : IGameUnitUltimateProgressionEntry
    {
        public WotrGameUnitProgressionMythicModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public UnitEntityModel Unit { get; }
        public string Name => "Mythic";
        public int Level { get => Unit.Descriptor.Progression.MythicExperience; set => Unit.Descriptor.Progression.MythicExperience = value; }
        public int MinLevel => 0;
        public int MaxLevel => 10;
    }
}
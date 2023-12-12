namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitRaceModel : IGameUnitRaceModel
    {
        public W40KRTGameUnitRaceModel(UnitProgressionPartItemModel progression)
        {
            Progression = progression;
        }

        public UnitProgressionPartItemModel Progression { get; }
        public string DisplayName => Progression.Race;
        public bool IsSupported => false;
    }
}
namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSpellCasterModel : IGameUnitSpellCasterModel
    {
        public W40KRTGameUnitSpellCasterModel(IGameUnitModel owner, UnitEntityModel @ref)
        {
            
        }

        public IGameUnitSpellCasterBonusSpellModel BonusSpells { get; }
        public IGameModelCollection<IGameUnitSpellBookEntry> SpellBooks { get; } = GameModelCollection<IGameUnitSpellBookEntry>.Empty;
        public bool IsSupported => false;
    }
}
namespace Arcemi.Models.Accessors
{
    public interface ICustomSpellModel
    {
        string Blueprint { get; }
        int DecorationColorNumber { get; set; }
        int DecorationBorderNumber { get; set; }
        int SpellLevelCost { get; set; }
        int HeightenLevel { get; set; }
        MetamagicCollection Metamagic { get; }
        string MetamagicMask { get; set; }
    }
}

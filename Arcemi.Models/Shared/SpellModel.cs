using Arcemi.Models.Accessors;

namespace Arcemi.Models
{
    public class MemorizedSpellReferenceModel : RefModel, ICustomSpellModel
    {
        public MemorizedSpellReferenceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public int DecorationColorNumber { get => A.Value<int>(); set => A.Value(value); }
        public int DecorationBorderNumber { get => A.Value<int>(); set => A.Value(value); }
        public CustomSpellMetamagicDataModel MetamagicData => A.Object<CustomSpellMetamagicDataModel>();

        int ICustomSpellModel.SpellLevelCost { get => MetamagicData?.SpellLevelCost ?? 0; set { if (MetamagicData is object) MetamagicData.SpellLevelCost = value; } }
        int ICustomSpellModel.HeightenLevel { get => MetamagicData?.HeightenLevel ?? 0; set { if (MetamagicData is object) MetamagicData.HeightenLevel = value; } }

        MetamagicCollection ICustomSpellModel.Metamagic => MetamagicData?.Metamagic;

        string ICustomSpellModel.MetamagicMask { get => MetamagicData?.MetamagicMask; set { if (MetamagicData is object) MetamagicData.MetamagicMask = value; } }
    }
}
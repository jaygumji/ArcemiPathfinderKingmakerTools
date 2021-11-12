using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class CustomSpellMetamagicDataModel : RefModel
    {
        public CustomSpellMetamagicDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string MetamagicMask { get => A.Value<string>(); set => A.Value(value); }
        public int SpellLevelCost { get => A.Value<int>(); set => A.Value(value); }
        public int HeightenLevel { get => A.Value<int>(); set => A.Value(value); }

        private MetamagicCollection _Metamagic;
        public MetamagicCollection Metamagic => _Metamagic ?? (_Metamagic = new MetamagicCollection(MetamagicMask, v => MetamagicMask = v));
    }
}
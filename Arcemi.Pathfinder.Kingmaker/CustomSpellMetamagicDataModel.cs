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

        private IReadOnlyList<Metamagic> _Metamagic;
        public IReadOnlyList<Metamagic> Metamagic => _Metamagic ?? (_Metamagic = Kingmaker.Metamagic.All(MetamagicMask, v => MetamagicMask = v));
    }
}
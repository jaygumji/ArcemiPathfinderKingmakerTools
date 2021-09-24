using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactContextModel : RefModel
    {
        public FactContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public CharacterModel OwnerDescriptor => A.Object<CharacterModel>("m_OwnerDescriptor");

        public IReadOnlyList<int> Ranks => A.ListValue<int>("m_Ranks");
        public IReadOnlyList<int> SharedValues => A.ListValue<int>("m_SharedValues");
        public string SpellDescriptor { get => A.Value<string>(); set => A.Value(value); }
        public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
        public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }
    }
}
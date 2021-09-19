using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerLeaderAttributeModel : RefModel
    {
        public PlayerLeaderAttributeModel(ModelDataAccessor accessor, string name) : base(accessor)
        {
            Name = name;
        }

        public string Name { get; }
        public int BaseValue { get => A.Value<int>("m_BaseValue"); set => A.Value(value, "m_BaseValue"); }
        public IReadOnlyList<PersistentModifierModel> PersistentModifierList => A.List(factory: a => new PersistentModifierModel(a));
    }
}
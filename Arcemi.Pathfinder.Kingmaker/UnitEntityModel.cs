using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    [SaveFileType("Kingmaker.EntitySystem.Entities.UnitEntityData, Assembly-CSharp")]
    public class UnitEntityModel : RefModel
    {
        public UnitEntityModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId => A.Value<string>();

        public CharacterModel Descriptor => A.Object<CharacterModel>();
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a));

        public IEnumerable<FeatureFactItemModel> FindRankedFeatures()
        {
            return Facts.Items.OfType<FeatureFactItemModel>().Where(f => f.Rank > 0).ToArray();
        }
    }
}

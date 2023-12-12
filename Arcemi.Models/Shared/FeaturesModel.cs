using System.Collections.Generic;

namespace Arcemi.Models
{
    public class FeaturesModel : RefModel
    {
        public FeaturesModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<FactModel> Facts => A.List("m_Facts", a => new FactModel(a));

    }
}
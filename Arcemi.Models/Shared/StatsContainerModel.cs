using System.Collections.Generic;

namespace Arcemi.Models
{
    public class StatsContainerModel : RefModel
    {
        public StatsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<KeyValuePairObjectModel<StatsContainerConverterModel>> ContainerConverter
            => A.List(factory: a => new KeyValuePairObjectModel<StatsContainerConverterModel>(a, valueAccessor => new StatsContainerConverterModel(valueAccessor)));
    }

    public class StatsContainerConverterModel : RefModel
    {
        public StatsContainerConverterModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public int BaseValue { get => A.Value<int>("m_BaseValue"); set => A.Value(value, "m_BaseValue"); }
    }
}
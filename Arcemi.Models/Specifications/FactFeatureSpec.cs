using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class FactFeatureSpec
    {
        public FactFeatureSpec(string blueprint, params string[] components)
        {
            Blueprint = blueprint;
            Components = components.Select(c => new ComponentSpec(c)).ToArray();
        }

        public string Blueprint { get; }
        public IReadOnlyList<ComponentSpec> Components { get; }

        public void AddTo(FeatureFactItemModel model)
        {
            model.Blueprint = Blueprint;
            foreach (var component in Components) {
                component.AddTo(model.Components);
            }
        }
    }
}

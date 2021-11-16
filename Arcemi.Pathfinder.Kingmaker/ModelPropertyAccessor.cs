using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ModelPropertyAccessor<T> : PropertyAccessor<T>
    {
        public ModelPropertyAccessor(ModelDataAccessor a, string name)
            : base(() => a.Value<T>(name), v => a.Value(JToken.FromObject(v), name))
        {
        }
    }
}
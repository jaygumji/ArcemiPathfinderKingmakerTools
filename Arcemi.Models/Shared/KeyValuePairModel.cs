using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class KeyValuePairModel<TValue> : Model
    {
        public KeyValuePairModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName(IGameResourcesProvider res) => res.Blueprints.GetNameOrBlueprint(Key);
        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public TValue Value { get => A.Value<TValue>(); set => A.Value(JToken.FromObject(value)); }
    }
}
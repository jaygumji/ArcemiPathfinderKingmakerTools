using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class KeyValuePairModel<TValue> : Model
    {
        public KeyValuePairModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Key);
        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public TValue Value { get => A.Value<TValue>(); set => A.Value(JToken.FromObject(value)); }
    }
}
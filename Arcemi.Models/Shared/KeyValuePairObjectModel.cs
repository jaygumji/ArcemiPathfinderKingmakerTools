using System;

namespace Arcemi.Models
{
    public class KeyValuePairObjectModel<TModel> : Model
        where TModel : Model
    {
        private readonly Func<ModelDataAccessor, TModel> _factory;

        public KeyValuePairObjectModel(ModelDataAccessor accessor, Func<ModelDataAccessor, TModel> factory) : base(accessor)
        {
            _factory = factory;
        }

        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public TModel Value { get => A.Object<TModel>(factory: _factory); }
    }
}
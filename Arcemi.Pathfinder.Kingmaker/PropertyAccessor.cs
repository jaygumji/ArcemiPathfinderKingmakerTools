using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PropertyAccessor<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public PropertyAccessor(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public T Get() => _getter();
        public void Set(T value) => _setter(value);
    }
}
#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Arcemi.Pathfinder.Kingmaker
{

    public class ModelDataAccessor
    {
        private readonly JObject _obj;
        private readonly IReferences _refs;
        private NotifyChangeTracker _changeTracker;

        public string TypeValue()
        {
            return Value<string>("Type", "$type");
        }

        public void SetChangeTracker(NotifyChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public ModelDataAccessor(JObject obj, IReferences refs)
        {
            _obj = obj;
            _refs = refs;
        }

        public void SetObjectToNull(string name)
        {
            var property = _obj.Property(name);
            if (property == null) {
                return;
            }
            property.Value = null;
            _refs.RemoveObject(_obj, name);
            _changeTracker?.Updated(name, null);
        }

        public T Object<T>([CallerMemberName] string name = null, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var obj = _refs.GetOrCreateObject(_obj, name, factory, createIfNull);
            _changeTracker?.On(name, propertyName);
            return obj;
        }

        public ListValueAccessor<T> ListValue<T>([CallerMemberName] string name = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var listAccessor = _refs.GetOrCreateListValue<T>(_obj, name, createIfNull);
            _changeTracker?.On(name, propertyName);
            return listAccessor;
        }

        public ListAccessor<T> List<T>([CallerMemberName] string name = null, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var listAccessor = _refs.GetOrCreateList(_obj, name, factory, createIfNull);
            _changeTracker?.On(name, propertyName);
            return listAccessor;
        }

        public DictionaryOfValueListAccessor<TValue> DictionaryOfValueList<TValue>([CallerMemberName] string name = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var dictAccessor = _refs.GetOrCreateDictionaryOfValueList<TValue>(_obj, name, createIfNull);
            _changeTracker?.On(name, propertyName);
            return dictAccessor;
        }

        public T Value<T>([CallerMemberName] string name = null, [CallerMemberName] string propertyName = null)
        {
            _changeTracker?.On(name, propertyName);
            var property = _obj.Property(name);
            if (property == null || property.Value is null) {
                return default(T);
            }
            return _obj.Property(name).Value.Value<T>();
        }

        public void Value(JToken value, [CallerMemberName] string name = null)
        {
            if (_obj.Property(name) != null) {
                _obj.Property(name).Value = value;
            }
            else {
                _obj.Add(name, value);
            }
            _changeTracker?.Updated(name, value);
        }

    }
}
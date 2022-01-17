#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IModelDataAccessor
    {
        JObject UnderlyingObject { get; }
    }
    public class ModelDataAccessor : IModelDataAccessor
    {
        private readonly JObject _obj;
        private readonly IReferences _refs;
        private NotifyChangeTracker _changeTracker;

        public IGameResourcesProvider Res { get; }

        JObject IModelDataAccessor.UnderlyingObject => _obj;

        public string TypeValue()
        {
            return Value<string>("$type", "Type");
        }

        public void SetChangeTracker(NotifyChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public ModelDataAccessor(JObject obj, IReferences refs, IGameResourcesProvider res)
        {
            _obj = obj;
            _refs = refs;
            Res = res;
        }

        public string ExportCode()
        {
            var obj = _obj.Export(deep: true);
            var json = obj.ToString(Formatting.None);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        public T CreateImportView<T>(string script, Func<ModelDataAccessor, T> factory = null)
        {
            var bytes = Convert.FromBase64String(script.Trim());
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var obj = JObject.Parse(json);
            var accessor = new ModelDataAccessor(obj, new References(Res), Res);
            return (factory ?? Mappings.GetFactory<T>()).Invoke(accessor);
        }

        public void ImportCode(string script)
        {
            var bytes = Convert.FromBase64String(script.Trim().Trim('\"'));
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var obj = JObject.Parse(json);
            obj.ImportTo(_obj, ImportOptions.CodeScripts);
            _refs.Refresh(_obj);
        }

        public void ShallowMerge(JObject target)
        {
            foreach (var property in _obj.Properties()) {
                if (property.Name == "$id") continue;
                if (property.Value.Type == JTokenType.Array) continue;
                if (property.Value.Type == JTokenType.Object) continue;

                var tp = target.Property(property.Name);
                if (tp != null && !tp.Value.IsNullOrEmpty()) {
                    continue;
                }

                if (tp != null) {
                    if (!tp.Value.IsNullOrEmpty()) continue;
                    tp.Value = property.Value;
                }
                else {
                    target.Add(property.Name, property.Value);
                }
            }
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

        public T NewObject<T>(string name, Action<IReferences, JObject> preparation = null, Func<ModelDataAccessor, T> factory = null)
        {
            var obj = typeof(T).IsSubclassOf(typeof(RefModel))
                ? _refs.Create()
                : new JObject();

            preparation?.Invoke(_refs, obj);

            _obj[name] = obj;

            return Object(name, factory);
        }

        public ListD2Accessor<T> ListD2<T>([CallerMemberName] string name = null, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
            where T : Model
        {
            var listD2Accessor = _refs.GetOrCreateListD2(_obj, name, factory, createIfNull);
            _changeTracker?.On(name, propertyName);
            return listD2Accessor;
        }

        public ListValueAccessor<T> ListValue<T>([CallerMemberName] string name = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var listAccessor = _refs.GetOrCreateListValue<T>(_obj, name, createIfNull);
            _changeTracker?.On(name, propertyName);
            return listAccessor;
        }

        public ListAccessor<T> List<T>([CallerMemberName] string name = null, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
            where T : Model
        {
            var listAccessor = _refs.GetOrCreateList(_obj, name, factory, createIfNull);
            _changeTracker?.On(name, propertyName);
            return listAccessor;
        }

        public DictionaryAccessor<T> Dictionary<T>([CallerMemberName] string name = null, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
            where T : Model
        {
            var dictAccessor = _refs.GetOrCreateDictionary(_obj, name, factory, createIfNull);
            _changeTracker?.On(name, propertyName);
            return dictAccessor;
        }

        public DictionaryOfValueAccessor<TValue> DictionaryOfValue<TValue>([CallerMemberName] string name = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var dictAccessor = _refs.GetOrCreateDictionaryOfValue<TValue>(_obj, name, createIfNull);
            _changeTracker?.On(name, propertyName);
            return dictAccessor;
        }

        public DictionaryOfValueListAccessor<TValue> DictionaryOfValueList<TValue>([CallerMemberName] string name = null, bool createIfNull = false, [CallerMemberName] string propertyName = null)
        {
            var dictAccessor = _refs.GetOrCreateDictionaryOfValueList<TValue>(_obj, name, createIfNull);
            _changeTracker?.On(name, propertyName);
            return dictAccessor;
        }

        public T Value<T>([CallerMemberName] string name = null, [CallerMemberName] string propertyName = null, T defaultValue = default)
        {
            _changeTracker?.On(name, propertyName);
            var property = _obj.Property(name);
            if (property == null || property.Value is null) {
                return defaultValue;
            }
            if (typeof(T) == typeof(TimeSpan)) {
                var str = _obj.Property(name).Value.Value<string>();
                return string.IsNullOrEmpty(str) ? default(T) : (T)(object)TimeSpan.Parse(str);
            }
            return _obj.Property(name).Value.Value<T>();
        }

        public void Value(TimeSpan value, [CallerMemberName] string name = null)
        {
            var token = (JToken)value.ToString();
            Value(token, name);
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
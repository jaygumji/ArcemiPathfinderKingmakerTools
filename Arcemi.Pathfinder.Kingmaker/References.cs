#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{

    public class References : IReferences
    {
        private readonly Dictionary<string, JObject> _lookup;
        private int _maxId;

        private readonly ObjectCache _objects;
        private readonly ObjectCache _lists;
        private readonly ObjectCache _valueLists;
        private readonly ObjectCache _dictionaryOfValueLists;

        private readonly IReferences _refs;

        public References()
        {
            _lookup = new Dictionary<string, JObject>(StringComparer.Ordinal);
            _objects = new ObjectCache();
            _lists = new ObjectCache();
            _valueLists = new ObjectCache();
            _dictionaryOfValueLists = new ObjectCache();
            _refs = this;
        }

        public void Add(string id, JObject obj)
        {
            if (int.TryParse(id, out var intId)) {
                if (_maxId < intId) {
                    _maxId = intId;
                }
            }

            _lookup.Add(id, obj);
        }

        public object CreateObject(JObject jObj, Func<ModelDataAccessor, object> factory)
        {
            var accessor = new ModelDataAccessor(jObj, this);
            var obj = factory.Invoke(accessor);
            var id = jObj.Property("$id").Value.Value<string>();
            _objects.Add(id, obj);
            return obj;
        }

        JObject IReferences.Create()
        {
            var obj = new JObject {
                { "$id", (++_maxId).ToString() }
            };
            return obj;
        }

        JObject IReferences.CreateReference(string refId)
        {
            var obj = new JObject {
                {"$ref", refId}
            };
            return obj;
        }

        JObject IReferences.CreateReference(JObject refObj)
        {
            var refId = refObj.Property("$id").Value.Value<string>();
            return ((IReferences)this).CreateReference(refId);
        }

        bool IReferences.TryGetReferred(string refId, out JObject refObj)
        {
            return _lookup.TryGetValue(refId, out refObj);
        }

        JObject IReferences.GetReferred(JObject obj)
        {
            var refProperty = obj.Property("$ref");
            if (refProperty != null && !(refProperty.Value is null)) {
                var refId = refProperty.Value.Value<string>();
                if (_refs.TryGetReferred(refId, out var refObj)) {
                    return refObj;
                }
                else {
                    throw new InvalidOperationException($"The reference to '{refId}' does not exist.");
                }
            }
            return obj;
        }

        private ModelDataAccessor Get(JObject parent, string name)
        {
            var property = parent.Property(name);
            if (property == null || property.Value is null || property.Value.Type == JTokenType.Null) {
                return null;
            }
            if (!(property.Value is JObject obj)) {
                throw new ArgumentException($"Parameter {name} does not reference a valid reference object.");
            }

            obj = _refs.GetReferred(obj);
            return new ModelDataAccessor(obj, this);
        }

        T IReferences.GetOrCreateObject<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory, bool createIfNull)
        {
            if (_objects.TryGet(parent, name, out T obj)) {
                return obj;
            }

            var property = Get(parent, name);
            if (property == null) {
                if (createIfNull) {
                    var jObj = _refs.Create();
                    if (parent.Property(name) != null) {
                        parent[name] = jObj;
                    }
                    else {
                        parent.Add(name, jObj);
                    }
                    property = new ModelDataAccessor(jObj, this);
                }
                else {
                    return default(T);
                }
            }
            obj = (factory ?? Mappings.GetFactory<T>()).Invoke(property);
            _objects.Add(parent, name, obj);
            return obj;
        }

        ListAccessor<T> IReferences.GetOrCreateList<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory, bool createIfNull)
        {

            if (_lists.TryGet(parent, name, out ListAccessor<T> list)) {
                return list;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid array.");
            }

            if (!(property.Value is JArray arr)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid array.");
                }
                arr = new JArray();
                property.Value = arr;
            }

            var listAccessor = new ListAccessor<T>(arr, _refs, factory ?? Mappings.GetFactory<T>());
            _lists.Add(parent, name, listAccessor);
            return listAccessor;
        }

        ListValueAccessor<T> IReferences.GetOrCreateListValue<T>(JObject parent, string name, bool createIfNull)
        {
            if (_valueLists.TryGet(parent, name, out ListValueAccessor<T> list)) {
                return list;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid array.");
            }

            if (!(property.Value is JArray arr)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid array.");
                }
                arr = new JArray();
                property.Value = arr;
            }

            var listAccessor = new ListValueAccessor<T>(arr);
            _valueLists.Add(parent, name, listAccessor);
            return listAccessor;
        }

        public DictionaryOfValueListAccessor<TValue> GetOrCreateDictionaryOfValueList<TValue>(JObject parent, string name, bool createIfNull = false)
        {
            if (_dictionaryOfValueLists.TryGet(parent, name, out DictionaryOfValueListAccessor<TValue> dict)) {
                return dict;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid object.");
            }

            if (!(property.Value is JObject dictObj)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid object.");
                }
                dictObj = new JObject();
                property.Value = dictObj;
            }

            dict = new DictionaryOfValueListAccessor<TValue>(dictObj);
            _dictionaryOfValueLists.Add(parent, name, dict);
            return dict;
        }

        bool IReferences.RemoveObject(JObject parent, string name)
        {
            return _objects.Remove(parent, name);
        }

        bool IReferences.RemoveList(JObject parent, string name)
        {
            return _lists.Remove(parent, name);
        }

        bool IReferences.RemoveListValue(JObject parent, string name)
        {
            return _valueLists.Remove(parent, name);
        }
    }
}
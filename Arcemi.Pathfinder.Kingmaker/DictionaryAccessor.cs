#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class DictionaryAccessor<T> : IReadOnlyDictionary<string, T>, IModelContainer
        where T : Model
    {
        private readonly JObject _obj;
        private readonly Dictionary<string, T> _dict;
        private readonly IReferences _refs;
        private readonly Func<ModelDataAccessor, T> _factory;
        private readonly IGameResourcesProvider _res;

        public DictionaryAccessor(JObject obj, IReferences refs, IGameResourcesProvider res, Func<ModelDataAccessor, T> factory)
        {
            _obj = obj;
            _dict = new Dictionary<string, T>(StringComparer.Ordinal);
            _refs = refs;
            _factory = factory;
            _res = res;
            ((IModelContainer)this).Refresh();
        }

        void IModelContainer.Refresh()
        {
            if (_dict.Count > 0) _dict.Clear();
            foreach (var property in _obj.Properties()) {
                if (property.Value == null || property.Value is null || property.Value.Type == JTokenType.Null) {
                    _dict.Add(property.Name, null);
                    continue;
                }
                var item = _refs.GetReferred((JObject)property.Value);
                var accessor = new ModelDataAccessor(item, _refs, _res);
                var dictItem = _factory.Invoke(accessor);
                _dict.Add(property.Name, dictItem);
            }
        }

        public T this[string key]
        {
            get => _dict[key];
            set {
                throw new NotImplementedException();
            }
        }

        public ICollection<string> Keys => _dict.Keys;
        public ICollection<T> Values => _dict.Values;
        public int Count => _dict.Count;

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => _dict.Keys;

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values => _dict.Values;

        T IReadOnlyDictionary<string, T>.this[string key] => _dict[key];

        public void Touch()
        {
        }

        public T Add(string key, Action<IReferences, JObject> init = null)
        {
            JObject obj;
            if (typeof(T).IsSubclassOf(typeof(RefModel))) {
                obj = _refs.Create();
            }
            else {
                obj = new JObject();
            }
            init?.Invoke(_refs, obj);
            var accessor = new ModelDataAccessor(obj, _refs, _res);
            var item = _factory.Invoke(accessor);
            _dict.Add(key, item);
            _obj.Add(key, obj);
            return item;
        }

        public void AddRef<TRef>(string key, TRef item)
            where TRef : RefModel, T
        {
            _dict.Add(key, item);
            _obj.Add(key, _refs.CreateReference(item.Id));
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(string key)
        {
            _dict.Remove(key);
            return _obj.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        bool IReadOnlyDictionary<string, T>.TryGetValue(string key, out T value)
        {
            if (TryGetValue(key, out var list)) {
                value = list;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            foreach (var item in _dict) {
                yield return new KeyValuePair<string, T>(item.Key, item.Value);
            }
        }
    }
}
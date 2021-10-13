#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class DictionaryOfValueAccessor<TValue> : IDictionary<string, TValue>, IReadOnlyDictionary<string, TValue>
    {
        private readonly JObject _obj;
        private readonly Dictionary<string, TValue> _dict;

        public DictionaryOfValueAccessor(JObject obj)
        {
            _obj = obj;
            _dict = new Dictionary<string, TValue>(StringComparer.Ordinal);
            foreach (var property in obj.Properties()) {
                _dict.Add(property.Name, property.Value.Value<TValue>());
            }
        }

        public TValue this[string key]
        {
            get => _dict[key];
            set {
                _dict[key] = value;
                _obj[key] = JToken.FromObject(value);
            }
        }

        public ICollection<string> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;
        public int Count => _dict.Count;
        bool ICollection<KeyValuePair<string, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<string, TValue>>)_dict).IsReadOnly;

        IEnumerable<string> IReadOnlyDictionary<string, TValue>.Keys => _dict.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<string, TValue>.Values => _dict.Values;

        TValue IReadOnlyDictionary<string, TValue>.this[string key] => _dict[key];

        public void Add(string key, TValue value)
        {
            _dict.Add(key, value);
            _obj.Add(key, JToken.FromObject(value));
        }

        void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, TValue>> items)
        {
            foreach (var item in items) {
                Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            foreach (var key in _dict.Keys) {
                _obj.Remove(key);
            }
            _dict.Clear();
        }

        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item)
        {
            return _dict.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, TValue>>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(string key)
        {
            _dict.Remove(key);
            return _obj.Remove(key);
        }

        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
        {
            ((ICollection<KeyValuePair<string, TValue>>)_dict).Remove(item);
            return _obj.Remove(item.Key);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        bool IReadOnlyDictionary<string, TValue>.TryGetValue(string key, out TValue value)
        {
            if (TryGetValue(key, out var list)) {
                value = list;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator<KeyValuePair<string, TValue>> IEnumerable<KeyValuePair<string, TValue>>.GetEnumerator()
        {
            foreach (var item in _dict) {
                yield return new KeyValuePair<string, TValue>(item.Key, item.Value);
            }
        }
    }
}
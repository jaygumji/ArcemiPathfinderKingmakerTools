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

namespace Arcemi.Models
{
    public class DictionaryOfValueListAccessor<TValue> : IDictionary<string, ListValueAccessor<TValue>>, IReadOnlyDictionary<string, IReadOnlyList<TValue>>, IModelContainer
    {
        private readonly JObject _obj;
        private readonly Dictionary<string, ListValueAccessor<TValue>> _dict;

        public DictionaryOfValueListAccessor(JObject obj)
        {
            _obj = obj;
            _dict = new Dictionary<string, ListValueAccessor<TValue>>(StringComparer.Ordinal);
            ((IModelContainer)this).Refresh();
        }

        void IModelContainer.Refresh()
        {
            if (_dict.Count > 0) _dict.Clear();
            foreach (var property in _obj.Properties()) {
                var propertyValue = property.Value;
                if (propertyValue == null || !(propertyValue is JArray arr)) continue;

                var accessor = new ListValueAccessor<TValue>(arr);
                _dict.Add(property.Name, accessor);
            }
        }

        public ListValueAccessor<TValue> this[string key]
        {
            get => _dict[key];
            set {
                _dict[key] = value;
                _obj[key] = JToken.FromObject(value);
            }
        }

        public ICollection<string> Keys => _dict.Keys;
        public ICollection<ListValueAccessor<TValue>> Values => _dict.Values;
        public int Count => _dict.Count;
        bool ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>.IsReadOnly => ((ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>)_dict).IsReadOnly;

        IEnumerable<string> IReadOnlyDictionary<string, IReadOnlyList<TValue>>.Keys => _dict.Keys;

        IEnumerable<IReadOnlyList<TValue>> IReadOnlyDictionary<string, IReadOnlyList<TValue>>.Values => _dict.Values;

        IReadOnlyList<TValue> IReadOnlyDictionary<string, IReadOnlyList<TValue>>.this[string key] => _dict[key];

        public void Add(string key, ListValueAccessor<TValue> value)
        {
            _dict.Add(key, value);
            _obj.Add(key, value.UnderlyingStructure);
        }

        public ListValueAccessor<TValue> Add(string key)
        {
            var arr = new JArray();
            var list = new ListValueAccessor<TValue>(arr);
            _dict.Add(key, list);
            _obj.Add(key, arr);
            return list;
        }

        void ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>.Add(KeyValuePair<string, ListValueAccessor<TValue>> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        bool ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>.Contains(KeyValuePair<string, ListValueAccessor<TValue>> item)
        {
            return _dict.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>.CopyTo(KeyValuePair<string, ListValueAccessor<TValue>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, ListValueAccessor<TValue>>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(string key)
        {
            _dict.Remove(key);
            return _obj.Remove(key);
        }

        bool ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>.Remove(KeyValuePair<string, ListValueAccessor<TValue>> item)
        {
            ((ICollection<KeyValuePair<string, ListValueAccessor<TValue>>>)_dict).Remove(item);
            return _obj.Remove(item.Key);
        }

        public bool TryGetValue(string key, out ListValueAccessor<TValue> value)
        {
            return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        bool IReadOnlyDictionary<string, IReadOnlyList<TValue>>.TryGetValue(string key, out IReadOnlyList<TValue> value)
        {
            if (TryGetValue(key, out var list)) {
                value = list;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator<KeyValuePair<string, IReadOnlyList<TValue>>> IEnumerable<KeyValuePair<string, IReadOnlyList<TValue>>>.GetEnumerator()
        {
            foreach (var item in _dict) {
                yield return new KeyValuePair<string, IReadOnlyList<TValue>>(item.Key, item.Value);
            }
        }
    }
}
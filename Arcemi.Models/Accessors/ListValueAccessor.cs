#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class ListValueAccessor<T> : IList, IReadOnlyList<T>, IModelContainer
    {
        private readonly JArray _array;
        private readonly List<T> _items;

        public ListValueAccessor(JArray array)
        {
            _array = array;
            _items = new List<T>();
            ((IModelContainer)this).Refresh();
        }

        void IModelContainer.Refresh()
        {
            if (_items.Count > 0) _items.Clear();
            foreach (var item in _array) {
                _items.Add(item.Value<T>());
            }
        }

        public JArray UnderlyingStructure => _array;

        public T this[int index]
        {
            get {
                return _items[index];
            }
            set {
                _items[index] = value;
                _array[index] = JToken.FromObject(value);
            }
        }

        object IList.this[int index] { get => _items[index]; set => throw new NotImplementedException(); }

        public int Count => _items.Count;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((IList)_items).SyncRoot;

        int IList.Add(object value)
        {
            Add((T)value);
            return _items.Count - 1;
        }

        public void Add(T value)
        {
            _items.Add(value);
            _array.Add(JToken.FromObject(value));
        }

        public void Insert(int index, T value)
        {
            _items.Insert(index, value);
            _array.Insert(index, JToken.FromObject(value));
        }

        public void AddRange(IEnumerable<T> values)
        {
            _items.AddRange(values);
            foreach (var value in values) {
                _array.Add(JToken.FromObject(value));
            }
        }

        public void Clear()
        {
            _array.Clear();
            _items.Clear();
        }

        bool IList.Contains(object value)
        {
            return ((IList)_items).Contains(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_items).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_items).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        public bool Remove(T value)
        {
            var idx = _items.IndexOf(value);
            if (idx < 0) return false;
            _array.RemoveAt(idx);
            _items.RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int index)
        {
            _array.RemoveAt(index);
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
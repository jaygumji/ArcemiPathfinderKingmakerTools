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
    public class ListValueAccessor<T> : IList, IReadOnlyList<T>
    {
        private readonly JArray _array;
        private readonly List<T> _items;

        public ListValueAccessor(JArray array)
        {
            _array = array;
            _items = array
                .Select(t => t.Value<T>())
                .ToList() ?? new List<T>();
        }

        public JArray UnderlyingStructure => _array;

        public T this[int index] => _items[index];

        object IList.this[int index] { get => _items[index]; set => throw new NotImplementedException(); }

        public int Count => _items.Count;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((IList)_items).SyncRoot;

        int IList.Add(object value)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
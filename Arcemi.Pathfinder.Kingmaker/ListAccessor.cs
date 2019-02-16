#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ListAccessor<T> : IList, IReadOnlyList<T>, INotifyCollectionChanged
    {
        private readonly JArray _array;
        private readonly IReferences _refs;
        private readonly Func<ModelDataAccessor, T> _factory;
        private readonly List<T> _items;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ListAccessor(JArray array, IReferences refs, Func<ModelDataAccessor, T> factory)
        {
            _array = array;
            _refs = refs;
            _factory = factory;
            _items = array
                .Select(t => {
                    var obj = refs.GetReferred((JObject)t);
                    var accessor = new ModelDataAccessor(obj, _refs);
                    return factory.Invoke(accessor);
                })
                .ToList() ?? new List<T>();
        }

        public T Add(Action<IReferences, JObject> init = null)
        {
            JObject obj;
            if (typeof(T).IsSubclassOf(typeof(RefModel))) {
                obj = _refs.Create();
            }
            else {
                obj = new JObject();
            }
            init?.Invoke(_refs, obj);
            var accessor = new ModelDataAccessor(obj, _refs);
            var item = _factory.Invoke(accessor);
            _array.Add(obj);
            _items.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return item;
        }

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

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
            var item = _items[index];
            _array.RemoveAt(index);
            _items.RemoveAt(index);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}